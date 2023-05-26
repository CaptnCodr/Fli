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
        | BASH -> "bash", "-c"
        | CUSTOM(shell, flag) -> shell, flag

    let private toOption =
        function
        | null
        | "" -> None
        | _ as s -> Some s

    let private createProcess executable argumentString =
        ProcessStartInfo(
            FileName = executable,
            Arguments = argumentString,
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        )

    let private trim (s: string) = s.TrimEnd([| '\r'; '\n' |])

    let private duration (startTime: DateTime) (endTime: DateTime) =
        endTime.Subtract(startTime)

#if NET
    let private startProcessAsync (inputFunc: Process -> Threading.Tasks.Task<unit>) (outputFunc: string -> unit) cancellationToken psi =
        async {
            let proc = Process.Start(startInfo = psi)
            do! proc |> inputFunc |> Async.AwaitTask

            let sbStd = StringBuilder()
            proc.OutputDataReceived.AddHandler(new DataReceivedEventHandler(fun s e -> 
                use o = proc.StandardOutput
                sbStd.Append(o.ReadToEnd()) |> ignore
            ))

            let sbErr = StringBuilder()
            proc.ErrorDataReceived.AddHandler(new DataReceivedEventHandler(fun s e -> 
                use o = proc.StandardError
                sbErr.Append(o.ReadToEnd()) |> ignore
            ))
            
            let text = sbStd.ToString()
            let error = sbErr.ToString()
            
            try
                do! proc.WaitForExitAsync(cancellationToken) |> Async.AwaitTask
            with 
            | :? OperationCanceledException -> ()

            do text |> outputFunc

            return { Id = proc.Id
                     Text = text |> trim |> toOption
                     ExitCode = proc.ExitCode
                     Error = error |> trim |> toOption
                     Duration = Some (duration proc.StartTime proc.ExitTime) }
        }
        |> Async.StartAsTask
        |> Async.AwaitTask
#endif

    let private startProcess (inputFunc: Process -> unit) (outputFunc: string -> unit) psi =
        let proc = Process.Start(startInfo = psi)
        proc |> inputFunc

        let text = proc.StandardOutput.ReadToEnd()
        let error = proc.StandardError.ReadToEnd()
        proc.WaitForExit()

        text |> outputFunc

        { Id = proc.Id
          Text = text |> trim |> toOption
          ExitCode = proc.ExitCode
          Error = error |> trim |> toOption
          Duration = Some (duration proc.StartTime proc.ExitTime) }


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
        ((variables |> Option.defaultValue [] |> List.iter (psi.Environment.Add)), psi)
        |> snd

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

    type Command =
        static member internal buildProcess(context: ShellContext) =
            let (proc, flag) = (context.config.Shell, context.config.Input) ||> shellToProcess

            (createProcess proc $"""{flag} {context.config.Command |> Option.defaultValue ""}""")
                .With(WorkingDirectory = (context.config.WorkingDirectory |> Option.defaultValue ""))
                .With(StandardOutputEncoding = (context.config.Encoding |> Option.defaultValue null))
                .With(StandardErrorEncoding = (context.config.Encoding |> Option.defaultValue null))
            |> addEnvironmentVariables context.config.EnvironmentVariables

        static member internal buildProcess(context: ExecContext) =
            checkVerb context.config.Verb context.config.Program

            (createProcess context.config.Program (context.config.Arguments |> Option.defaultValue ""))
                .With(Verb = (context.config.Verb |> Option.defaultValue null))
                .With(WorkingDirectory = (context.config.WorkingDirectory |> Option.defaultValue ""))
                .With(UserName = (context.config.UserName |> Option.defaultValue ""))
                .With(StandardOutputEncoding = (context.config.Encoding |> Option.defaultValue null))
                .With(StandardErrorEncoding = (context.config.Encoding |> Option.defaultValue null))
            |> addCredentials context.config.Credentials
            |> addEnvironmentVariables context.config.EnvironmentVariables

        /// Stringifies shell + opening flag and given command.
        static member toString(context: ShellContext) =
            let (proc, flag) = (context.config.Shell, context.config.Input) ||> shellToProcess
            $"""{proc} {flag} {context.config.Command |> Option.defaultValue ""}"""

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
            let cts = new CancellationTokenSource()
            match context.config.CancelAfter with 
            | None -> ()
            | Some ca -> cts.CancelAfter(ca)

            context
            |> Command.buildProcess
            |> startProcessAsync 
                (writeInputAsync context.config.Input) 
                (writeOutput context.config.Output) 
                cts.Token

        /// Executes the given context as a new process asynchronously.
        static member executeAsync(context: ExecContext) =
            let cts = new CancellationTokenSource()
            match context.config.CancelAfter with 
            | None -> ()
            | Some ca -> cts.CancelAfter(ca)

            context
            |> Command.buildProcess
            |> startProcessAsync 
                (writeInputAsync context.config.Input) 
                (writeOutput context.config.Output) 
                cts.Token
#endif
