module FsCli.Cli

module Cli =

    let cli (cli: Cli) (config: Config) : CliContext =
        { config = { config.CliConfig with Cli = cli } }

    let command (command: string) (context: CliContext) : CliContext =
        { context with config = { context.config with Command = command } }

module Program =

    let program (program: string) (config: Config) : ProgramContext =
        { config = { config.ProgramConfig with Program = program } }

    let arguments (arguments: string) (context: ProgramContext) : ProgramContext =
        { context with config = { context.config with Arguments = arguments } }
