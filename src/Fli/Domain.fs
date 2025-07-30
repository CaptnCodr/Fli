namespace Fli

[<AutoOpen>]
module Domain =

    open System
    open System.Text

    type ICommandContext<'a> =
        abstract member Context: 'a

    type ShellConfig =
        { Shell: Shells
          Command: string option
          Input: string option
          Output: Outputs option
          Stream: Outputs option
          WorkingDirectory: string option
          EnvironmentVariables: (string * string) list option
          Encoding: Encoding option
          CancelAfter: int option
          WindowStyle: WindowStyle option }

    and Shells =
        | CMD
        | PS
        | PWSH
        | WSL
        | SH
        | BASH
        | ZSH
        | CUSTOM of shell: string * flag: string

    and Outputs =
        | File of string
        | StringBuilder of StringBuilder
        | Custom of Func<string, unit>

    and WindowStyle =
        | Hidden
        | Maximized
        | Minimized
        | Normal

    type ExecConfig =
        { Program: string
          Arguments: Arguments option
          Input: string option
          Output: Outputs option
          Stream: Outputs option
          WorkingDirectory: string option
          Verb: string option
          UserName: string option
          Credentials: Credentials option
          EnvironmentVariables: (string * string) list option
          Encoding: Encoding option
          CancelAfter: int option
          WindowStyle: WindowStyle option }

    and Credentials = Credentials of Domain: string * UserName: string * Password: string

    and Arguments =
        | Arguments of string option
        | ArgumentList of string array option

        member x.toString() =
            match x with
            | Arguments(Some s) -> s
            | ArgumentList(Some s) ->
                let escapeString (str: string) =
                    if str.Contains("\"") then
                        str.Replace("\"", "\"\"") |> fun s -> $"\"{s}\""
                    else
                        str

                s |> Seq.map escapeString |> String.concat " "
            | _ -> ""

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
              Stream = None
              WorkingDirectory = None
              EnvironmentVariables = None
              Encoding = None
              CancelAfter = None
              WindowStyle = None }
          ExecConfig =
            { Program = ""
              Arguments = None
              Input = None
              Output = None
              Stream = None
              WorkingDirectory = None
              Verb = None
              UserName = None
              Credentials = None
              EnvironmentVariables = None
              Encoding = None
              CancelAfter = None
              WindowStyle = None } }

    type Output =
        { Id: int
          Text: string option
          ExitCode: int
          Error: string option }
