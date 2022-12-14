namespace Fli

[<AutoOpen>]
module Domain =

    open System.Text

    type ICommandContext<'a> =
        abstract member Context: 'a

    type ShellConfig =
        { Shell: Shells
          Command: string option
          Input: string option
          Output: Outputs option
          WorkingDirectory: string option
          EnvironmentVariables: (string * string) list option
          Encoding: Encoding option }

    and Shells =
        | CMD
        | PS
        | PWSH
        | WSL
        | BASH
        | CUSTOM of shell: string * flag: string

    and Outputs =
        | File of string
        | StringBuilder of StringBuilder
        | Custom of System.Func<string, unit>

    type ExecConfig =
        { Program: string
          Arguments: string option
          Input: string option
          Output: Outputs option
          WorkingDirectory: string option
          Verb: string option
          UserName: string option
          Credentials: Credentials option
          EnvironmentVariables: (string * string) list option
          Encoding: Encoding option }

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

        /// Prints `Id` from `Output`.
        static member printId = Output.toId >> printfn "%i"

        /// Gets `Text` from `Output`.
        static member toText(output: Output) = output.Text |> Option.defaultValue ""

        /// Prints `Text` from `Output`.
        static member printText = Output.toText >> printfn "%s"

        /// Gets `ExitCode` from `Output`.
        static member toExitCode(output: Output) = output.ExitCode

        /// Prints `ExitCode` from `Output`.
        static member printExitCode = Output.toExitCode >> printfn "%i"

        /// Gets `Error` from `Output`.
        static member toError(output: Output) = output.Error |> Option.defaultValue ""

        /// Prints `Error` from `Output`.
        static member printError = Output.toError >> printfn "%s"
