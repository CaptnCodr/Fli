namespace Fli

open Domain

module Cli =

    let shell (shell: Shells) (config: Config) : ShellContext =
        { config =
            { config.ShellConfig with
                Shell = shell } }

    let command (command: string) (context: ShellContext) =
        { context with
            config.Command = Some command }

    let input (input: string) (context: ShellContext) =
        { context with
            config.Input = Some input }

    let output (output: Outputs) (context: ShellContext) =
        { context with
            config.Output = Some output }

    let workingDirectory (workingDirectory: string) (context: ShellContext) =
        { context with
            config.WorkingDirectory = Some workingDirectory }

    let windowStyle (windowStyle: WindowStyle) (context: ShellContext) =
        { context with
            config.WindowStyle = Some windowStyle }

    let environmentVariables (variables: (string * string) list) (context: ShellContext) =
        let vars =
            match context.config.EnvironmentVariables with
            | Some(vs) -> vs @ variables
            | None -> variables

        { context with
            config.EnvironmentVariables = Some vars }

    let encoding (encoding: System.Text.Encoding) (context: ShellContext) =
        { context with
            config.Encoding = Some encoding }

    let cancelAfter (cancelAfter: int) (context: ShellContext) =
        { context with
            config.CancelAfter = Some cancelAfter }

module Program =

    let program (program: string) (config: Config) : ExecContext =
        { config =
            { config.ExecConfig with
                Program = program } }

    let arguments (arguments: string) (context: ExecContext) =
        { context with
            config.Arguments = Some arguments }

    let input (input: string) (context: ExecContext) =
        { context with
            config.Input = Some input }

    let output (output: Outputs) (context: ExecContext) =
        { context with
            config.Output = Some output }

    let workingDirectory (workingDirectory: string) (context: ExecContext) =
        { context with
            config.WorkingDirectory = Some workingDirectory }

    let windowStyle (windowStyle: WindowStyle) (context: ExecContext) =
        { context with
            config.WindowStyle = Some windowStyle }

    let verb (verb: string) (context: ExecContext) =
        { context with config.Verb = Some verb }

    let userName (userName: string) (context: ExecContext) =
        { context with
            config.UserName = Some userName }

    let credentials (credentials: Credentials) (context: ExecContext) =
        { context with
            config.Credentials = Some credentials }

    let environmentVariables (variables: (string * string) list) (context: ExecContext) =
        let vars =
            match context.config.EnvironmentVariables with
            | Some(vs) -> vs @ variables
            | None -> variables

        { context with
            config.EnvironmentVariables = Some vars }

    let encoding (encoding: System.Text.Encoding) (context: ExecContext) =
        { context with
            config.Encoding = Some encoding }

    let cancelAfter (cancelAfter: int) (context: ExecContext) =
        { context with
            config.CancelAfter = Some cancelAfter }
