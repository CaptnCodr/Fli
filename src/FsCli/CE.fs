namespace FsCli

[<AutoOpen>]
module CE = 

    open Domain
    open System.Collections

    type ICommandContext<'a> with

        member this.Yield(_) = this


    let private defaults =
        { CliConfig = { Cli = CMD; Command = "" }
          ProgramConfig = { Program = ""; Arguments = "" } }

    type StartingContext =
        { config: Config option }

        member this.ActualConfig = this.config |> Option.defaultValue defaults

        interface ICommandContext<StartingContext> with
            member this.Self = this

    let cli = { config = None }


    /// Extensions for CLI context
    type ICommandContext<'a> with

        [<CustomOperation("CLI")>]
        member this.Cli(context: ICommandContext<StartingContext>, cli) = Cli.cli cli context.Self.ActualConfig

        [<CustomOperation("Command")>]
        member this.Command(context: ICommandContext<CliContext>, command) = Cli.command command context.Self

    /// Extensions for Exec context
    type ICommandContext<'a> with

        [<CustomOperation("Exec")>]
        member this.Exec(context: ICommandContext<StartingContext>, program) =
            Program.program program context.Self.ActualConfig

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

            Program.arguments args context.Self
