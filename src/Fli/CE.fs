namespace Fli

[<AutoOpen>]
module CE =

    open Domain
    open System.Collections

    type ICommandContext<'a> with

        member this.Yield(_) = this


    let private defaults =
        { ShellConfig =
            { Shell = CMD
              Command = ""
              WorkingDirectory = None }
          ProgramConfig =
            { Program = ""
              Arguments = ""
              WorkingDirectory = None
              Verb = None } }

    type StartingContext =
        { config: Config option }

        member this.CurrentConfig = this.config |> Option.defaultValue defaults

        interface ICommandContext<StartingContext> with
            member this.Context = this

    let cli = { config = None }


    /// Extensions for CLI context
    type ICommandContext<'a> with

        [<CustomOperation("Shell")>]
        member this.Cli(context: ICommandContext<StartingContext>, shell) =
            Cli.shell shell context.Context.CurrentConfig

        [<CustomOperation("Command")>]
        member this.Command(context: ICommandContext<ShellContext>, command) = Cli.command command context.Context

        [<CustomOperation("WorkingDirectory")>]
        member this.WorkingDirectory(context: ICommandContext<ShellContext>, workingDirectory) =
            Cli.workingDirectory workingDirectory context.Context

    /// Extensions for Exec context
    type ICommandContext<'a> with

        [<CustomOperation("Exec")>]
        member this.Exec(context: ICommandContext<StartingContext>, program) =
            Program.program program context.Context.CurrentConfig

        [<CustomOperation("Arguments")>]
        member this.Arguments(context: ICommandContext<ProgramContext>, arguments) =
            let args =
                match box (arguments) with
                | :? string as s -> s
                | :? seq<string> as s -> s |> Seq.map (fun sa -> sa |> string) |> String.concat " "
                | :? list<string> as l -> l |> List.map (fun la -> la |> string) |> String.concat " "
                | :? array<string> as a -> a |> Array.map (fun aa -> aa |> string) |> String.concat " "
                | :? IEnumerable as e -> e |> Seq.cast |> Seq.map (fun ea -> ea |> string) |> String.concat " "
                | _ -> failwith "Cannot convert arguments to a string!"

            Program.arguments args context.Context

        [<CustomOperation("WorkingDirectory")>]
        member this.WorkingDirectory(context: ICommandContext<ProgramContext>, workingDirectory) =
            Program.workingDirectory workingDirectory context.Context

        [<CustomOperation("Verb")>]
        member this.Verb(context: ICommandContext<ProgramContext>, verb) = Program.verb verb context.Context
