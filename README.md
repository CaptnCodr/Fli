# Fli
[![build](https://github.com/CaptnCodr/Fli/actions/workflows/build.yml/badge.svg)](https://github.com/CaptnCodr/Fli/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/fli?color=33cc56)](https://www.nuget.org/packages/Fli/)

Execute CLI commands from your F# code in F# style!

### Getting Started
Get it from Nuget: `dotnet add package Fli`

Just `open Fli` and start ...

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

### Implementations

#### Builder Methods:

`ShellContext` methods (`cli { Shell ... }`):
| Method                 |  operation type          |
|------------------------|--------------------------|
| `Shell`                | `Fli.Shells`             |
| `Command`              | `string`                 |
| `WorkingDirectory`     | `string`                 |
| `EnvironmentVariable`  | `string * string`        |
| `EnvironmentVariables` | `(string * string) list` |
| `Encoding`             | `System.Text.Encoding`   |

`ExecContext` methods (`cli { Exec ... }`):
| Method                 |  operation type            |
|------------------------|----------------------------|
| `Exec`                 | `string`                   |
| `Arguments`            | `string` / `string seq` / `string list` / `string array` |
| `Verb`                 | `string`                   |
| `Username`             | `string`                   |
| `Credentials`          | `string * string * string` |
| `WorkingDirectory`     | `string`                   |
| `EnvironmentVariable`  | `string * string`          |
| `EnvironmentVariables` | `(string * string) list`   |
| `Encoding`             | `System.Text.Encoding`     |

Currently provided `Fli.Shells`:
- `CMD` runs `cmd.exe /C ...`
- `PS` runs `powershell.exe -Command ...`
- `PWSH` runs `pwsh.exe -Command ...`
- `BASH` runs `bash -c ..`

### Inspiration
Use CE's for command line interface commands came in mind while using [FsHttp](https://github.com/fsprojects/FsHttp).