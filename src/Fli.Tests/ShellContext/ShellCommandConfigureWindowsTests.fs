module Fli.Tests.ShellContext.ShellCommandConfigureWindowsTests

open Fli
open NUnit.Framework
open FsUnit
open System.Collections.Generic
open System.Text


[<Test>]
[<Platform("Win")>]
let ``Check FileName in ProcessStartInfo with CMD Shell`` () =
    cli { Shell CMD }
    |> Command.buildProcess
    |> _.FileName
    |> should equal "cmd.exe"

[<Test>]
[<Platform("Win")>]
let ``Check Argument in ProcessStartInfo with Command`` () =
    cli {
        Shell PS
        Command "echo Hello World!"
    }
    |> Command.buildProcess
    |> _.Arguments
    |> should equal "-Command echo Hello World!"

[<Test>]
[<Platform("Win")>]
let ``Check WorkingDirectory in ProcessStartInfo with WorkingDirectory`` () =
    cli {
        Shell CMD
        WorkingDirectory @"C:\Users"
    }
    |> Command.buildProcess
    |> _.WorkingDirectory
    |> should equal @"C:\Users"

[<Test>]
[<Platform("Win")>]
let ``Check Environment in ProcessStartInfo with single environment variable`` () =
    cli {
        Shell CMD
        EnvironmentVariable("Fli", "test")
    }
    |> Command.buildProcess
    |> _.Environment.Contains(KeyValuePair("Fli", "test"))
    |> should be True

[<Test>]
[<Platform("Win")>]
let ``Check Environment in ProcessStartInfo with multiple environment variables`` () =
    let config =
        cli {
            Shell CMD
            EnvironmentVariables [ ("Fli", "test"); ("Fli.Test", "test") ]
        }
        |> Command.buildProcess

    config.Environment.Contains(KeyValuePair("Fli", "test")) |> should be True
    config.Environment.Contains(KeyValuePair("Fli.Test", "test")) |> should be True

[<Test>]
[<Platform("Win")>]
let ``Check StandardOutputEncoding & StandardErrorEncoding with setting Encoding`` () =
    let config =
        cli {
            Shell CMD
            Encoding Encoding.UTF8
        }
        |> Command.buildProcess

    config.StandardOutputEncoding |> should equal Encoding.UTF8
    config.StandardErrorEncoding |> should equal Encoding.UTF8

[<Test>]
[<Platform("Win")>]
let ``Check all possible values in ProcessStartInfo`` () =
    let config =
        cli {
            Shell CMD
            Command "echo Hello World! €"
            Input "echo TestInput"
            WorkingDirectory @"C:\Users"
            EnvironmentVariable("Fli", "test")
            EnvironmentVariables [ ("Fli.Test", "test") ]
            Encoding Encoding.UTF8
        }
        |> Command.buildProcess

    config.FileName |> should equal "cmd.exe"
    config.Arguments |> should equal "/k echo Hello World! €"
    config.WorkingDirectory |> should equal @"C:\Users"
    config.Environment.Contains(KeyValuePair("Fli", "test")) |> should be True
    config.Environment.Contains(KeyValuePair("Fli.Test", "test")) |> should be True
    config.StandardOutputEncoding |> should equal Encoding.UTF8
    config.StandardErrorEncoding |> should equal Encoding.UTF8
