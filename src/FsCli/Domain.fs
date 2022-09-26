[<AutoOpen>]
module FsCli.Domain

type ICommandContext<'a> =
    abstract member Self: 'a

type Config = { Cli: Cli; Command: string }

and Cli =
    | CMD
    | PWSH

type CommandContext =
    { config: Config }

    interface ICommandContext<CommandContext> with
        member this.Self = this
