module Fli.Tests.ShellContext.ShellCommandExecuteWindowsTests

open NUnit.Framework
open FsUnit
open Fli
open System
open System.Text
open System.Diagnostics


[<Test>]
[<Platform("Win")>]
let ``Hello World with CMD`` () =
    let operation =
        cli {
            Shell CMD
            Command "echo Hello World!"
        }
        |> Command.execute

    operation |> Output.toText |> should equal "Hello World!"
    operation |> Output.toError |> should equal ""

[<Test>]
[<Platform("Win")>]
let ``Hello World with CMD waiting async`` () =
    async {
        let stopwatch = new Stopwatch()
        stopwatch.Start()

        try
            let! operation =
                cli {
                    Shell(CUSTOM("cmd.exe", "/K"))
                    Command "Hello World!"
                    CancelAfter 3000
                }
                |> Command.executeAsync

            ()
        with :? AggregateException as e ->
            e.GetType() |> should equal typeof<AggregateException>

        stopwatch.Stop()
        stopwatch.Elapsed.TotalSeconds |> should be (inRange 2.9 3.2)
    }

[<Test>]
[<Platform("Win")>]
let ``Hello World with CUSTOM shell`` () =
    cli {
        Shell(CUSTOM("cmd.exe", "/c"))
        Command "echo Hello World!"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Hello World!"

[<Test>]
[<Platform("Win")>]
let ``CMD returning non zero ExitCode`` () =
    cli {
        Shell CMD
        Command "echl Test"
    }
    |> Command.execute
    |> Output.toExitCode
    |> should equal 1

[<Test>]
[<Platform("Win")>]
let ``Get output in StringBuilder`` () =
    let sb = StringBuilder()

    cli {
        Shell CMD
        Command "echo Test"
        Output sb
    }
    |> Command.execute
    |> ignore

    sb.ToString() |> should equal "Test\r\n"

[<Test>]
[<Platform("Win")>]
let ``CMD returning non zero process id`` () =
    cli {
        Shell CMD
        Command "echo Test"
    }
    |> Command.execute
    |> Output.toId
    |> should not' (equal 0)

[<Test>]
[<Platform("Win")>]
let ``Text in Input with CMD`` () =
    cli {
        Shell CMD
        Input "echo 123\r\necho 345"
        WorkingDirectory @"C:\"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "C:\\>echo 123\r\n123\r\n\r\nC:\\>echo 345\r\n345\r\n\r\nC:\\>"

[<Test>]
[<Platform("Win")>]
let ``CMD returning error message`` () =
    cli {
        Shell CMD
        Command "echl Test"
    }
    |> Command.execute
    |> Output.toError
    |> should not' (equal None)

[<Test>]
[<Platform("Win")>]
let ``CMD returning error message without text`` () =
    let operation =
        cli {
            Shell CMD
            Command "echl Test"
        }
        |> Command.execute

    operation |> Output.toError |> should not' (equal None)
    operation |> Output.toText |> should equal ""

[<Test>]
[<Platform("Win")>]
let ``Hello World with PS`` () =
    cli {
        Shell PS
        Command "Write-Host Hello World!"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Hello World!"

[<Test>]
[<Platform("Win")>]
let ``Hello World with PWSH`` () =
    cli {
        Shell PWSH
        Command "Write-Host Hello World!"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Hello World!"
