module Fli.Tests.ShellContext.ShellCommandConfigureUnixTests

open Fli
open NUnit.Framework
open FsUnit
open System.Collections.Generic
open System.Text


[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check FileName in ProcessStartInfo with CMD Shell`` () =
    cli { Shell BASH }
    |> Command.buildProcess
    |> (fun p -> p.FileName)
    |> should equal "bash"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check Argument in ProcessStartInfo with Command`` () =
    cli {
        Shell BASH
        Command "echo Hello World!"
    }
    |> Command.buildProcess
    |> _.Arguments
    |> should equal "-c \"echo Hello World!\""

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check WorkingDirectory in ProcessStartInfo with WorkingDirectory`` () =
    cli {
        Shell BASH
        WorkingDirectory @"/etc/"
    }
    |> Command.buildProcess
    |> _.WorkingDirectory
    |> should equal @"/etc/"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check Environment in ProcessStartInfo with single environment variable`` () =
    cli {
        Shell BASH
        EnvironmentVariable("Fli", "test")
    }
    |> Command.buildProcess
    |> _.Environment.Contains(KeyValuePair("Fli", "test"))
    |> should be True

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check Environment in ProcessStartInfo with multiple environment variables`` () =
    let config =
        cli {
            Shell BASH
            EnvironmentVariables [ ("Fli", "test"); ("Fli.Test", "test") ]
        }
        |> Command.buildProcess

    config.Environment.Contains(KeyValuePair("Fli", "test")) |> should be True
    config.Environment.Contains(KeyValuePair("Fli.Test", "test")) |> should be True

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check StandardOutputEncoding & StandardErrorEncoding with setting Encoding`` () =
    let config =
        cli {
            Shell BASH
            Encoding Encoding.UTF8
        }
        |> Command.buildProcess

    config.StandardOutputEncoding |> should equal Encoding.UTF8
    config.StandardErrorEncoding |> should equal Encoding.UTF8

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check all possible values in ProcessStartInfo`` () =
    let config =
        cli {
            Shell BASH
            Command "echo Hello World! €"
            Input "echo TestInput"
            WorkingDirectory @"C:\Users"
            EnvironmentVariable("Fli", "test")
            EnvironmentVariables [ ("Fli.Test", "test") ]
            Encoding Encoding.UTF8
        }
        |> Command.buildProcess

    config.FileName |> should equal "bash"
    config.Arguments |> should equal "-c \"echo Hello World! €\""
    config.WorkingDirectory |> should equal @"C:\Users"
    config.Environment.Contains(KeyValuePair("Fli", "test")) |> should be True
    config.Environment.Contains(KeyValuePair("Fli.Test", "test")) |> should be True
    config.StandardOutputEncoding |> should equal Encoding.UTF8
    config.StandardErrorEncoding |> should equal Encoding.UTF8
