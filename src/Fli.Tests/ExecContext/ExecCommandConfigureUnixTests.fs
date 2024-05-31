module Fli.Tests.ExecContext.ExecCommandConfigureUnixTests

open NUnit.Framework
open FsUnit
open Fli
open System
open System.Collections.Generic
open System.Text

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check FileName in ProcessStartInfo Exec program`` () =
    cli { Exec "bash" }
    |> Command.buildProcess
    |> _.FileName
    |> should equal "bash"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check Arguments in ProcessStartInfo with Arguments`` () =
    cli {
        Exec "bash"
        Arguments "-c echo Hello World!"
    }
    |> Command.buildProcess
    |> _.Arguments
    |> should equal "-c echo Hello World!"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check WorkingDirectory in ProcessStartInfo with WorkingDirectory`` () =
    cli {
        Exec "bash"
        WorkingDirectory @"/etc/"
    }
    |> Command.buildProcess
    |> _.WorkingDirectory
    |> should equal @"/etc/"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check WindowStyle in ProcessStartInfo with WorkingDirectory`` () =
    cli {
        Exec "bash"
        WindowStyle Normal
    }
    |> Command.buildProcess
    |> _.WindowStyle
    |> should equal Diagnostics.ProcessWindowStyle.Normal

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check UserName in ProcessStartInfo with Username`` () =
    cli {
        Exec "bash"
        Username "root"
    }
    |> Command.buildProcess
    |> _.UserName
    |> should equal "root"

[<Test>]
[<Platform("Linux,Unix,MacOsX")>]
let ``Check Environment in ProcessStartInfo with single environment variable`` () =
    cli {
        Exec "bash"
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
            Exec "bash"
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
            Exec "bash"
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