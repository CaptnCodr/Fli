module Fli.ProgramCommandExecuteTests

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
        |> should equal "Hello World!\r\n"
    else
        Assert.Pass()