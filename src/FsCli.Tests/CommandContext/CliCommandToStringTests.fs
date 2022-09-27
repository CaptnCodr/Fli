module FsCli.CliCommandToStringTests

open NUnit.Framework
open FsUnit
open FsCli.CE


[<Test>]
let ``CMD command toString returns full line`` () =
    cli {
        CLI CMD
        Command "echo Hello World!"
    }
    |> Command.toString
    |> should equal "cmd.exe /C echo Hello World!"

[<Test>]
let ``PWSH command toString returns full line`` () =
    cli {
        CLI PWSH
        Command "Write-Host Hello World!"
    }
    |> Command.toString
    |> should equal "pwsh.exe -Command Write-Host Hello World!"
