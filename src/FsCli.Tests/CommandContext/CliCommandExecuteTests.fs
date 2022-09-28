module FsCli.CliCommandExecuteTests

open NUnit.Framework
open FsUnit
open FsCli
open System


[<Test>]
let ``Hello World with CMD`` () =
    if OperatingSystem.IsWindows() then
        cli {
            CLI CMD
            Command "echo Hello World!"
        }
        |> Command.execute
        |> should equal "Hello World!\r\n"
    else
        Assert.Pass()

[<Test>]
let ``Hello World with PWSH`` () =
    if OperatingSystem.IsWindows() then
        cli {
            CLI PWSH
            Command "Write-Host Hello World!"
        }
        |> Command.execute
        |> should equal "Hello World!\r\n"
    else
        Assert.Pass()

[<Test>]
let ``Hello World with Bash`` () =
    if OperatingSystem.IsWindows() |> not then
        cli {
            CLI Bash
            Command "\"echo Hello World!\""
        }
        |> Command.execute
        |> should equal "Hello World!\n"
    else
        Assert.Pass()
