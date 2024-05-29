module Fli.Tests.ShellContext.ShellConfigTests

open NUnit.Framework
open FsUnit
open Fli


[<Test>]
let ``Check Shell config with CMD`` () =
    cli { Shell CMD } |> _.config.Shell |> should equal CMD

[<Test>]
let ``Check Command config`` () =
    cli {
        Shell PS
        Command "echo test"
    }
    |> _.config.Command
    |> should equal (Some "echo test")

[<Test>]
let ``Check Input config for CMD`` () =
    cli {
        Shell CMD
        Input "echo 123\r\necho Test"
    }
    |> _.config.Input
    |> should equal (Some "echo 123\r\necho Test")

[<Test>]
let ``Check Output config for CMD`` () =
    cli {
        Shell CMD
        Output @"C:\Users\test.txt"
    }
    |> _.config.Output
    |> should equal (Some(File @"C:\Users\test.txt"))

[<Test>]
let ``Empty string in Output ends up as None`` () =
    cli {
        Shell CMD
        Output ""
    }
    |> _.config.Output |> should be (ofCase <@ None @>)

[<Test>]
let ``Nullable file path in Output ends up as None`` () =
    cli {
        Shell CMD
        Output (File(null))
    }
    |> _.config.Output |> should be (ofCase <@ None @>)

[<Test>]
let ``Check WorkingDirectory config`` () =
    cli {
        Shell BASH
        WorkingDirectory @"C:\Users"
    }
    |> _.config.WorkingDirectory
    |> should equal (Some @"C:\Users")

[<Test>]
let ``Check EnvironmentVariables with single KeyValue config`` () =
    cli {
        Shell BASH
        EnvironmentVariable("user", "admin")
    }
    |> _.config.EnvironmentVariables.Value
    |> should equal [ ("user", "admin") ]

[<Test>]
let ``Check EnvironmentVariables with multiple KeyValues config`` () =
    cli {
        Shell BASH
        EnvironmentVariables [ ("user", "admin"); ("path", "path/to/file") ]
    }
    |> _.config.EnvironmentVariables.Value
    |> should equal [ ("user", "admin"); ("path", "path/to/file") ]

[<Test>]
let ``Check Encoding with setting Encoding`` () =
    cli {
        Shell BASH
        Encoding System.Text.Encoding.UTF8
    }
    |> _.config.Encoding.Value
    |> should equal System.Text.Encoding.UTF8
