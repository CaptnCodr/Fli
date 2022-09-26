module FsCli.Command

open System.Diagnostics

open FsCli

let private cliToProc =
    function
    | CMD -> "cmd.exe", "/C"
    | PWSH -> "pwsh.exe", "-Command"

let execute (context: CommandContext) =
    let (proc, flag) = context.config.Cli |> cliToProc

    let info = ProcessStartInfo(proc, $"{flag} {context.config.Command}")
    info.WindowStyle <- ProcessWindowStyle.Hidden
    info.CreateNoWindow <- true
    info.RedirectStandardOutput <- true
    info.RedirectStandardError <- true

    Process.Start(info).StandardOutput.ReadToEnd()

let toString (context: CommandContext) =
    let (proc, flag) = context.config.Cli |> cliToProc
    $"{proc} {flag} {context.config.Command}"
