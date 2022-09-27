[<AutoOpen>]
module FsCli.Command

open System.Diagnostics

open FsCli

let private cliToProc =
    function
    | CMD -> "cmd.exe", "/C"
    | PWSH -> "pwsh.exe", "-Command"

let private startProcess (``process``: string) (argumentString: string) =
    let info = ProcessStartInfo(``process``, argumentString)
    info.WindowStyle <- ProcessWindowStyle.Hidden
    info.CreateNoWindow <- true
    info.UseShellExecute <- false
    info.RedirectStandardOutput <- true

    Process.Start(info).StandardOutput.ReadToEnd()

type Command =
    static member execute(context: CliContext) =
        let (proc, flag) = context.config.Cli |> cliToProc
        (proc, $"{flag} {context.config.Command}") ||> startProcess

    static member toString(context: CliContext) =
        let (proc, flag) = context.config.Cli |> cliToProc
        $"{proc} {flag} {context.config.Command}"

    static member execute(context: ProgramContext) =
        (context.config.Program, context.config.Arguments) ||> startProcess

    static member toString(context: ProgramContext) =
        $"{context.config.Program} {context.config.Arguments}"
