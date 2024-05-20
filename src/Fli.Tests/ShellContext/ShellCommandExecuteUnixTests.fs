module Fli.Tests.ShellContext.ShellCommandExecuteUnixTests

open NUnit.Framework
open FsUnit
open Fli
open System
open System.Text
open System.Diagnostics

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Hello World with CUSTOM shell`` () =
    cli {
        Shell(CUSTOM("bash", "-c"))
        Command "echo Hello World!"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Hello World!"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Get output in StringBuilder`` () =
    let sb = StringBuilder()

    cli {
        Shell BASH
        Command "echo Test"
        Output sb
    }
    |> Command.execute
    |> ignore

    sb.ToString() |> should equal "Test\n"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Hello World with SH`` () =
    cli {
        Shell SH
        Command "echo Hello World!"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Hello World!"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Hello World with BASH`` () =
    cli {
        Shell BASH
        Command "echo Hello World!"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Hello World!"

[<Test>]
[<Platform("MacOsX")>]
let ``Hello World with ZSH`` () =
    cli {
        Shell ZSH
        Command "echo Hello World!"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Hello World!"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Input text in BASH`` () =
    cli {
        Shell BASH
        Command "echo Hello World!"
        Input "echo Test"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Hello World!"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Hello World with BASH async`` () =
    async {
        let! output =
            cli {
                Shell BASH
                Command "echo Hello World!"
            }
            |> Command.executeAsync

        output |> Output.toText |> should equal "Hello World!"
    }

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``BASH returning non zero ExitCode`` () =
    cli {
        Shell BASH
        Command "echl Test"
    }
    |> Command.execute
    |> Output.toExitCode
    |> should equal 127

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``BASH returning non zero process id`` () =
    cli {
        Shell BASH
        Command "echo Test"
    }
    |> Command.execute
    |> Output.toId
    |> should not' (equal 0)
