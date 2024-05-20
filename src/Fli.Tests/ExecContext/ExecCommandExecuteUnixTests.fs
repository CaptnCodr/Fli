module Fli.Tests.ExecContext.ExecCommandExecuteLinuxTests

open NUnit.Framework
open FsUnit
open Fli
open System
open System.Text

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Hello World with executing program`` () =
    cli {
        Exec "bash"
        Arguments "-c \"echo Hello World!\""
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Hello World!"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Get process Id`` () =
    cli {
        Exec "bash"
        Arguments "-c \"echo Test\""
    }
    |> Command.execute
    |> Output.toId
    |> should not' (equal 0)

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``print text with Input with executing program`` () =
    cli {
        Exec "bash"
        Arguments "-c \"echo Test\""
        Input "echo Hello World!"
        WorkingDirectory @"/etc/"
    }
    |> Command.execute
    |> Output.toText
    |> should equal "Test"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Get output in StringBuilder`` () =
    let sb = StringBuilder()

    cli {
        Exec "bash"
        Arguments "-c \"echo Test\""
        Output sb
    }
    |> Command.execute
    |> ignore

    sb.ToString() |> should equal "Test\n"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Call custom function in output`` () =
    let testFunc (test: string) (s: string) = s |> should equal test

    cli {
        Exec "bash"
        Arguments "-c \"echo Test\""
        Output(testFunc "Test\n")
    }
    |> Command.execute
    |> ignore

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Hello World with executing program async`` () =
    async {
        let! output =
            cli {
                Exec "bash"
                Arguments "-c \"echo Hello World!\""
            }
            |> Command.executeAsync

        output |> Output.toText |> should equal "Hello World!"
    }