namespace Fli

[<AutoOpen>]
module Command =

    open Domain
    open Helpers
    open System
    open System.Diagnostics
    open System.Runtime.InteropServices

    let private shellToProcess =
        function
        | CMD -> "cmd.exe", "/C"
        | PS -> "powershell.exe", "-Command"
        | PWSH -> "pwsh.exe", "-Command"
        | BASH -> "bash", "-c"

    let private createProcess executable argumentString =
        ProcessStartInfo(
            FileName = executable,
            Arguments = argumentString,
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true
        )

    let private startProcess (psi: ProcessStartInfo) =
        Process.Start(psi).StandardOutput.ReadToEnd()

    let private checkVerb (verb: string) (executable: string) =
        let verbs = ProcessStartInfo(executable).Verbs

        if not (verbs |> Array.contains verb) then
            $"""Unknown verb '{verb}'. Possible verbs on '{executable}': {verbs |> String.concat ", "}"""
            |> ArgumentException
            |> raise

    let private addVerb verb (psi: ProcessStartInfo) =
        psi.Verb <- (verb |> Option.defaultValue null)
        psi

    let private addWorkingDirectory workingDirectory (psi: ProcessStartInfo) =
        psi.WorkingDirectory <- (workingDirectory |> Option.defaultValue "")
        psi

    let private addUserName username (psi: ProcessStartInfo) =
        psi.UserName <- (username |> Option.defaultValue "")
        psi

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


    type Command =
        static member internal buildProcess(context: ShellContext) =
            let (proc, flag) = context.config.Shell |> shellToProcess

            (createProcess proc $"{flag} {context.config.Command}")
            |> addWorkingDirectory context.config.WorkingDirectory
            |> addEnvironmentVariables context.config.EnvironmentVariables

        static member execute(context: ShellContext) =
            context |> (Command.buildProcess >> startProcess)

        static member toString(context: ShellContext) =
            let (proc, flag) = context.config.Shell |> shellToProcess
            $"{proc} {flag} {context.config.Command}"

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

        static member execute(context: ExecContext) =
            context |> (Command.buildProcess >> startProcess)

        static member toString(context: ExecContext) =
            $"""{context.config.Program} {context.config.Arguments |> Option.defaultValue ""}"""
