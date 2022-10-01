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

[<Test>]
let ``Hello World with executing program with Verb`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Exec "cmd.exe"
            Verb "open"
            Arguments "/C echo Hello World!"
        }
        |> Command.execute
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
            |> should equal ("Unknown verb 'print'. Possible verbs on 'cmd.exe': open, runas, runasuser.")
    else
        Assert.Pass()
