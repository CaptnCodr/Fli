namespace Fli

open Domain

module Cli =

    let shell (shell: Shells) (config: Config) : ShellContext =
        { config = { config.ShellConfig with Shell = shell } }

    let command (command: string) (context: ShellContext) =
        { context with config = { context.config with Command = Some command } }

    let input (input: string) (context: ShellContext) =
        { context with config = { context.config with Input = Some input } }

    let workingDirectory (workingDirectory: string) (context: ShellContext) =
        { context with config = { context.config with WorkingDirectory = Some workingDirectory } }

    let environmentVariables (variables: (string * string) list) (context: ShellContext) =
        let vars =
            match context.config.EnvironmentVariables with
            | Some (vs) -> vs @ variables
            | None -> variables

        { context with config = { context.config with EnvironmentVariables = Some vars } }

    let encoding (encoding: System.Text.Encoding) (context: ShellContext) =
        { context with config = { context.config with Encoding = Some encoding } }

module Program =

    let program (program: string) (config: Config) : ExecContext =
        { config = { config.ExecConfig with Program = program } }

    let arguments (arguments: string) (context: ExecContext) =
        { context with config = { context.config with Arguments = Some arguments } }

    let input (input: string) (context: ExecContext) =
        { context with config = { context.config with Input = Some input } }

    let workingDirectory (workingDirectory: string) (context: ExecContext) =
        { context with config = { context.config with WorkingDirectory = Some workingDirectory } }

    let verb (verb: string) (context: ExecContext) =
        { context with config = { context.config with Verb = Some verb } }

    let userName (userName: string) (context: ExecContext) =
        { context with config = { context.config with UserName = Some userName } }

    let credentials (credentials: Credentials) (context: ExecContext) =
        { context with config = { context.config with Credentials = Some credentials } }

    let environmentVariables (variables: (string * string) list) (context: ExecContext) =
        let vars =
            match context.config.EnvironmentVariables with
            | Some (vs) -> vs @ variables
            | None -> variables

        { context with config = { context.config with EnvironmentVariables = Some vars } }

    let encoding (encoding: System.Text.Encoding) (context: ExecContext) =
        { context with config = { context.config with Encoding = Some encoding } }
