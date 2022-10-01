namespace Fli

[<AutoOpen>]
module Command =

    open Domain
    open System
    open System.Diagnostics

    let private shellToProcess =
        function
        | CMD -> "cmd.exe", "/C"
        | PS -> "powershell.exe", "-Command"
        | PWSH -> "pwsh.exe", "-Command"
        | BASH -> "bash", "-c"

    let private createProcess executable argumentString workingDirectory verb =
        ProcessStartInfo(
            FileName = executable,
            Verb = (verb |> Option.defaultValue null),
            Arguments = argumentString,
            WorkingDirectory = (workingDirectory |> Option.defaultValue ""),
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true
        )

    let private startProcess (psi: ProcessStartInfo) =
        Process.Start(psi).StandardOutput.ReadToEnd()

    let private checkVerb (verb: string) (executable: string) =
        let verbs = ProcessStartInfo(executable).Verbs

        if not (verbs |> Array.contains verb) then
            $"""Unknown verb '{verb}'. Possible verbs on '{executable}': {verbs |> String.concat ", "}."""
            |> ArgumentException
            |> raise

    type Command =
        static member execute(context: ShellContext) =
            let (proc, flag) = context.config.Shell |> shellToProcess

            (createProcess proc $"{flag} {context.config.Command}" context.config.WorkingDirectory None)
            |> startProcess

        static member toString(context: ShellContext) =
            let (proc, flag) = context.config.Shell |> shellToProcess
            $"{proc} {flag} {context.config.Command}"

        static member execute(context: ProgramContext) =
            match context.config.Verb with
            | Some (verb) -> checkVerb verb context.config.Program
            | None -> ()

            (createProcess
                context.config.Program
                context.config.Arguments
                context.config.WorkingDirectory
                context.config.Verb)
            |> startProcess

        static member toString(context: ProgramContext) =
            $"{context.config.Program} {context.config.Arguments}"
