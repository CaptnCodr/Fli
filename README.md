# Fli
[![build](https://github.com/CaptnCodr/Fli/actions/workflows/build.yml/badge.svg)](https://github.com/CaptnCodr/Fli/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/fli?color=33cc56)](https://www.nuget.org/packages/Fli/)

Execute CLI commands from your F# code in F# style!

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
    Arguments ["commit"; "-m"; "Fixing issue #1337."]
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

Write output to a specific file:
```fsharp
cli {
    Exec "dotnet"
    Arguments "--list-sdks"
    Output @"absolute\path\to\dotnet-sdks.txt"
}
|> Command.execute
```

Add environment variables for the executing program:
```fsharp
cli {
    Exec "git"
    EnvironmentVariables [("GIT_AUTHOR_NAME", "Jon Doe"); ("GIT_AUTHOR_EMAIL", "jon.doe@domain.com")]
}
|> Command.execute
```

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

#### `Command.toString`
`Command.toString` concatenates only the the executing shell/program + the given commands/arguments:
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
| `Output`               | `Outputs` (see below)      |
| `WorkingDirectory`     | `string`                   |
| `EnvironmentVariable`  | `string * string`          |
| `EnvironmentVariables` | `(string * string) list`   |
| `Encoding`             | `System.Text.Encoding`     |

`ExecContext` operations (`cli { Exec ... }`):
| Operation              |  Type                                                    |
|------------------------|----------------------------------------------------------|
| `Exec`                 | `string`                                                 |
| `Arguments`            | `string` / `string seq` / `string list` / `string array` |
| `Input`                | `string`                                                 |
| `Output`               | `Outputs` (see below)                                    |
| `Verb`                 | `string`                                                 |
| `Username`             | `string`                                                 |
| `Credentials`          | `string * string * string`                               |
| `WorkingDirectory`     | `string`                                                 |
| `EnvironmentVariable`  | `string * string`                                        |
| `EnvironmentVariables` | `(string * string) list`                                 |
| `Encoding`             | `System.Text.Encoding`                                   |

Currently provided `Fli.Shells`:
- `CMD` runs `cmd.exe /c ...` or `cmd.exe /k ...` (depends if `Input` is provided or not)
- `PS` runs `powershell.exe -Command ...`
- `PWSH` runs `pwsh.exe -Command ...`
- `WSL` runs `wsl.exe -- ...`
- `BASH` runs `bash -c ...`

Provided `Fli.Outputs`:
- `File of string` a string with an absolute path of the output file.
- `StringBuilder of StringBuilder` a StringBuilder which will be filled with the output text.
- `Custom of Func<string, unit>` a custom function (`string -> unit`) that will be called with the output string (logging, printing etc.).

### Do you miss something?
Open an [issue](https://github.com/CaptnCodr/Fli/issues) or start a [discussion](https://github.com/CaptnCodr/Fli/discussions).

### Inspiration
Use CE's for CLI commands came in mind while using [FsHttp](https://github.com/fsprojects/FsHttp).