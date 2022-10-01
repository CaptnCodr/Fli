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
let ``Check Credentials config`` () =
    cli {
        Shell CMD
        WorkingDirectory @"C:\Users"
        Credentials ("user", "password", "domain")
    }
    |> fun c -> c.config.Credentials.Value
    |> fun creds -> (creds.UserName, creds.Password, creds.Domain)
    |> should equal ("user", "password", "domain")

[<Test>]
let ``Check Credentials config with NetworkCredentials`` () =
    cli {
        Shell CMD
        WorkingDirectory @"C:\Users"
        Credentials (NetworkCredential("user", "password", "domain"))
    }
    |> fun c -> c.config.Credentials.Value
    |> fun creds -> (creds.UserName, creds.Password, creds.Domain)
    |> should equal ("user", "password", "domain")