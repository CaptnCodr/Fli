module FsCli.CliCommandExecuteTests

open NUnit.Framework
open FsUnit
open FsCli
open System.Runtime.Versioning


[<Test>]
[<SupportedOSPlatform("windows")>]
let ``Hello World with CMD`` () =
    cli {
        CLI CMD
        Command "echo Hello World!"
    }
    |> Command.execute
    |> should equal "Hello World!\r\n"

[<Test>]
[<SupportedOSPlatform("windows")>]
let ``Hello World with PWSH`` () =
    cli {
        CLI PWSH
        Command "Write-Host Hello World!"
    }
    |> Command.execute
    |> should equal "Hello World!\r\n"

[<Test>]
[<SupportedOSPlatform("linux")>]
let ``Hello World with Bash`` () =
    cli {
        CLI Bash
        Command "\"echo Hello World!\""
    }
    |> Command.execute
    |> should equal "Hello World!\n"
