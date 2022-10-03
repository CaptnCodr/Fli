module Fli.CliCommandConfigTests

open NUnit.Framework
open FsUnit
open Fli
open System.Net


[<Test>]
let ``Check Shell config with CMD`` () =
    cli { Shell CMD } |> (fun c -> c.config.Shell) |> should equal CMD

[<Test>]
let ``Check Command config`` () =
    cli {
        Shell PS
        Command "-Command test"
    }
    |> fun c -> c.config.Command
    |> should equal "-Command test"

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
