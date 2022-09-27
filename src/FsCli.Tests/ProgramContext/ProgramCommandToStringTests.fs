module FsCli.ProgramCommandToStringTests

open NUnit.Framework
open FsUnit
open FsCli


[<Test>]
let ``Hello World with executing program`` () =
    cli {
        Exec "cmd"
        Arguments "/C echo Hello World!"
    }
    |> Command.toString
    |> should equal "cmd /C echo Hello World!"

[<Test>]
let ``Hello World with an argument list`` () =
    cli {
        Exec "cmd"
        Arguments [ "/C"; "echo"; "Hello World!" ]
    }
    |> Command.toString
    |> should equal "cmd /C echo Hello World!"

[<Test>]
let ``Hello World with an argument array`` () =
    cli {
        Exec "cmd"
        Arguments [| "/C"; "echo"; "Hello World!" |]
    }
    |> Command.toString
    |> should equal "cmd /C echo Hello World!"

[<Test>]
let ``Hello World with an argument seq`` () =
    cli {
        Exec "cmd"

        Arguments(
            seq {
                "/C"
                "echo"
                "Hello World!"
            }
        )
    }
    |> Command.toString
    |> should equal "cmd /C echo Hello World!"
