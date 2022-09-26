module FsCli.Cli

module Cli =

    let cli (cli: Cli) (context: CommandContext) =
        { context with config = { context.config with Cli = cli } }

    let command (command: string) (context: CommandContext) =
        { context with config = { context.config with Command = command } }
