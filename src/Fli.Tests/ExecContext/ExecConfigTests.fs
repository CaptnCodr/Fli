module Fli.ExecContext.ExecCommandConfigTests

open NUnit.Framework
open FsUnit
open Fli


[<Test>]
let ``Check executable name config for executing program`` () =
    cli { Exec "cmd.exe" } |> (fun c -> c.config.Program) |> should equal "cmd.exe"

[<Test>]
let ``Check arguments config for executing program`` () =
    cli {
        Exec "cmd.exe"
        Arguments "echo Hello World!"
    }
    |> fun c -> c.config.Arguments
    |> should equal (Some "echo Hello World!")


[<Test>]
let ``Check arguments list config for executing program`` () =
    cli {
        Exec "cmd.exe"
        Arguments [ "echo"; "Hello World!" ]
    }
    |> fun c -> c.config.Arguments
    |> should equal (Some "echo Hello World!")

[<Test>]
let ``Check working directory config for executing program`` () =
    cli {
        Exec "cmd.exe"
        WorkingDirectory @"C:\Users"
    }
    |> fun c -> c.config.WorkingDirectory
    |> should equal (Some @"C:\Users")

[<Test>]
let ``Check Verb config for executing program`` () =
    cli {
        Exec "cmd.exe"
        Verb "runas"
    }
    |> fun c -> c.config.Verb
    |> should equal (Some "runas")

[<Test>]
let ``Check credentials config for executing program`` () =
    cli {
        Exec "cmd.exe"
        WorkingDirectory @"C:\Users"
        Username "admin"
    }
    |> fun c -> c.config.UserName.Value
    |> should equal "admin"

[<Test>]
let ``Check credentials config for executing program with NetworkCredentials`` () =
    cli {
        Exec "cmd.exe"
        WorkingDirectory @"C:\Users"
        Username "admin@company"
    }
    |> fun c -> c.config.UserName.Value
    |> should equal "admin@company"

[<Test>]
let ``Check EnvironmentVariables with single KeyValue config`` () =
    cli {
        Exec "cmd.exe"
        EnvironmentVariable("user", "admin")
    }
    |> fun c -> c.config.EnvironmentVariables.Value
    |> should equal [ ("user", "admin") ]

[<Test>]
let ``Check EnvironmentVariables with multiple KeyValues config`` () =
    cli {
        Exec "cmd.exe"
        EnvironmentVariables [ ("user", "admin"); ("path", "path/to/file") ]
    }
    |> fun c -> c.config.EnvironmentVariables.Value
    |> should equal [ ("user", "admin"); ("path", "path/to/file") ]

[<Test>]
let ``Check Credentials with domain, username and password`` () =
    cli {
        Exec "cmd.exe"
        Credentials("domain", "user", "password123")
    }
    |> fun c -> c.config.Credentials.Value
    |> should equal (Credentials("domain", "user", "password123"))

[<Test>]
let ``Check Encoding with setting Encoding`` () =
    cli {
        Exec "cmd.exe"
        Encoding System.Text.Encoding.UTF8
    }
    |> fun c -> c.config.Encoding.Value
    |> should equal System.Text.Encoding.UTF8
