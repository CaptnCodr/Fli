namespace Fli

[<AutoOpen>]
module Domain =

    type ICommandContext<'a> =
        abstract member Context: 'a

    type ShellConfig =
        { Shell: Shells
          Command: string
          WorkingDirectory: string option
          EnvironmentVariables: (string * string) list option }

    and Shells =
        | CMD
        | PS
        | PWSH
        | BASH

    type ProgramConfig =
        { Program: string
          Arguments: string option
          WorkingDirectory: string option
          Verb: string option
          UserName: string option
          EnvironmentVariables: (string * string) list option }

    type Config =
        { ShellConfig: ShellConfig
          ProgramConfig: ProgramConfig }

    type ShellContext =
        { config: ShellConfig }

        interface ICommandContext<ShellContext> with
            member this.Context = this

    type ProgramContext =
        { config: ProgramConfig }

        interface ICommandContext<ProgramContext> with
            member this.Context = this

    let Defaults =
        { ShellConfig =
            { Shell = CMD
              Command = ""
              WorkingDirectory = None
              EnvironmentVariables = None }
          ProgramConfig =
            { Program = ""
              Arguments = None
              WorkingDirectory = None
              Verb = None
              UserName = None
              EnvironmentVariables = None } }
