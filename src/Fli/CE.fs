namespace Fli

[<AutoOpen>]
module CE =

    open Domain
    open System.Collections

    type ICommandContext<'a> with

        member this.Yield(_) = this

    type StartingContext =
        { config: Config option }

        member this.CurrentConfig = this.config |> Option.defaultValue Defaults

        interface ICommandContext<StartingContext> with
            member this.Context = this

    let cli = { config = None }

    let private matchArguments arguments =
        match box (arguments) with
        | :? string as s -> s
        | :? seq<string> as s -> s |> Seq.map (fun sa -> sa |> string) |> String.concat " "
        | :? list<string> as l -> l |> List.map (fun la -> la |> string) |> String.concat " "
        | :? array<string> as a -> a |> Array.map (fun aa -> aa |> string) |> String.concat " "
        | :? IEnumerable as e -> e |> Seq.cast |> Seq.map (fun ea -> ea |> string) |> String.concat " "
        | _ -> failwith "Cannot convert arguments to a string!"

    /// Extensions for Shell context.
    type ICommandContext<'a> with

        [<CustomOperation("Shell")>]
        member this.Cli(context: ICommandContext<StartingContext>, shell) =
            Cli.shell shell context.Context.CurrentConfig

        [<CustomOperation("Command")>]
        member this.Command(context: ICommandContext<ShellContext>, command) = Cli.command command context.Context

        [<CustomOperation("WorkingDirectory")>]
        member this.WorkingDirectory(context: ICommandContext<ShellContext>, workingDirectory) =
            Cli.workingDirectory workingDirectory context.Context

        [<CustomOperation("EnvironmentVariable")>]
        member this.EnvironmentVariable(context: ICommandContext<ShellContext>, environmentVariable) =
            Cli.environmentVariables [ environmentVariable ] context.Context

        [<CustomOperation("EnvironmentVariables")>]
        member this.EnvironmentVariables(context: ICommandContext<ShellContext>, environmentVariables) =
            Cli.environmentVariables environmentVariables context.Context

        [<CustomOperation("Encoding")>]
        member this.Encoding(context: ICommandContext<ShellContext>, encoding) =
            Cli.encoding encoding context.Context

    /// Extensions for Exec context.
    type ICommandContext<'a> with

        [<CustomOperation("Exec")>]
        member this.Exec(context: ICommandContext<StartingContext>, program) =
            Program.program program context.Context.CurrentConfig

        [<CustomOperation("Arguments")>]
        member this.Arguments(context: ICommandContext<ExecContext>, arguments) =
            Program.arguments (matchArguments arguments) context.Context

        [<CustomOperation("WorkingDirectory")>]
        member this.WorkingDirectory(context: ICommandContext<ExecContext>, workingDirectory) =
            Program.workingDirectory workingDirectory context.Context

        [<CustomOperation("Verb")>]
        member this.Verb(context: ICommandContext<ExecContext>, verb) = Program.verb verb context.Context

        [<CustomOperation("Username")>]
        member this.UserName(context: ICommandContext<ExecContext>, userName) =
            Program.userName userName context.Context

        [<CustomOperation("Credentials")>]
        member this.Credentials(context: ICommandContext<ExecContext>, credentials) =
            let (domain, user, pw) = credentials in Program.credentials (Credentials(domain, user, pw)) context.Context

        [<CustomOperation("EnvironmentVariable")>]
        member this.EnvironmentVariable(context: ICommandContext<ExecContext>, environmentVariable) =
            Program.environmentVariables [ environmentVariable ] context.Context

        [<CustomOperation("EnvironmentVariables")>]
        member this.EnvironmentVariables(context: ICommandContext<ExecContext>, environmentVariables) =
            Program.environmentVariables environmentVariables context.Context

        [<CustomOperation("Encoding")>]
        member this.Encoding(context: ICommandContext<ExecContext>, encoding) =
            Program.encoding encoding context.Context