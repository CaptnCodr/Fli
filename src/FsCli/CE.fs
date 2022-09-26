[<AutoOpen>]
module FsCli.CE

open FsCli.Domain
open FsCli.Cli


type ICommandContext<'a> with

    member this.Yield(_) = this


let cli = { config = { Cli = CMD; Command = "" } }

type ICommandContext<'a> with

    [<CustomOperation("CLI")>]
    member this.Cli(context: ICommandContext<CommandContext>, cli) = Cli.cli cli context.Self

    [<CustomOperation("Command")>]
    member this.Command(context: ICommandContext<CommandContext>, command) = Cli.command command context.Self
