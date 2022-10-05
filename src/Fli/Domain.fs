namespace Fli

[<AutoOpen>]
module Domain =

    type ICommandContext<'a> =
        abstract member Context: 'a

    type ShellConfig =
        { Shell: Shells
          Command: string
          WorkingDirectory: string option
          EnvironmentVariables: (string * string) list option
          Encoding: System.Text.Encoding option }

    and Shells =
        | CMD
        | PS
        | PWSH
        | BASH

    type ExecConfig =
        { Program: string
          Arguments: string option
          WorkingDirectory: string option
          Verb: string option
          UserName: string option
          Credentials: Credentials option
          EnvironmentVariables: (string * string) list option
          Encoding: System.Text.Encoding option }

    and Credentials = Credentials of Domain: string * UserName: string * Password: string

    type Config =
        { ShellConfig: ShellConfig
          ExecConfig: ExecConfig }

    type ShellContext =
        { config: ShellConfig }

        interface ICommandContext<ShellContext> with
            member this.Context = this

    type ExecContext =
        { config: ExecConfig }

        interface ICommandContext<ExecContext> with
            member this.Context = this

    let Defaults =
        { ShellConfig =
            { Shell = CMD
              Command = ""
              WorkingDirectory = None
              EnvironmentVariables = None 
              Encoding = None }
          ExecConfig =
            { Program = ""
              Arguments = None
              WorkingDirectory = None
              Verb = None
              UserName = None
              Credentials = None
              EnvironmentVariables = None
              Encoding = None } }
