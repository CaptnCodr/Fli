module Fli.ExecContext.ExecCommandToStringTests

open NUnit.Framework
open FsUnit
open Fli


[<Test>]
let ``Hello World with executing program`` () =
    cli {
        Exec "cmd.exe"
        Arguments "/C echo Hello World!"
    }
    |> Command.toString
    |> should equal "cmd.exe /C echo Hello World!"

[<Test>]
let ``Hello World with an argument list`` () =
    cli {
        Exec "cmd.exe"
        Arguments [ "/C"; "echo"; "Hello World!" ]
    }
    |> Command.toString
    |> should equal "cmd.exe /C echo Hello World!"

[<Test>]
let ``Hello World with an argument array`` () =
    cli {
        Exec "cmd.exe"
        Arguments [| "/C"; "echo"; "Hello World!" |]
    }
    |> Command.toString
    |> should equal "cmd.exe /C echo Hello World!"

[<Test>]
let ``Hello World with an argument seq`` () =
    cli {
        Exec "cmd.exe"

        Arguments(
            seq {
                "/C"
                "echo"
                "Hello World!"
            }
        )
    }
    |> Command.toString
    |> should equal "cmd.exe /C echo Hello World!"
