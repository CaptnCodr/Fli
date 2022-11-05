module Fli.Tests.ExecContext.ExecCommandConfigureTests

open NUnit.Framework
open FsUnit
open Fli
open System
open System.Collections.Generic
open System.Text

[<Test>]
let ``Check FileName in ProcessStartInfo Exec program`` () =
    cli { Exec "cmd.exe" }
    |> Command.buildProcess
    |> (fun p -> p.FileName)
    |> should equal "cmd.exe"

[<Test>]
let ``Check Arguments in ProcessStartInfo with Arguments`` () =
    cli {
        Exec "cmd.exe"
        Arguments "-c echo Hello World!"
    }
    |> Command.buildProcess
    |> (fun p -> p.Arguments)
    |> should equal "-c echo Hello World!"

[<Test>]
let ``Check WorkingDirectory in ProcessStartInfo with WorkingDirectory`` () =
    cli {
        Exec "cnd.exe"
        WorkingDirectory @"C:\Users"
    }
    |> Command.buildProcess
    |> (fun p -> p.WorkingDirectory)
    |> should equal @"C:\Users"

[<Test>]
let ``Check Verb in ProcessStartInfo with Verb`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Exec "cmd.exe"
            Verb "open"
        }
        |> Command.buildProcess
        |> (fun p -> p.Verb)
        |> should equal "open"
    else
        Assert.Pass()

[<Test>]
let ``Check UserName in ProcessStartInfo with Username`` () =
    cli {
        Exec "cmd.exe"
        Username "admin"
    }
    |> Command.buildProcess
    |> (fun p -> p.UserName)
    |> should equal "admin"

[<Test>]
let ``Check Domain, UserName, Password in ProcessStartInfo with Credentials for windows (overwrites Username)`` () =
    if OperatingSystem.IsWindows() then
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
let ``Check Environment in ProcessStartInfo with single environment variable`` () =
    cli {
        Exec "cmd.exe"
        EnvironmentVariable("Fli", "test")
    }
    |> Command.buildProcess
    |> (fun p -> p.Environment.Contains(KeyValuePair("Fli", "test")))
    |> should be True

[<Test>]
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
let ``Check all possible values in ProcessStartInfo for windows`` () =
    if OperatingSystem.IsWindows() then
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
    else
        let config =
            cli {
                Exec "bash"
                Arguments "--help"
                Output "./Users/test.txt"
                WorkingDirectory "./Users"
                Username "admin"
                EnvironmentVariable("Fli", "test")
                EnvironmentVariables [ ("Fli.Test", "test") ]
                Encoding Encoding.UTF8
            }
            |> Command.buildProcess

        config.FileName |> should equal "bash"
        config.Arguments |> should equal "--help"
        config.WorkingDirectory |> should equal "./Users"
        config.Verb |> should equal String.Empty
        config.UserName |> should equal "admin"
        config.Environment.Contains(KeyValuePair("Fli", "test")) |> should be True
        config.Environment.Contains(KeyValuePair("Fli.Test", "test")) |> should be True
        config.StandardOutputEncoding |> should equal Encoding.UTF8
        config.StandardErrorEncoding |> should equal Encoding.UTF8
