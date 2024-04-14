namespace Fli

[<AutoOpen>]
module Command =

    open Domain
    open Helpers
    open Extensions
    open System
    open System.IO
    open System.Text
    open System.Diagnostics
    open System.Runtime.InteropServices
    open System.Threading

    let private shellToProcess (shell: Shells) (input: string option) =
        match shell with
        | CMD -> "cmd.exe", (if input.IsNone then "/c" else "/k")
        | PS -> "powershell.exe", "-Command"
        | PWSH -> "pwsh.exe", "-Command"
        | WSL -> "wsl.exe", "--"
        | SH -> "sh", "-c"
        | BASH -> "bash", "-c"
        | ZSH -> "zsh", "-c"
        | CUSTOM(shell, flag) -> shell, flag

    let private toOption =
        function
        | null
        | "" -> None
        | _ as s -> Some s

    let private createProcess executable argumentString openDefault =
        ProcessStartInfo(
            FileName = executable,
            Arguments = argumentString,
            CreateNoWindow = true,
            UseShellExecute = openDefault,
            RedirectStandardInput = not (openDefault),
            RedirectStandardOutput = not (openDefault),
            RedirectStandardError = not (openDefault)
        )

    let private trim (s: string) = s.TrimEnd([| '\r'; '\n' |])

    let returnOr (sb: StringBuilder) (output: string) =
        match (sb.ToString(), output) with
        | (t, _) when t.Length > 0 -> t
        | (_, o) when o.Length > 0 -> o
        | (_, _) -> ""

#if NET
    let private startProcessAsync
        (inFunc: Process -> Tasks.Task<unit>)
        (outFunc: string -> unit)
        cancellationToken
        psi
        =
        async {
            let proc = Process.Start(startInfo = psi)
            do! proc |> inFunc |> Async.AwaitTask

            let sbStd = StringBuilder()
            let sbErr = StringBuilder()

            proc.OutputDataReceived.AddHandler(
                new DataReceivedEventHandler(fun s e ->
                    use o = proc.StandardOutput
                    sbStd.Append(o.ReadToEnd()) |> ignore)
            )

            proc.ErrorDataReceived.AddHandler(
                new DataReceivedEventHandler(fun s e ->
                    use o = proc.StandardError
                    sbErr.Append(o.ReadToEnd()) |> ignore)
            )

            try
                do! proc.WaitForExitAsync(cancellationToken) |> Async.AwaitTask
            with :? OperationCanceledException ->
                ()

            cancellationToken.ThrowIfCancellationRequested()
            let! stdo = proc.StandardOutput.ReadToEndAsync() |> Async.AwaitTask
            let text = returnOr sbStd stdo

            let! stde = proc.StandardError.ReadToEndAsync() |> Async.AwaitTask
            let error = returnOr sbErr stde

            do text |> outFunc

            return
                { Id = proc.Id
                  Text = text |> trim |> toOption
                  ExitCode = proc.ExitCode
                  Error = error |> trim |> toOption }
        }
        |> Async.StartAsTask
        |> Async.AwaitTask
#endif

    let private startProcess (inputFunc: Process -> unit) (outputFunc: string -> unit) psi =
        let proc = Process.Start(startInfo = psi)
        proc |> inputFunc

        let text =
            if psi.UseShellExecute |> not then
                proc.StandardOutput.ReadToEnd()
            else
                ""

        let error =
            if psi.UseShellExecute |> not then
                proc.StandardError.ReadToEnd()
            else
                ""

        proc.WaitForExit()

        text |> outputFunc

        { Id = proc.Id
          Text = text |> trim |> toOption
          ExitCode = proc.ExitCode
          Error = error |> trim |> toOption }


    let private checkVerb (verb: string option) (executable: string) =
        match verb with
        | Some(v) ->
            let verbs = ProcessStartInfo(executable).Verbs

            if not (verbs |> Array.contains v) then
                $"""Unknown verb '{v}'. Possible verbs on '{executable}': {verbs |> String.concat ", "}"""
                |> ArgumentException
                |> raise
        | None -> ()

    let private addEnvironmentVariables (variables: (string * string) list option) (psi: ProcessStartInfo) =
        if psi.UseShellExecute |> not then
            ((variables |> Option.defaultValue [] |> List.iter (psi.Environment.Add)), psi)
            |> snd
        else
            psi

    let private addCredentials (credentials: Credentials option) (psi: ProcessStartInfo) =
        match credentials with
        | Some(Credentials(domain, username, password)) ->
            psi.UserName <- username

            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                psi.Domain <- domain
                psi.Password <- (password |> toSecureString)

            psi
        | None -> psi

    let private writeInput (input: string option) (encoding: Encoding option) (p: Process) =
        match input with
        | Some(inputText) ->
            try
                use sw = p.StandardInput
                sw.WriteLine(inputText, encoding)
                sw.Flush()
                sw.Close()
            with :? IOException as ex when ex.GetType() = typedefof<IOException> ->
                ()
        | None -> ()

    let private writeInputAsync (input: string option) (p: Process) =
        async {
            match input with
            | Some(inputText) ->
                try
                    use sw = p.StandardInput
                    do! inputText |> sw.WriteLineAsync |> Async.AwaitTask
                    do! sw.FlushAsync() |> Async.AwaitTask
                    sw.Close()
                with :? IOException as ex when ex.GetType() = typedefof<IOException> ->
                    ()
            | None -> ()
        }
        |> Async.StartAsTask

    let private writeOutput (outputType: Outputs option) (output: string) =
        match outputType with
        | Some(o) ->
            match o with
            | Outputs.File(file) -> File.WriteAllText(file, output)
            | Outputs.StringBuilder(stringBuilder) -> output |> stringBuilder.Append |> ignore
            | Outputs.Custom(func) -> func.Invoke(output)

        | None -> ()

    let private setupCancellationToken (cancelAfter: int option) =
        let cts = new CancellationTokenSource()

        match cancelAfter with
        | None -> ()
        | Some ca -> cts.CancelAfter(ca)

        cts.Token

    let private quoteBashCommand (context: ShellContext) =
        let noQuoteNeeded = [|
            Shells.CMD
            Shells.PWSH
            Shells.PS
            Shells.WSL
        |]

        match Array.contains context.config.Shell noQuoteNeeded with
        | true -> context.config.Command |> Option.defaultValue ""
        | false -> context.config.Command |> Option.defaultValue "" |> (fun s -> $"\"{s}\"")

    let private getProcessWindowStyle (windowStyle: WindowStyle) =
        match windowStyle with
        | Hidden -> ProcessWindowStyle.Hidden
        | Maximized -> ProcessWindowStyle.Maximized
        | Minimized -> ProcessWindowStyle.Minimized
        | Normal -> ProcessWindowStyle.Normal

    type Command =
        static member internal buildProcess(context: ShellContext) =
            let (proc, flag) = (context.config.Shell, context.config.Input) ||> shellToProcess
            let command = context |> quoteBashCommand

            (createProcess proc $"""{flag} {command}""" false)
                .With(WorkingDirectory = (context.config.WorkingDirectory |> Option.defaultValue ""))
                .With(StandardOutputEncoding = (context.config.Encoding |> Option.defaultValue null))
                .With(StandardErrorEncoding = (context.config.Encoding |> Option.defaultValue null))
                .With(WindowStyle = getProcessWindowStyle (context.config.WindowStyle |> Option.defaultValue Hidden))
            |> addEnvironmentVariables context.config.EnvironmentVariables

        static member internal buildProcess(context: ExecContext) =
            checkVerb context.config.Verb context.config.Program

            let arguments = (context.config.Arguments |> Option.defaultValue "")

            let openDefault =
                if arguments = "" && context.config.EnvironmentVariables.IsNone then
                    true
                else
                    false

            (createProcess context.config.Program arguments openDefault)
                .With(Verb = (context.config.Verb |> Option.defaultValue null))
                .With(WorkingDirectory = (context.config.WorkingDirectory |> Option.defaultValue ""))
                .With(UserName = (context.config.UserName |> Option.defaultValue ""))
                .With(StandardOutputEncoding = (context.config.Encoding |> Option.defaultValue null))
                .With(StandardErrorEncoding = (context.config.Encoding |> Option.defaultValue null))
                .With(WindowStyle = getProcessWindowStyle (context.config.WindowStyle |> Option.defaultValue Hidden))
            |> addCredentials context.config.Credentials
            |> addEnvironmentVariables context.config.EnvironmentVariables

        /// Stringifies shell + opening flag and given command.
        static member toString(context: ShellContext) =
            let (proc, flag) = (context.config.Shell, context.config.Input) ||> shellToProcess
            let command = context |> quoteBashCommand
            $"""{proc} {flag} {command}"""

        /// Stringifies executable + arguments.
        static member toString(context: ExecContext) =
            $"""{context.config.Program} {context.config.Arguments |> Option.defaultValue ""}"""

        /// Executes the given context as a new process.
        static member execute(context: ShellContext) =
            context
            |> Command.buildProcess
            |> startProcess
                (writeInput context.config.Input context.config.Encoding)
                (writeOutput context.config.Output)

        /// Executes the given context as a new process.
        static member execute(context: ExecContext) =
            context
            |> Command.buildProcess
            |> startProcess
                (writeInput context.config.Input context.config.Encoding)
                (writeOutput context.config.Output)

#if NET
        /// Executes the given context as a new process asynchronously.
        static member executeAsync(context: ShellContext) =
            context
            |> Command.buildProcess
            |> startProcessAsync
                (writeInputAsync context.config.Input)
                (writeOutput context.config.Output)
                (setupCancellationToken context.config.CancelAfter)

        /// Executes the given context as a new process asynchronously.
        static member executeAsync(context: ExecContext) =
            context
            |> Command.buildProcess
            |> startProcessAsync
                (writeInputAsync context.config.Input)
                (writeOutput context.config.Output)
                (setupCancellationToken context.config.CancelAfter)
#endif
