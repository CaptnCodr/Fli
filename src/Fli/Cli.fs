namespace Fli

open Domain

module Cli =

    let shell (shell: Shells) (config: Config) : ShellContext =
        { config = { config.ShellConfig with Shell = shell } }

    let command (command: string) (context: ShellContext) =
        { context with config = { context.config with Command = command } }

    let workingDirectory (workingDirectory: string) (context: ShellContext) =
        { context with config = { context.config with WorkingDirectory = Some workingDirectory } }

    let environmentVariables (variables: (string * string) list) (context: ShellContext) =
        let vars =
            match context.config.EnvironmentVariables with
            | Some (vs) -> vs @ variables
            | None -> variables

        { context with config = { context.config with EnvironmentVariables = Some vars } }

module Program =

    let program (program: string) (config: Config) : ProgramContext =
        { config = { config.ProgramConfig with Program = program } }

    let arguments (arguments: string) (context: ProgramContext) =
        { context with config = { context.config with Arguments = Some arguments } }

    let workingDirectory (workingDirectory: string) (context: ProgramContext) =
        { context with config = { context.config with WorkingDirectory = Some workingDirectory } }

    let verb (verb: string) (context: ProgramContext) =
        { context with config = { context.config with Verb = Some verb } }

    let userName (userName: string) (context: ProgramContext) =
        { context with config = { context.config with UserName = Some userName } }

    let credentials (credentials: Credentials) (context: ProgramContext) =
        { context with config = { context.config with Credentials = Some credentials } }

    let environmentVariables (variables: (string * string) list) (context: ProgramContext) =
        let vars =
            match context.config.EnvironmentVariables with
            | Some (vs) -> vs @ variables
            | None -> variables

        { context with config = { context.config with EnvironmentVariables = Some vars } }
