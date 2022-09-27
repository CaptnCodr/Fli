module FsCli.ProgramCommandExecuteTests

open NUnit.Framework
open FsUnit
open FsCli


[<Test>]
let ``Hello World with executing program`` () =
    cli {
        Exec "cmd"
        Arguments "/C echo Hello World!"
    }
    |> Command.execute
    |> should equal "Hello World!\r\n"
