module Fli.ProgramCommandConfigTests

open NUnit.Framework
open FsUnit
open Fli
open System
open System.Net


[<Test>]
let ``Check executable name config for executing program`` () =
    if OperatingSystem.IsWindows() then
        cli { Exec "cmd.exe" } |> (fun c -> c.config.Program) |> should equal "cmd.exe"
    else
        Assert.Pass()

[<Test>]
let ``Check arguments config for executing program`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Exec "cmd.exe"
            Arguments "echo Hello World!"
        }
        |> fun c -> c.config.Arguments
        |> should equal "echo Hello World!"
    else
        Assert.Pass()


[<Test>]
let ``Check arguments list config for executing program`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Exec "cmd.exe"
            Arguments [ "echo"; "Hello World!" ]
        }
        |> fun c -> c.config.Arguments
        |> should equal "echo Hello World!"
    else
        Assert.Pass()

[<Test>]
let ``Check working directory config for executing program`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Exec "cmd.exe"
            WorkingDirectory @"C:\Users"
        }
        |> fun c -> c.config.WorkingDirectory
        |> should equal (Some @"C:\Users")
    else
        Assert.Pass()

[<Test>]
let ``Check credentials config for executing program`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Exec "cmd.exe"
            WorkingDirectory @"C:\Users"
            Credentials ("user", "password", "domain")
        }
        |> fun c -> c.config.Credentials.Value
        |> fun creds -> (creds.UserName, creds.Password, creds.Domain)
        |> should equal ("user", "password", "domain")
    else
        Assert.Pass()

[<Test>]
let ``Check credentials config for executing program with NetworkCredentials`` () =
    if OperatingSystem.IsWindows() then
        cli {
            Exec "cmd.exe"
            WorkingDirectory @"C:\Users"
            Credentials (NetworkCredential("user", "password", "domain"))
        }
        |> fun c -> c.config.Credentials.Value
        |> fun creds -> (creds.UserName, creds.Password, creds.Domain)
        |> should equal ("user", "password", "domain")
    else
        Assert.Pass()
