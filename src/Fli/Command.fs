namespace Fli

open System.Text

[<AutoOpen>]
module Command =

    open Domain
    open Helpers
    open System
    open System.Text
    open System.Diagnostics
    open System.Runtime.InteropServices
    open System.Threading.Tasks

    let private shellToProcess (shell: Shells) (input: string option) =
        match shell with
        | CMD -> "cmd.exe", (if input.IsNone then "/c" else "/k")
        | PS -> "powershell.exe", "-Command"
        | PWSH -> "pwsh.exe", "-Command"
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

#if NET
    let private startProcessAsync (writeInputAsync: Process -> Task<unit>) (psi: ProcessStartInfo) =
        async {
            let proc = psi |> Process.Start
            do! proc |> writeInputAsync |> Async.AwaitTask

            let! text = proc.StandardOutput.ReadToEndAsync() |> Async.AwaitTask
            let! error = proc.StandardError.ReadToEndAsync() |> Async.AwaitTask
            do! proc.WaitForExitAsync() |> Async.AwaitTask

            return
                { Text = text |> toOption
                  ExitCode = proc.ExitCode
                  Error = error |> toOption }
        }
        |> Async.StartAsTask
        |> Async.AwaitTask
#endif

    let private startProcess (writeInputFunc: Process -> unit) (psi: ProcessStartInfo) =
        let proc = psi |> Process.Start
        proc |> writeInputFunc

        let text = proc.StandardOutput.ReadToEnd()
        let error = proc.StandardError.ReadToEnd()
        proc.WaitForExit()

        { Text = text |> toOption
          ExitCode = proc.ExitCode
          Error = error |> toOption }


    let private checkVerb (verb: string) (executable: string) =
        let verbs = ProcessStartInfo(executable).Verbs

        if not (verbs |> Array.contains verb) then
            $"""Unknown verb '{verb}'. Possible verbs on '{executable}': {verbs |> String.concat ", "}"""
            |> ArgumentException
            |> raise

    let private setReturn (func: unit) (psi: ProcessStartInfo) =
        func
        psi

    let private addVerb verb (psi: ProcessStartInfo) =
        setReturn (psi.Verb <- (verb |> Option.defaultValue null)) psi

    let private addWorkingDirectory workingDirectory (psi: ProcessStartInfo) =
        setReturn (psi.WorkingDirectory <- (workingDirectory |> Option.defaultValue "")) psi

    let private addUserName username (psi: ProcessStartInfo) =
        setReturn (psi.UserName <- (username |> Option.defaultValue "")) psi

    let private addEnvironmentVariables (variables: (string * string) list option) (psi: ProcessStartInfo) =
        match variables with
        | Some (v) -> v |> List.iter (psi.Environment.Add)
        | None -> ()

        psi

    let private addCredentials (credentials: Credentials option) (psi: ProcessStartInfo) =
        match credentials with
        | Some (Credentials (domain, username, password)) ->
            psi.UserName <- username

            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                psi.Domain <- domain
                psi.Password <- (password |> toSecureString)
        | None -> ()

        psi

    let private addEncoding (encoding: Encoding option) (psi: ProcessStartInfo) =
        match encoding with
        | Some (e) ->
            psi.StandardOutputEncoding <- e
            psi.StandardErrorEncoding <- e
        | None -> ()

        psi

    let private writeInput (input: string option) (encoding: Encoding option) (p: Process) =
        match input with
        | Some (inputText) ->
            use sw = p.StandardInput
            sw.WriteLine(inputText, encoding)
            sw.Flush()
            sw.Close()
        | None -> ()

    let private writeInputAsync (input: string option) (p: Process) =
        async {
            match input with
            | Some (inputText) ->
                use sw = p.StandardInput
                do! inputText |> sw.WriteLineAsync |> Async.AwaitTask
                sw.FlushAsync()
                sw.Close()
            | None -> ()
        }
        |> Async.StartAsTask

    type Command =
        static member internal buildProcess(context: ShellContext) =
            let (proc, flag) = (context.config.Shell, context.config.Input) ||> shellToProcess

            (createProcess proc $"""{flag} {context.config.Command |> Option.defaultValue ""}""")
            |> addWorkingDirectory context.config.WorkingDirectory
            |> addEnvironmentVariables context.config.EnvironmentVariables
            |> addEncoding context.config.Encoding

        static member internal buildProcess(context: ExecContext) =
            match context.config.Verb with
            | Some (verb) -> checkVerb verb context.config.Program
            | None -> ()

            (createProcess context.config.Program (context.config.Arguments |> Option.defaultValue ""))
            |> addVerb context.config.Verb
            |> addWorkingDirectory context.config.WorkingDirectory
            |> addUserName context.config.UserName
            |> addEnvironmentVariables context.config.EnvironmentVariables
            |> addCredentials context.config.Credentials
            |> addEncoding context.config.Encoding

        static member toString(context: ShellContext) =
            let (proc, flag) = (context.config.Shell, context.config.Input) ||> shellToProcess
            $"""{proc} {flag} {context.config.Command |> Option.defaultValue ""}"""

        static member toString(context: ExecContext) =
            $"""{context.config.Program} {context.config.Arguments |> Option.defaultValue ""}"""

        static member execute(context: ShellContext) =
            context
            |> Command.buildProcess
            |> startProcess (writeInput context.config.Input context.config.Encoding)

        static member execute(context: ExecContext) =
            context
            |> Command.buildProcess
            |> startProcess (writeInput context.config.Input context.config.Encoding)

#if NET
        static member executeAsync(context: ShellContext) =
            context
            |> Command.buildProcess
            |> startProcessAsync (writeInputAsync context.config.Input)

        static member executeAsync(context: ExecContext) =
            context
            |> Command.buildProcess
            |> startProcessAsync (writeInputAsync context.config.Input)
#endif
