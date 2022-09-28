module FsCli.CliCommandToStringTests

open NUnit.Framework
open FsUnit
open FsCli


[<Test>]
let ``CMD command toString returns full line`` () =
    cli {
        CLI CMD
        Command "echo Hello World!"
    }
    |> Command.toString
    |> should equal "cmd.exe /C echo Hello World!"

[<Test>]
let ``PS command toString returns full line`` () =
    cli {
        CLI PS
        Command "Write-Host Hello World!"
    }
    |> Command.toString
    |> should equal "powershell.exe -Command Write-Host Hello World!"

[<Test>]
let ``PWSH command toString returns full line`` () =
    cli {
        CLI PWSH
        Command "Write-Host Hello World!"
    }
    |> Command.toString
    |> should equal "pwsh.exe -Command Write-Host Hello World!"

[<Test>]
let ``BASH command toString returns full line`` () =
    cli {
        CLI BASH
        Command "\"echo Hello World!\""
    }
    |> Command.toString
    |> should equal "bash -c \"echo Hello World!\""
