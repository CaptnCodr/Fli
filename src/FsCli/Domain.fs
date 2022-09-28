[<AutoOpen>]
module FsCli.Domain

type ICommandContext<'a> =
    abstract member Self: 'a

type CliConfig = { Cli: Cli; Command: string }

and Cli =
    | CMD
    | PWSH
    | Bash

type ProgramConfig = { Program: string; Arguments: string }

type Config =
    { CliConfig: CliConfig
      ProgramConfig: ProgramConfig }

type CliContext =
    { config: CliConfig }

    interface ICommandContext<CliContext> with
        member this.Self = this

type ProgramContext =
    { config: ProgramConfig }

    interface ICommandContext<ProgramContext> with
        member this.Self = this
