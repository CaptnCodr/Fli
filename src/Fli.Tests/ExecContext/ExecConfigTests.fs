module Fli.Tests.ExecContext.ExecCommandConfigTests

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
    |> _.config.Arguments
    |> should equal (Some(Arguments(Some "echo Hello World!")))

[<Test>]
let ``Check arguments list config for executing program`` () =
    cli {
        Exec "cmd.exe"
        Arguments [ "echo"; "Hello World!" ]
    }
    |> _.config.Arguments
    |> should equal (Some(ArgumentList(Some [| "echo"; "Hello World!" |])))

[<Test>]
let ``Check Input config for executing program`` () =
    cli {
        Exec "cmd.exe"
        Input "echo 123\r\necho Test"
    }
    |> _.config.Input
    |> should equal (Some "echo 123\r\necho Test")

[<Test>]
let ``Check Output config for executing program`` () =
    cli {
        Exec "cmd.exe"
        Output @"C:\Users\test.txt"
    }
    |> _.config.Output
    |> should equal (Some(File @"C:\Users\test.txt"))

[<Test>]
let ``Empty string in Output ends up as None`` () =
    cli {
        Exec "cmd.exe"
        Output ""
    }
    |> _.config.Output
    |> should be (ofCase <@ None @>)

[<Test>]
let ``Nullable file path in Output ends up as None`` () =
    cli {
        Exec "cmd.exe"
        Output(File(null))
    }
    |> _.config.Output
    |> should be (ofCase <@ None @>)

[<Test>]
let ``Check working directory config for executing program`` () =
    cli {
        Exec "cmd.exe"
        WorkingDirectory @"C:\Users"
    }
    |> _.config.WorkingDirectory
    |> should equal (Some @"C:\Users")

[<Test>]
let ``Check WindowStyle config for executing program`` () =
    cli {
        Exec "cmd.exe"
        WindowStyle Normal
    }
    |> _.config.WindowStyle
    |> should equal (Some Normal)

[<Test>]
let ``Check Verb config for executing program`` () =
    cli {
        Exec "cmd.exe"
        Verb "runas"
    }
    |> _.config.Verb
    |> should equal (Some "runas")

[<Test>]
let ``Check credentials config for executing program`` () =
    cli {
        Exec "cmd.exe"
        WorkingDirectory @"C:\Users"
        Username "admin"
    }
    |> _.config.UserName.Value
    |> should equal "admin"

[<Test>]
let ``Check credentials config for executing program with NetworkCredentials`` () =
    cli {
        Exec "cmd.exe"
        WorkingDirectory @"C:\Users"
        Username "admin@company"
    }
    |> _.config.UserName.Value
    |> should equal "admin@company"

[<Test>]
let ``Check EnvironmentVariables with single KeyValue config`` () =
    cli {
        Exec "cmd.exe"
        EnvironmentVariable("user", "admin")
    }
    |> _.config.EnvironmentVariables.Value
    |> should equal [ ("user", "admin") ]

[<Test>]
let ``Check EnvironmentVariables with multiple KeyValues config`` () =
    cli {
        Exec "cmd.exe"
        EnvironmentVariables [ ("user", "admin"); ("path", "path/to/file") ]
    }
    |> _.config.EnvironmentVariables.Value
    |> should equal [ ("user", "admin"); ("path", "path/to/file") ]

[<Test>]
let ``Check Credentials with domain, username and password`` () =
    cli {
        Exec "cmd.exe"
        Credentials("domain", "user", "password123")
    }
    |> _.config.Credentials.Value
    |> should equal (Credentials("domain", "user", "password123"))

[<Test>]
let ``Check Encoding with setting Encoding`` () =
    cli {
        Exec "cmd.exe"
        Encoding System.Text.Encoding.UTF8
    }
    |> _.config.Encoding.Value
    |> should equal System.Text.Encoding.UTF8
