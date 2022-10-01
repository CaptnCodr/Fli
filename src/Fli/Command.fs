namespace Fli

open System.Net

[<AutoOpen>]
module Command =

    open Domain
    open System.Diagnostics

    let private shellToProcess =
        function
        | CMD -> "cmd.exe", "/C"
        | PS -> "powershell.exe", "-Command"
        | PWSH -> "pwsh.exe", "-Command"
        | BASH -> "bash", "-c"

    let private createProcess (executable: string) (argumentString: string) (workingDirectory: string option) (credentials: NetworkCredential option) =
        ProcessStartInfo(
            FileName = executable,
            Arguments = argumentString,
            WorkingDirectory = (workingDirectory |> Option.defaultValue ""),
            Domain = (if credentials.IsSome then credentials.Value.Domain else ""),
            UserName = (if credentials.IsSome then credentials.Value.UserName else ""),
            Password = (if credentials.IsSome then credentials.Value.SecurePassword else null),
            PasswordInClearText = (if credentials.IsSome then credentials.Value.Password else null),
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true
        )

    let private startProcess (psi: ProcessStartInfo) =
        Process.Start(psi).StandardOutput.ReadToEnd()

    type Command =
        static member execute(context: ShellContext) =
            let (proc, flag) = context.config.Shell |> shellToProcess

            (createProcess proc $"{flag} {context.config.Command}" context.config.WorkingDirectory context.config.Credentials)
            |> startProcess

        static member toString(context: ShellContext) =
            let (proc, flag) = context.config.Shell |> shellToProcess
            $"{proc} {flag} {context.config.Command}"

        static member execute(context: ProgramContext) =
            (createProcess context.config.Program context.config.Arguments context.config.WorkingDirectory context.config.Credentials)
            |> startProcess

        static member toString(context: ProgramContext) =
            $"{context.config.Program} {context.config.Arguments}"
