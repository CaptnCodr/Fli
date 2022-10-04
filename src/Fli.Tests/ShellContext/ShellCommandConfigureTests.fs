module Fli.ShellContext.ShellCommandConfigureTests

open Fli
open NUnit.Framework
open FsUnit
open System.Collections.Generic


[<Test>]
let ``Check FileName in ProcessStartInfo with CMD Shell`` () =
    cli { Shell CMD }
    |> Command.configureProcess
    |> (fun p -> p.FileName)
    |> should equal "cmd.exe"

[<Test>]
let ``Check Argument in ProcessStartInfo with Command`` () =
    cli {
        Shell PS
        Command "echo Hello World!"
    }
    |> Command.configureProcess
    |> (fun p -> p.Arguments)
    |> should equal "-Command echo Hello World!"

[<Test>]
let ``Check WorkingDirectory in ProcessStartInfo with WorkingDirectory`` () =
    cli {
        Shell CMD
        WorkingDirectory @"C:\Users"
    }
    |> Command.configureProcess
    |> (fun p -> p.WorkingDirectory)
    |> should equal @"C:\Users"

[<Test>]
let ``Check Environment in ProcessStartInfo with single environment variable`` () =
    cli {
        Shell CMD
        EnvironmentVariable("Fli", "test")
    }
    |> Command.configureProcess
    |> (fun p -> p.Environment.Contains(KeyValuePair("Fli", "test")))
    |> should be True

[<Test>]
let ``Check Environment in ProcessStartInfo with multiple environment variables`` () =
    let config =
        cli {
            Shell CMD
            EnvironmentVariables [ ("Fli", "test"); ("Fli.Test", "test") ]
        }
        |> Command.configureProcess

    config.Environment.Contains(KeyValuePair("Fli", "test")) |> should be True
    config.Environment.Contains(KeyValuePair("Fli.Test", "test")) |> should be True

[<Test>]
let ``Check all possible values in ProcessStartInfo`` () =
    let config =
        cli {
            Shell BASH
            Command "echo Hello World!"
            WorkingDirectory @"C:\Users"
            EnvironmentVariable("Fli", "test")
            EnvironmentVariables [ ("Fli.Test", "test") ]
        }
        |> Command.configureProcess

    config.FileName |> should equal "bash"
    config.Arguments |> should equal "-c echo Hello World!"
    config.WorkingDirectory |> should equal @"C:\Users"
    config.Environment.Contains(KeyValuePair("Fli", "test")) |> should be True
    config.Environment.Contains(KeyValuePair("Fli.Test", "test")) |> should be True
