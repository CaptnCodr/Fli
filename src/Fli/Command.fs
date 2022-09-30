namespace Fli

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

    let private createProcess (executable: string) (argumentString: string) (workingDirectory: string option) =
        ProcessStartInfo(
            FileName = executable,
            Arguments = argumentString,
            WorkingDirectory = (workingDirectory |> Option.defaultValue ""),
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

            (proc, $"{flag} {context.config.Command}", context.config.WorkingDirectory)
            |||> createProcess
            |> startProcess

        static member toString(context: ShellContext) =
            let (proc, flag) = context.config.Shell |> shellToProcess
            $"{proc} {flag} {context.config.Command}"

        static member execute(context: ProgramContext) =
            (context.config.Program, context.config.Arguments, context.config.WorkingDirectory)
            |||> createProcess
            |> startProcess

        static member toString(context: ProgramContext) =
            $"{context.config.Program} {context.config.Arguments}"
