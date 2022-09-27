module FsCli.CliCommandExecuteTests

open NUnit.Framework
open FsUnit
open FsCli


[<Test>]
let ``Hello World with CMD`` () =
    cli {
        CLI CMD
        Command "echo Hello World!"
    }
    |> Command.execute
    |> should equal "Hello World!\r\n"

[<Test>]
let ``Hello World with PWSH`` () =
    cli {
        CLI PWSH
        Command "Write-Host Hello World!"
    }
    |> Command.execute
    |> should equal "Hello World!\r\n"
