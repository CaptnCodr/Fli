namespace Fli

open Domain

module Cli =

    let shell (shell: Shells) (config: Config) : ShellContext =
        { config = { config.ShellConfig with Shell = shell } }

    let command (command: string) (context: ShellContext) =
        { context with config = { context.config with Command = command } }

    let workingDirectory (workingDirectory: string) (context: ShellContext) =
        { context with config = { context.config with WorkingDirectory = Some workingDirectory } }

    let credentials (credentials: System.Net.NetworkCredential) (context: ShellContext) =
        { context with config = { context.config with Credentials = Some credentials } }

module Program =

    let program (program: string) (config: Config) : ProgramContext =
        { config = { config.ProgramConfig with Program = program } }

    let arguments (arguments: string) (context: ProgramContext) =
        { context with config = { context.config with Arguments = arguments } }

    let workingDirectory (workingDirectory: string) (context: ProgramContext) =
        { context with config = { context.config with WorkingDirectory = Some workingDirectory } }

    let verb (verb: string) (context: ProgramContext) =
        { context with config = { context.config with Verb = Some verb } }
        
    let credentials (credentials: System.Net.NetworkCredential) (context: ProgramContext) =
        { context with config = { context.config with Credentials = Some credentials } }
