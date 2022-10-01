namespace Fli

[<AutoOpen>]
module Domain =

    type ICommandContext<'a> =
        abstract member Context: 'a

    type ShellConfig =
        { Shell: Shells
          Command: string
          WorkingDirectory: string option
          Credentials: System.Net.NetworkCredential option }

    and Shells =
        | CMD
        | PS
        | PWSH
        | BASH

    type ProgramConfig =
        { Program: string
          Arguments: string
          WorkingDirectory: string option
          Credentials: System.Net.NetworkCredential option }

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
              Credentials = None }
          ProgramConfig =
            { Program = ""
              Arguments = ""
              WorkingDirectory = None
              Credentials = None } }