module FsCli.ProgramCommandExecuteTests

open NUnit.Framework
open FsUnit
open FsCli
open System.Runtime.Versioning


[<Test>]
[<SupportedOSPlatform("windows")>]
let ``Hello World with executing program`` () =
    cli {
        Exec "cmd.exe"
        Arguments "/C echo Hello World!"
    }
    |> Command.execute
    |> should equal "Hello World!\r\n"
