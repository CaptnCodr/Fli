# Fli
[![build](https://github.com/CaptnCodr/Fli/actions/workflows/build.yml/badge.svg)](https://github.com/CaptnCodr/Fli/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/fli?color=33cc56)](https://www.nuget.org/packages/Fli/)
<img align="right" width="100" src="https://raw.githubusercontent.com/CaptnCodr/Fli/main/logo.png">

Execute CLI commands from your F# code in F# style!

**Fli is part of the F# Advent Calendar 2022: [A little story about Fli](https://gist.github.com/CaptnCodr/d709b30eb1191bedda090623d04bf738)**

### Features
- Starting processes easily
- Execute CLI commands in your favourite shell
- F# computation expression syntax
- Wrap authenticated CLI tools
- No external dependencies

### Install
Get it from [Nuget](https://www.nuget.org/packages/Fli/): `dotnet add package Fli`

### Usage
`open Fli` and start

For example:
```fsharp
cli {
    Shell CMD
    Command "echo Hello World!"
}
|> Command.execute
```
that starts `CMD.exe` as Shell and `echo Hello World!` is the command to execute.

Run a file with PowerShell from a specific directory:
```fsharp
cli {
    Shell PWSH
    Command "test.bat"
    WorkingDirectory (Environment.GetFolderPath Environment.SpecialFolder.UserProfile)
}
|> Command.execute
```

Executing programs with arguments:
```fsharp
cli {
    Exec "path/to/executable"
    Arguments "--info"
}
|> Command.execute
```

an example with `git`:
```fsharp
cli {
    Exec "git"
    Arguments ["commit"; "-m"; "\"Fixing issue #1337.\""]
}
|> Command.execute
```

Add a verb to your executing program:
```fsharp
cli {
    Exec "adobe.exe"
    Arguments (Path.Combine ((Environment.GetFolderPath Environment.SpecialFolder.UserProfile), "test.pdf"))
    Verb "open"
}
|> Command.execute
```
or open a file in the default/assigned program:
```fsharp
cli {
    Exec "test.pdf"
}
|> Command.execute
```
(Hint: if file extension is not assigned to any installed program, it will throw a `System.NullReferenceException`)

Write output to a specific file:
```fsharp
cli {
    Exec "dotnet"
    Arguments "--list-sdks"
    Output @"absolute\path\to\dotnet-sdks.txt"
}
|> Command.execute
```

Write output to a function (logging, printing, etc.):
```fsharp
let log (output: string) = Debug.Log($"CLI log: {output}")

cli {
    Exec "dotnet"
    Arguments "--list-sdks"
    Output log
}
|> Command.execute
```

Add environment variables for the executing program:
```fsharp
cli {
    Exec "git"
    EnvironmentVariables [("GIT_AUTHOR_NAME", "Jon Doe"); ("GIT_AUTHOR_EMAIL", "jon.doe@domain.com")]
    Output ""
}
|> Command.execute
```
Hint: `Output ""` will be ignored. This is for conditional cases, e.g.: `Output (if true then logFilePath else "")`.

Add credentials to program:
```fsharp
cli {
    Exec "program"
    Credentials ("domain", "bobk", "password123")
}
|> Command.execute
```
Hint: Running a process as a different user is supported on all platforms. Other options (Domain, Password) are only available on Windows. As an alternative for not Windows based systems there is:
```fsharp
cli {
    Exec "path/to/program"
    Username "admin"
}
|> Command.execute
```

For Windows applications it's possible to set their visibility. There are four possible values: `Hidden`, `Maximized`, `Minimized` and `Normal`. The default is `Hidden`.
```fsharp
cli {
    Exec @"C:\Windows\regedit.exe"
    WindowStyle Normal
}
|> Command.execute
```

#### `Command.execute`
`Command.execute` returns record: `type Output = { Id: int; Text: string option; ExitCode: int; Error: string option }`
which has getter methods to get only one value:
```fsharp
toId: Output -> int
toText: Output -> string
toExitCode: Output -> int
toError: Output -> string
```
example:
```fsharp
cli {
    Shell CMD
    Command "echo Hello World!"
}
|> Command.execute // { Id = 123; Text = Some "Hello World!"; ExitCode = 0; Error = None }
|> Output.toText // "Hello World!"

// same with Output.toId:
cli { ... }
|> Command.execute // { Id = 123; Text = Some "Hello World!"; ExitCode = 0; Error = None }
|> Output.toId // 123

// same with Output.toExitCode:
cli { ... }
|> Command.execute // { Id = 123; Text = Some "Hello World!"; ExitCode = 0; Error = None }
|> Output.toExitCode // 0

// in case of an error:
cli { ... }
|> Command.execute // { Id = 123; Text = None; ExitCode = 1; Error = Some "This is an error!" }
|> Output.toError // "This is an error!"
```

#### `Output` functions
```fsharp
throwIfErrored: Output -> Output
throw: (Output -> bool) -> Output -> Output
```

`Output.throw` and `Output.throwIfErrored` are assertion functions that if something's not right it will throw an exception.
That is useful for build scripts to stop the execution immediately, here is an example:
```fsharp
cli {
    Exec "dotnet"
    Arguments [| "build"; "-c"; "Release" |]
    WorkingDirectory "src/"
}
|> Command.execute // returns { Id = 123; Text = None; ExitCode = 1; Error = Some "This is an error!" }
|> Output.throwIfErrored // <- Exception thrown!
|> Output.toError
```

or, you can define when to "fail":
```fsharp
cli { ... }
|> Command.execute // returns { Id = 123; Text = "An error occured: ..."; ExitCode = 1; Error = Some "Error detail." }
|> Output.throw (fun output -> output.Text.Contains("error")) // <- Exception thrown!
|> Output.toError
```

#### Printing `Output` fields
There are printing methods in `Output` too:
```fsharp
printId: Output -> unit
printText: Output -> unit
printExitCode: Output -> unit
printError: Output -> unit
```

Instead of writing:
```fsharp
cli { ... }
|> Command.execute
|> Output.toText
|> printfn "%s"
```
For a little shorter code you can use:
```fsharp
cli { ... }
|> Command.execute
|> Output.printText
```

#### `Command.toString`
`Command.toString` concatenates only the executing shell/program + the given commands/arguments:
```fsharp
cli {
    Shell PS
    Command "Write-Host Hello World!"
}
|> Command.toString // "powershell.exe -Command Write-Host Hello World!"
```
and:
```fsharp
cli {
    Exec "cmd.exe"
    Arguments [ "/C"; "echo"; "Hello World!" ]
}
|> Command.toString // "cmd.exe /C echo Hello World!"
```

#### Builder operations:

`ShellContext` operations (`cli { Shell ... }`):
| Operation              |  Type                      |
|------------------------|----------------------------|
| `Shell`                | `Fli.Shells`               |
| `Command`              | `string`                   |
| `Input`                | `string`                   |
| `Output`               | `Fli.Outputs`              |
| `WorkingDirectory`     | `string`                   |
| `WindowStyle`          | `Fli.WindowStyle`          |
| `EnvironmentVariable`  | `string * string`          |
| `EnvironmentVariables` | `(string * string) list`   |
| `Encoding`             | `System.Text.Encoding`     |
| `CancelAfter`          | `int`                      |

`ExecContext` operations (`cli { Exec ... }`):
| Operation              |  Type                                                    |
|------------------------|----------------------------------------------------------|
| `Exec`                 | `string`                                                 |
| `Arguments`            | `string` / `string seq` / `string list` / `string array` |
| `Input`                | `string`                                                 |
| `Output`               | `Fli.Outputs`                                            |
| `Verb`                 | `string`                                                 |
| `Username`             | `string`                                                 |
| `Credentials`          | `string * string * string`                               |
| `WorkingDirectory`     | `string`                                                 |
| `WindowStyle`          | `Fli.WindowStyle`                                        |
| `EnvironmentVariable`  | `string * string`                                        |
| `EnvironmentVariables` | `(string * string) list`                                 |
| `Encoding`             | `System.Text.Encoding`                                   |
| `CancelAfter`          | `int`                                                    |

Currently provided `Fli.Shells`:
- `CMD` runs either `cmd.exe /c ...` or `cmd.exe /k ...` (if `Input` is provided)
- `PS` runs `powershell.exe -Command ...`
- `PWSH` runs `pwsh.exe -Command ...`
- `WSL` runs `wsl.exe -- ...`
- `SH` runs `sh -c ...`
- `BASH` runs `bash -c ...`
- `ZSH` runs `zsh -c ...`
- `CUSTOM (shell: string * flag: string)` runs the specified `shell` with the specified starting argument (`flag`)

Provided `Fli.Outputs`:
- `File of string` a string with an absolute path of the output file.
- `StringBuilder of StringBuilder` a StringBuilder which will be filled with the output text.
- `Custom of Func<string, unit>` a custom function (`string -> unit`) that will be called with the output string (logging, printing etc.).

Provided `Fli.WindowStyle`:
- `Hidden` (default)
- `Maximized`
- `Minimized`
- `Normal`

### Do you miss something?
Open an [issue](https://github.com/CaptnCodr/Fli/issues) or start a [discussion](https://github.com/CaptnCodr/Fli/discussions).

### Contributing
After cloning this repository, there are some steps to start:
1. `dotnet tool restore`
2. `dotnet paket restore`
3. `dotnet restore`
4. `dotnet paket install`
5. `dotnet build`

After that, you can start coding, build and test.

Every contribution is welcome. :)

### Inspiration
Use CE's for CLI commands came in mind while using [FsHttp](https://github.com/fsprojects/FsHttp).
