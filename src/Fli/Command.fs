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

    let private startProcess (``process``: string) (argumentString: string) =
        let info = ProcessStartInfo(``process``, argumentString)
        info.WindowStyle <- ProcessWindowStyle.Hidden
        info.CreateNoWindow <- true
        info.UseShellExecute <- false
        info.RedirectStandardOutput <- true

        Process.Start(info).StandardOutput.ReadToEnd()

    type Command =
        static member execute(context: ShellContext) =
            let (proc, flag) = context.config.Shell |> shellToProcess
            (proc, $"{flag} {context.config.Command}") ||> startProcess

        static member toString(context: ShellContext) =
            let (proc, flag) = context.config.Shell |> shellToProcess
            $"{proc} {flag} {context.config.Command}"

        static member execute(context: ProgramContext) =
            (context.config.Program, context.config.Arguments) ||> startProcess

        static member toString(context: ProgramContext) =
            $"{context.config.Program} {context.config.Arguments}"
