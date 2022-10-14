module Fli.Tests.ExecContext.ExecCommandExecuteTests

open NUnit.Framework
open FsUnit
open Fli
open System


[<Test>]
let ``Hello World with executing program`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Exec "cmd.exe"
            Arguments "/C echo Hello World!"
        }
        |> Command.execute
        |> Output.toText
        |> should equal "Hello World!\r\n"
    else
        Assert.Pass()

[<Test>]
let ``Test with process Id`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Exec "cmd.exe"
            Arguments "/C echo Test"
        }
        |> Command.execute
        |> Output.toId
        |> should not' (equal 0)
    else
        Assert.Pass()

[<Test>]
let ``print text with Input with executing program`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Exec "cmd.exe"
            Arguments "/k echo Test"
            Input "echo Hello World!"
            WorkingDirectory @"C:\"
        }
        |> Command.execute
        |> Output.toText
        |> should equal "Test\r\n\r\nC:\\>echo Hello World!\r\nHello World!\r\n\r\nC:\\>"
    else
        Assert.Pass()

[<Test>]
let ``Hello World with executing program async`` () =
    if OperatingSystem.IsWindows() then
        async {
            let! output =
                cli {
                    Exec "cmd.exe"
                    Arguments "/C echo Hello World!"
                }
                |> Command.executeAsync

            output |> Output.toText |> should equal "Hello World!\r\n"
        }
        |> Async.Start
    else
        Assert.Pass()

[<Test>]
let ``Hello World with executing program with Verb`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Exec "cmd.exe"
            Verb "open"
            Arguments "/C echo Hello World!"
        }
        |> Command.execute
        |> Output.toText
        |> should equal "Hello World!\r\n"
    else
        Assert.Pass()

[<Test>]
let ``Hello World with executing program throws exception with unknown Verb`` () =
    if OperatingSystem.IsWindows() then
        try
            cli {
                Exec "cmd.exe"
                Verb "print"
            }
            |> Command.execute
            |> ignore
        with :? ArgumentException as ex ->
            ex.Message
            |> should equal ("Unknown verb 'print'. Possible verbs on 'cmd.exe': open, runas, runasuser")
    else
        Assert.Pass()
