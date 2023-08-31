module Fli.Tests.ShellContext.ShellCommandExecuteTests

open NUnit.Framework
open FsUnit
open Fli
open System
open System.Text
open System.Runtime.CompilerServices
open System.Threading
open System.Diagnostics


[<Test>]
let ``Hello World with CMD`` () =
    if OperatingSystem.IsWindows() then
        let operation =
            cli {
                Shell CMD
                Command "echo Hello World!"
            }
            |> Command.execute

        operation |> Output.toText |> should equal "Hello World!"
        operation |> Output.toError |> should equal ""
    else
        Assert.Pass()

[<Test>]
let ``Hello World with CMD waiting async`` () =
    async {
        if OperatingSystem.IsWindows() then
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
                    |> Async.StartAsTask
                    |> Async.AwaitTask

                ()
            with :? AggregateException as e ->
                e.GetType() |> should equal typeof<AggregateException>

            stopwatch.Stop()
            stopwatch.Elapsed.TotalSeconds |> should be (inRange 2.9 3.2)
        else
            Assert.Pass()
    }

[<Test>]
let ``Hello World with CUSTOM shell`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Shell(CUSTOM("cmd.exe", "/c"))
            Command "echo Hello World!"
        }
        |> Command.execute
        |> Output.toText
        |> should equal "Hello World!"
    else
        cli {
            Shell(CUSTOM("bash", "-c"))
            Command "\"echo Hello World!\""
        }
        |> Command.execute
        |> Output.toText
        |> should equal "Hello World!"

[<Test>]
let ``CMD returning non zero ExitCode`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Shell CMD
            Command "echl Test"
        }
        |> Command.execute
        |> Output.toExitCode
        |> should equal 1
    else
        Assert.Pass()

[<Test>]
let ``Get output in StringBuilder`` () =
    let sb = StringBuilder()

    if OperatingSystem.IsWindows() then
        cli {
            Shell CMD
            Command "echo Test"
            Output sb
        }
        |> Command.execute
        |> ignore

        sb.ToString() |> should equal "Test\r\n"
    else
        cli {
            Shell BASH
            Command "echo Test"
            Output sb
        }
        |> Command.execute
        |> ignore

        sb.ToString() |> should equal "Test\n"

[<Test>]
let ``CMD returning non zero process id`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Shell CMD
            Command "echo Test"
        }
        |> Command.execute
        |> Output.toId
        |> should not' (equal 0)
    else
        Assert.Pass()

[<Test>]
let ``Text in Input with CMD`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Shell CMD
            Input "echo 123\r\necho 345"
            WorkingDirectory @"C:\"
        }
        |> Command.execute
        |> Output.toText
        |> should equal "C:\\>echo 123\r\n123\r\n\r\nC:\\>echo 345\r\n345\r\n\r\nC:\\>"
    else
        Assert.Pass()

[<Test>]
let ``CMD returning error message`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Shell CMD
            Command "echl Test"
        }
        |> Command.execute
        |> Output.toError
        |> should not' (equal None)
    else
        Assert.Pass()

[<Test>]
let ``CMD returning error message without text`` () =
    if OperatingSystem.IsWindows() then
        let operation =
            cli {
                Shell CMD
                Command "echl Test"
            }
            |> Command.execute

        operation |> Output.toError |> should not' (equal None)
        operation |> Output.toText |> should equal ""
    else
        Assert.Pass()

[<Test>]
let ``Hello World with PS`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Shell PS
            Command "Write-Host Hello World!"
        }
        |> Command.execute
        |> Output.toText
        |> should equal "Hello World!"
    else
        Assert.Pass()

[<Test>]
let ``Hello World with PWSH`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Shell PWSH
            Command "Write-Host Hello World!"
        }
        |> Command.execute
        |> Output.toText
        |> should equal "Hello World!"
    else
        Assert.Pass()

[<Test>]
let ``Hello World with BASH`` () =
    if OperatingSystem.IsWindows() |> not then
        cli {
            Shell BASH
            Command "echo Hello World!"
        }
        |> Command.execute
        |> Output.toText
        |> should equal "Hello World!"
    else
        Assert.Pass()

[<Test>]
let ``Input text in BASH`` () =
    if OperatingSystem.IsWindows() |> not then
        cli {
            Shell BASH
            Command "echo Hello World!"
            Input "echo Test"
        }
        |> Command.execute
        |> Output.toText
        |> should equal "Hello World!"
    else
        Assert.Pass()

[<Test>]
let ``Hello World with BASH async`` () =
    if OperatingSystem.IsWindows() |> not then
        async {
            let! output =
                cli {
                    Shell BASH
                    Command "echo Hello World!"
                }
                |> Command.executeAsync

            output |> Output.toText |> should equal "Hello World!"
        }
    else
        Assert.Pass()
        |> async.Return 

[<Test>]
let ``BASH returning non zero ExitCode`` () =
    if OperatingSystem.IsWindows() |> not then
        cli {
            Shell BASH
            Command "echl Test"
        }
        |> Command.execute
        |> Output.toExitCode
        |> should equal 127
    else
        Assert.Pass()

[<Test>]
let ``BASH returning non zero process id`` () =
    if OperatingSystem.IsWindows() |> not then
        cli {
            Shell BASH
            Command "echo Test"
        }
        |> Command.execute
        |> Output.toId
        |> should not' (equal 0)
    else
        Assert.Pass()
