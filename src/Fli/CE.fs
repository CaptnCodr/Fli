namespace Fli

[<AutoOpen>]
module CE =

    open System.Text
    open Domain

    type ICommandContext<'a> with

        member this.Yield(_) = this

    type StartingContext =
        { config: Config option }

        member this.CurrentConfig = this.config |> Option.defaultValue Defaults

        interface ICommandContext<StartingContext> with
            member this.Context = this

    let cli = { config = None }

    /// Extensions for Shell context.
    type ICommandContext<'a> with

        /// `Shell` opening keyword, which `Fli.Shell` shall be used.
        [<CustomOperation("Shell")>]
        member _.Shell(context: ICommandContext<StartingContext>, shell) =
            Cli.shell shell context.Context.CurrentConfig

        /// Which `Command` should be executed in the `Shell`.
        [<CustomOperation("Command")>]
        member _.Command(context: ICommandContext<ShellContext>, command) = Cli.command command context.Context

        /// `Input` string(s) that can be used to interact with the shell.
        [<CustomOperation("Input")>]
        member _.Input(context: ICommandContext<ShellContext>, input) = Cli.input input context.Context

        /// Extra `Output` that is being executed immediately after getting output from execution.
        [<CustomOperation("Output")>]
        member _.Output(context: ICommandContext<ShellContext>, filePath: string) =
            Cli.output (File filePath) context.Context

        /// Extra `Output` that is being executed immediately after getting output from execution.
        [<CustomOperation("Output")>]
        member _.Output(context: ICommandContext<ShellContext>, stringBuilder: StringBuilder) =
            Cli.output (StringBuilder stringBuilder) context.Context

        /// Extra `Output` that is being executed immediately after getting output from execution.
        [<CustomOperation("Output")>]
        member _.Output(context: ICommandContext<ShellContext>, func: string -> unit) =
            Cli.output (Custom func) context.Context

        /// Current executing `working directory`.
        [<CustomOperation("WorkingDirectory")>]
        member _.WorkingDirectory(context: ICommandContext<ShellContext>, workingDirectory) =
            Cli.workingDirectory workingDirectory context.Context

        /// One tupled `EnvironmentVariable`.
        [<CustomOperation("EnvironmentVariable")>]
        member _.EnvironmentVariable(context: ICommandContext<ShellContext>, environmentVariable) =
            Cli.environmentVariables [ environmentVariable ] context.Context

        /// A list of tupled `EnvironmentVariables`.
        [<CustomOperation("EnvironmentVariables")>]
        member _.EnvironmentVariables(context: ICommandContext<ShellContext>, environmentVariables) =
            Cli.environmentVariables environmentVariables context.Context

        /// `Encoding` that'll be used for `Input`, 'Output.Text' and `Output.Error`.
        [<CustomOperation("Encoding")>]
        member _.Encoding(context: ICommandContext<ShellContext>, encoding) = Cli.encoding encoding context.Context

    /// Extensions for Exec context.
    type ICommandContext<'a> with

        /// `Exec` opening keyword, which binary/executable shall be started.
        [<CustomOperation("Exec")>]
        member _.Exec(context: ICommandContext<StartingContext>, program) =
            Program.program program context.Context.CurrentConfig

        /// `Arguments` that will be passed into the executable.
        [<CustomOperation("Arguments")>]
        member _.Arguments(context: ICommandContext<ExecContext>, arguments) =
            let matchArguments arguments =
                match box (arguments) with
                | :? string as s -> s
                | :? seq<string> as s -> s |> Seq.map string |> String.concat " "
                | _ -> failwith "Cannot convert arguments to a string!"

            Program.arguments (matchArguments arguments) context.Context

        /// `Input` string(s) that can be used to interact with the executable.
        [<CustomOperation("Input")>]
        member _.Input(context: ICommandContext<ExecContext>, input) = Program.input input context.Context

        /// Extra `Output` that is being executed immediately after getting output from execution.
        [<CustomOperation("Output")>]
        member _.Output(context: ICommandContext<ExecContext>, filePath: string) =
            Program.output (File filePath) context.Context

        /// Extra `Output` that is being executed immediately after getting output from execution.
        [<CustomOperation("Output")>]
        member _.Output(context: ICommandContext<ExecContext>, stringBuilder: StringBuilder) =
            Program.output (StringBuilder stringBuilder) context.Context

        /// Extra `Output` that is being executed immediately after getting output from execution.
        [<CustomOperation("Output")>]
        member _.Output(context: ICommandContext<ExecContext>, func: string -> unit) =
            Program.output (Custom func) context.Context

        /// Current executing `working directory`.
        [<CustomOperation("WorkingDirectory")>]
        member _.WorkingDirectory(context: ICommandContext<ExecContext>, workingDirectory) =
            Program.workingDirectory workingDirectory context.Context

        /// `Verb` keyword that can be used to start the executable.
        [<CustomOperation("Verb")>]
        member _.Verb(context: ICommandContext<ExecContext>, verb) = Program.verb verb context.Context

        /// Start the executable with another `Username`.
        [<CustomOperation("Username")>]
        member _.UserName(context: ICommandContext<ExecContext>, userName) =
            Program.userName userName context.Context

        /// Start the executable with other `Credentials`.
        /// Hint: `Domain` and `Password` are available for Windows systems only.
        [<CustomOperation("Credentials")>]
        member _.Credentials(context: ICommandContext<ExecContext>, credentials) =
            let (domain, user, pw) = credentials in Program.credentials (Credentials(domain, user, pw)) context.Context

        /// One tupled `EnvironmentVariable`.
        [<CustomOperation("EnvironmentVariable")>]
        member _.EnvironmentVariable(context: ICommandContext<ExecContext>, environmentVariable) =
            Program.environmentVariables [ environmentVariable ] context.Context

        /// A list of tupled `EnvironmentVariables`.
        [<CustomOperation("EnvironmentVariables")>]
        member _.EnvironmentVariables(context: ICommandContext<ExecContext>, environmentVariables) =
            Program.environmentVariables environmentVariables context.Context

        /// `Encoding` that'll be used for `Input`, 'Output.Text' and `Output.Error`.
        [<CustomOperation("Encoding")>]
        member _.Encoding(context: ICommandContext<ExecContext>, encoding) =
            Program.encoding encoding context.Context
