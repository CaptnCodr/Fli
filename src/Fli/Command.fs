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

    let private shellToProcess (shell: Shells) (input: string option) =
        match shell with
        | CMD -> "cmd.exe", (if input.IsNone then "/c" else "/k")
        | PS -> "powershell.exe", "-Command"
        | PWSH -> "pwsh.exe", "-Command"
        | WSL -> "wsl.exe", "--"
        | BASH -> "bash", "-c"

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

#if NET
    let private startProcessAsync
        (writeInputAsync: Process -> Threading.Tasks.Task<unit>)
        (writeOutputFunc: string -> unit)
        (psi: ProcessStartInfo)
        =
        async {
            let proc = psi |> Process.Start
            do! proc |> writeInputAsync |> Async.AwaitTask

            let! text = proc.StandardOutput.ReadToEndAsync() |> Async.AwaitTask
            let! error = proc.StandardError.ReadToEndAsync() |> Async.AwaitTask
            do! proc.WaitForExitAsync() |> Async.AwaitTask

            do text |> writeOutputFunc

            return
                { Id = proc.Id
                  Text = text |> trim |> toOption
                  ExitCode = proc.ExitCode
                  Error = error |> trim |> toOption }
        }
        |> Async.StartAsTask
        |> Async.AwaitTask
#endif

    let private startProcess
        (writeInputFunc: Process -> unit)
        (writeOutputFunc: string -> unit)
        (psi: ProcessStartInfo)
        =
        let proc = psi |> Process.Start
        proc |> writeInputFunc

        let text = proc.StandardOutput.ReadToEnd()
        let error = proc.StandardError.ReadToEnd()
        proc.WaitForExit()

        text |> writeOutputFunc

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
            context
            |> Command.buildProcess
            |> startProcessAsync (writeInputAsync context.config.Input) (writeOutput context.config.Output)

        /// Executes the given context as a new process asynchronously.
        static member executeAsync(context: ExecContext) =
            context
            |> Command.buildProcess
            |> startProcessAsync (writeInputAsync context.config.Input) (writeOutput context.config.Output)
#endif
