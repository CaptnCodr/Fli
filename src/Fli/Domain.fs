namespace Fli

[<AutoOpen>]
module Domain =

    type ICommandContext<'a> =
        abstract member Context: 'a

    type ShellConfig =
        { Shell: Shells
          Command: string option
          Input: string option
          Output: string option
          WorkingDirectory: string option
          EnvironmentVariables: (string * string) list option
          Encoding: System.Text.Encoding option }

    and Shells =
        | CMD
        | PS
        | PWSH
        | WSL
        | BASH

    type ExecConfig =
        { Program: string
          Arguments: string option
          Input: string option
          Output: string option
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
              Command = None
              Input = None
              Output = None
              WorkingDirectory = None
              EnvironmentVariables = None
              Encoding = None }
          ExecConfig =
            { Program = ""
              Arguments = None
              Input = None
              Output = None
              WorkingDirectory = None
              Verb = None
              UserName = None
              Credentials = None
              EnvironmentVariables = None
              Encoding = None } }

    type Output =
        { Id: int
          Text: string option
          ExitCode: int
          Error: string option }

        /// Gets `Id` from `Output`.
        static member toId(output: Output) = output.Id

        /// Gets `Text` from `Output`.
        static member toText(output: Output) = output.Text |> Option.defaultValue ""

        /// Gets `ExitCode` from `Output`.
        static member toExitCode(output: Output) = output.ExitCode

        /// Gets `Error` from `Output`.
        static member toError(output: Output) = output.Error |> Option.defaultValue ""