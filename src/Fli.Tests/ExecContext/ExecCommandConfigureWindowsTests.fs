module Fli.Tests.ExecContext.ExecCommandConfigureWindowsTests

open NUnit.Framework
open FsUnit
open Fli
open System
open System.Collections.Generic
open System.Text

[<Test>]
[<Platform("Win")>]
let ``Check FileName in ProcessStartInfo Exec program`` () =
    cli { Exec "cmd.exe" }
    |> Command.buildProcess
    |> (fun p -> p.FileName)
    |> should equal "cmd.exe"

[<Test>]
[<Platform("Win")>]
let ``Check Arguments in ProcessStartInfo with Arguments`` () =
    cli {
        Exec "cmd.exe"
        Arguments "-c echo Hello World!"
    }
    |> Command.buildProcess
    |> _.Arguments
    |> should equal "-c echo Hello World!"

[<Test>]
[<Platform("Win")>]
let ``Check WorkingDirectory in ProcessStartInfo with WorkingDirectory`` () =
    cli {
        Exec "cmd.exe"
        WorkingDirectory @"C:\Users"
    }
    |> Command.buildProcess
    |> _.WorkingDirectory
    |> should equal @"C:\Users"

[<Test>]
[<Platform("Win")>]
let ``Check WindowStyle in ProcessStartInfo with WorkingDirectory`` () =
    cli {
        Exec "cmd.exe"
        WindowStyle Normal
    }
    |> Command.buildProcess
    |> _.WindowStyle
    |> should equal Diagnostics.ProcessWindowStyle.Normal

[<Test>]
[<Platform("Win")>]
let ``Check Verb in ProcessStartInfo with Verb`` () =
    cli {
        Exec "cmd.exe"
        Verb "open"
    }
    |> Command.buildProcess
    |> _.Verb
    |> should equal "open"

[<Test>]
[<Platform("Win")>]
let ``Check UserName in ProcessStartInfo with Username`` () =
    cli {
        Exec "cmd.exe"
        Username "admin"
    }
    |> Command.buildProcess
    |> _.UserName
    |> should equal "admin"

[<Test>]
[<Platform("Win")>]
let ``Check Domain, UserName, Password in ProcessStartInfo with Credentials for windows (overwrites Username)`` () =
    let config =
        cli {
            Exec "cmd.exe"
            Username "admin"
            Credentials("domain", "user", "password")
        }
        |> Command.buildProcess

    config.Domain |> should equal "domain"
    config.UserName |> should equal "user"
    config.Password |> should not' (equal "password") // stored as SecureString

[<Test>]
[<Platform("Win")>]
let ``Check Environment in ProcessStartInfo with single environment variable`` () =
    cli {
        Exec "cmd.exe"
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
            Exec "cmd.exe"
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
            Exec "cmd.exe"
            Encoding Encoding.UTF8
        }
        |> Command.buildProcess

    config.StandardOutputEncoding |> should equal Encoding.UTF8
    config.StandardErrorEncoding |> should equal Encoding.UTF8

[<Test>]
[<Platform("Win")>]
let ``Check all possible values in ProcessStartInfo for windows`` () =
    let config =
        cli {
            Exec "cmd.exe"
            Arguments "--help"
            Input "Test"
            Output @"C:\Users\test.txt"
            WorkingDirectory @"C:\Users"
            Verb "open"
            Username "admin"
            Credentials("domain", "admin", "password")
            EnvironmentVariable("Fli", "test")
            EnvironmentVariables [ ("Fli.Test", "test") ]
            Encoding Encoding.UTF8
            WindowStyle Normal
        }
        |> Command.buildProcess

    config.FileName |> should equal "cmd.exe"
    config.Arguments |> should equal "--help"
    config.WorkingDirectory |> should equal @"C:\Users"
    config.Verb |> should equal "open"
    config.Domain |> should equal "domain"
    config.UserName |> should equal "admin"
    config.Password |> should not' (equal "password")
    config.Environment.Contains(KeyValuePair("Fli", "test")) |> should be True
    config.Environment.Contains(KeyValuePair("Fli.Test", "test")) |> should be True
    config.StandardOutputEncoding |> should equal Encoding.UTF8
    config.StandardErrorEncoding |> should equal Encoding.UTF8
    config.WindowStyle |> should equal Diagnostics.ProcessWindowStyle.Normal  