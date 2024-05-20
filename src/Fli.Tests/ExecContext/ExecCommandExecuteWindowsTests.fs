module Fli.Tests.ExecContext.ExecCommandExecuteWindowsTests

open NUnit.Framework
open FsUnit
open Fli
open System
open System.Text


[<Test>]
[<Platform("Win")>]
let ``Hello World with executing program`` () =
    cli {
        Exec "cmd.exe"
        Arguments "/C echo Hello World!"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Hello World!"

[<Test>]
[<Platform("Win")>]
let ``Get process Id`` () =
    cli {
        Exec "cmd.exe"
        Arguments "/C echo Test"
    }
    |> Command.execute
    |> Output.toId
    |> should not' (equal 0)

[<Test>]
[<Platform("Win")>]
let ``print text with Input with executing program`` () =
    cli {
        Exec "cmd.exe"
        Arguments "/k echo Test"
        Input "echo Hello World!"
        WorkingDirectory @"C:\"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Test\r\n\r\nC:\\>echo Hello World!\r\nHello World!\r\n\r\nC:\\>"

[<Test>]
[<Platform("Win")>]
let ``Get output in StringBuilder`` () =
    let sb = StringBuilder()

    cli {
        Exec "cmd.exe"
        Arguments "/c echo Test"
        Output sb
    }
    |> Command.execute
    |> ignore

    sb.ToString() |> should equal "Test\r\n"

[<Test>]
[<Platform("Win")>]
let ``Call custom function in output`` () =
    let testFunc (test: string) (s: string) = s |> should equal test

    cli {
        Exec "cmd.exe"
        Arguments "/c echo Test"
        Output(testFunc "Test\r\n")
    }
    |> Command.execute
    |> ignore

[<Test>]
[<Platform("Win")>]
let ``Hello World with executing program async`` () =
    async {
        let! output =
            cli {
                Exec "cmd.exe"
                Arguments "/C echo Hello World!"
            }
            |> Command.executeAsync

        output |> Output.toText |> should equal "Hello World!"
    }


[<Test>]
[<Platform("Win")>]
let ``Hello World with executing program with Verb`` () =
    cli {
        Exec "cmd.exe"
        Verb "open"
        Arguments "/C echo Hello World!"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Hello World!"


[<Test>]
[<Platform("Win")>]
let ``Hello World with executing program throws exception with unknown Verb`` () =
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
