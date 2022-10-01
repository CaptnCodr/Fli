namespace Fli

open System.Net
open System.Security
open Program

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

    let private matchCredentials credentials = 
        match box (credentials) with
        | :? (string * string) as (user, password) -> NetworkCredential(user, password)
        | :? (string * SecureString) as (user, password) -> NetworkCredential(user, password)
        | :? (string * string * string) as (user, password, domain) -> NetworkCredential(user, password, domain)
        | :? (string * SecureString * string) as (user, password, domain) -> NetworkCredential(user, password, domain)
        | :? NetworkCredential as networkCredentials -> networkCredentials
        | _ -> failwith "Cannot identify credentials."

    let private matchArguments arguments = 
        match box (arguments) with
        | :? string as s -> s
        | :? seq<string> as s -> s |> Seq.map (fun sa -> sa |> string) |> String.concat " "
        | :? list<string> as l -> l |> List.map (fun la -> la |> string) |> String.concat " "
        | :? array<string> as a -> a |> Array.map (fun aa -> aa |> string) |> String.concat " "
        | :? IEnumerable as e -> e |> Seq.cast |> Seq.map (fun ea -> ea |> string) |> String.concat " "
        | _ -> failwith "Cannot convert arguments to a string!"

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
            
        [<CustomOperation("Credentials")>]
        member this.Credentials(context: ICommandContext<ShellContext>, credentials) =
            Cli.credentials (matchCredentials credentials) context.Context

    /// Extensions for Exec context
    type ICommandContext<'a> with

        [<CustomOperation("Exec")>]
        member this.Exec(context: ICommandContext<StartingContext>, program) =
            Program.program program context.Context.CurrentConfig

        [<CustomOperation("Arguments")>]
        member this.Arguments(context: ICommandContext<ProgramContext>, arguments) =
            Program.arguments (matchArguments arguments) context.Context

        [<CustomOperation("WorkingDirectory")>]
        member this.WorkingDirectory(context: ICommandContext<ProgramContext>, workingDirectory) =
            Program.workingDirectory workingDirectory context.Context

        [<CustomOperation("Credentials")>]
        member this.Credentials(context: ICommandContext<ProgramContext>, credentials) =
            Program.credentials (matchCredentials credentials) context.Context
