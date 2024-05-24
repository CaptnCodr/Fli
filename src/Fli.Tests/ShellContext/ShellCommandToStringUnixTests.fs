module Fli.Tests.ShellContext.ShellCommandToStringUnixTests

open NUnit.Framework
open FsUnit
open Fli
open System

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``PWSH command toString returns full line`` () =
    cli {
        Shell PWSH
        Command "Write-Host Hello World!"
    }
    |> Command.toString
    |> should equal "pwsh -Command Write-Host Hello World!"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``BASH command toString returns full line`` () =
    cli {
        Shell BASH
        Command "echo Hello World!"
    }
    |> Command.toString
    |> should equal "bash -c \"echo Hello World!\""
