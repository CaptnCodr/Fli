module FsCli.CliCommandToStringTests

open NUnit.Framework
open FsUnit
open FsCli.CE
open System.Runtime.Versioning


[<Test>]
[<SupportedOSPlatform("windows")>]
let ``CMD command toString returns full line`` () =
    cli {
        CLI CMD
        Command "echo Hello World!"
    }
    |> Command.toString
    |> should equal "cmd.exe /C echo Hello World!"

[<Test>]
[<SupportedOSPlatform("windows")>]
let ``PWSH command toString returns full line`` () =
    cli {
        CLI PWSH
        Command "Write-Host Hello World!"
    }
    |> Command.toString
    |> should equal "pwsh.exe -Command Write-Host Hello World!"

[<Test>]
[<SupportedOSPlatform("linux")>]
let ``Bash command toString returns full line`` () =
    cli {
        CLI Bash
        Command "\"echo Hello World!\""
    }
    |> Command.toString
    |> should equal "bash -c \"echo Hello World!\""
