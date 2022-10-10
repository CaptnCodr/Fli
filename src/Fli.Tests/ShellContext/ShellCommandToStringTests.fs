module Fli.ShellContext.ShellCommandToStringTests

open NUnit.Framework
open FsUnit
open Fli


[<Test>]
let ``CMD command toString returns full line`` () =
    cli {
        Shell CMD
        Command "echo Hello World!"
    }
    |> Command.toString
    |> should equal "cmd.exe /c echo Hello World!"

[<Test>]
let ``CMD command toString returns full line in interactive mode`` () =
    cli {
        Shell CMD
        Command "echo Hello World!"
        Input "echo Hello World!"
    }
    |> Command.toString
    |> should equal "cmd.exe /k echo Hello World!"

[<Test>]
let ``PS command toString returns full line`` () =
    cli {
        Shell PS
        Command "Write-Host Hello World!"
    }
    |> Command.toString
    |> should equal "powershell.exe -Command Write-Host Hello World!"

[<Test>]
let ``PWSH command toString returns full line`` () =
    cli {
        Shell PWSH
        Command "Write-Host Hello World!"
    }
    |> Command.toString
    |> should equal "pwsh.exe -Command Write-Host Hello World!"

[<Test>]
let ``BASH command toString returns full line`` () =
    cli {
        Shell BASH
        Command "\"echo Hello World!\""
    }
    |> Command.toString
    |> should equal "bash -c \"echo Hello World!\""
