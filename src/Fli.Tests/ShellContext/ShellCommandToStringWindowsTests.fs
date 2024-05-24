module Fli.Tests.ShellContext.ShellCommandToStringWindowsTests

open NUnit.Framework
open FsUnit
open Fli
open System


[<Test>]
[<Platform("Win")>]
let ``CMD command toString returns full line`` () =
    cli {
        Shell CMD
        Command "echo Hello World!"
    }
    |> Command.toString
    |> should equal "cmd.exe /c echo Hello World!"

[<Test>]
[<Platform("Win")>]
let ``CMD command toString returns full line in interactive mode`` () =
    cli {
        Shell CMD
        Command "echo Hello World!"
        Input "echo Hello World!"
    }
    |> Command.toString
    |> should equal "cmd.exe /k echo Hello World!"

[<Test>]
[<Platform("Win")>]
let ``PS command toString returns full line`` () =
    cli {
        Shell PS
        Command "Write-Host Hello World!"
    }
    |> Command.toString
    |> should equal "powershell.exe -Command Write-Host Hello World!"

[<Test>]
[<Platform("Win")>]
let ``PWSH command toString returns full line`` () =
    cli {
        Shell PWSH
        Command "Write-Host Hello World!"
    }
    |> Command.toString
    |> should equal "pwsh.exe -Command Write-Host Hello World!"

[<Test>]
[<Platform("Win")>]
let ``WSL command toString returns full line`` () =
    cli {
        Shell WSL
        Command "echo Hello World!"
    }
    |> Command.toString
    |> should equal "wsl.exe -- echo Hello World!"
