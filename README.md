# Fli
[![build](https://github.com/CaptnCodr/Fli/actions/workflows/build.yml/badge.svg)](https://github.com/CaptnCodr/Fli/actions/workflows/build.yml)
[![Nuget](https://img.shields.io/nuget/v/fli?color=33cc56)](https://www.nuget.org/packages/Fli/)

Execute CLI commands from your F# code in F# style!

### Getting Started
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

#### Implementations

Currently provided Shells:
- `cmd.exe` as `CMD`
- `powershell.exe` as `PS`
- `pwsh.exe` as `PWSH`
- `bash` as `BASH`
- ...

### Inspiration
Use CE's for command line interface commands came in mind while using [FsHttp](https://github.com/fsprojects/FsHttp).