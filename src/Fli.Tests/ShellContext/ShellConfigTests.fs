module Fli.Tests.ShellContext.ShellConfigTests

open NUnit.Framework
open FsUnit
open Fli


[<Test>]
let ``Check Shell config with CMD`` () =
    cli { Shell CMD } |> (fun c -> c.config.Shell) |> should equal CMD

[<Test>]
let ``Check Command config`` () =
    cli {
        Shell PS
        Command "echo test"
    }
    |> fun c -> c.config.Command
    |> should equal (Some "echo test")

[<Test>]
let ``Check Input config for CMD`` () =
    cli {
        Shell CMD
        Input "echo 123\r\necho Test"
    }
    |> fun c -> c.config.Input
    |> should equal (Some "echo 123\r\necho Test")

[<Test>]
let ``Check Output config for CMD`` () =
    cli {
        Shell CMD
        Output @"C:\Users\test.txt"
    }
    |> fun c -> c.config.Output
    |> should equal (Some @"C:\Users\test.txt")

[<Test>]
let ``Check WorkingDirectory config`` () =
    cli {
        Shell BASH
        WorkingDirectory @"C:\Users"
    }
    |> fun c -> c.config.WorkingDirectory
    |> should equal (Some @"C:\Users")

[<Test>]
let ``Check EnvironmentVariables with single KeyValue config`` () =
    cli {
        Shell BASH
        EnvironmentVariable("user", "admin")
    }
    |> fun c -> c.config.EnvironmentVariables.Value
    |> should equal [ ("user", "admin") ]

[<Test>]
let ``Check EnvironmentVariables with multiple KeyValues config`` () =
    cli {
        Shell BASH
        EnvironmentVariables [ ("user", "admin"); ("path", "path/to/file") ]
    }
    |> fun c -> c.config.EnvironmentVariables.Value
    |> should equal [ ("user", "admin"); ("path", "path/to/file") ]

[<Test>]
let ``Check Encoding with setting Encoding`` () =
    cli {
        Shell BASH
        Encoding System.Text.Encoding.UTF8
    }
    |> fun c -> c.config.Encoding.Value
    |> should equal System.Text.Encoding.UTF8
