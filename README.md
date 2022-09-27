# FsCli
[![build](https://github.com/CaptnCodr/FsCli/actions/workflows/build.yml/badge.svg)](https://github.com/CaptnCodr/FsCli/actions/workflows/build.yml)

Execute CLI commands in your F# code in F# style!

### Getting Started
Just `open FsCli` and start ...

For example:
```fsharp
cli {
    CLI CMD
    Command "echo Hello World!"
}
|> Command.execute
```
that starts `CMD.exe` as CLI and `echo Hello World!` is the command to execute.

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

Currently provided CLI's:
- `cmd.exe` as `CMD`
- `pwsh.exe` as `PWSH`
- ... more are coming soon

### Inspiration
Use CE's for command line interface commands came in mind while using [FsHttp](https://github.com/fsprojects/FsHttp).