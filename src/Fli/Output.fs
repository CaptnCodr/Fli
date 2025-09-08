namespace Fli

module Output =

    /// Gets `Id` from `Output`.
    let toId (output: Output) = output.Id

    /// Prints `Id` from `Output`.
    let printId = toId >> printfn "%i"

    /// Gets `Text` from `Output`.
    let toText (output: Output) = output.Text |> Option.defaultValue ""

    /// Prints `Text` from `Output`.
    let printText = toText >> printfn "%s"

    /// Gets `ExitCode` from `Output`.
    let toExitCode (output: Output) = output.ExitCode

    /// Prints `ExitCode` from `Output`.
    let printExitCode = toExitCode >> printfn "%i"

    /// Gets `Error` from `Output`.
    let toError (output: Output) = output.Error |> Option.defaultValue ""

    /// Prints `Error` from `Output`.
    let printError = toError >> printfn "%s"

    /// Throws exception if given condition in `func` matches.
    let throw (func: Output -> bool) (output: Output) =
        if output |> func then
            failwith $"Execution failed with exit code {output.ExitCode} {output.Error}"

        output

    /// Throws exception if exit code is not 0.
    let throwIfErrored = throw (fun o -> o.ExitCode <> 0)

    let from (str: string) (output: Output): Output =
        if output.Text = None |> not || output.Error = None |> not then
            output
        elif output.ExitCode = 0 then
            { Id = output.Id
              Text = Some(str)
              ExitCode = output.ExitCode
              Error = None }
        else
            { Id = output.Id
              Text = None
              ExitCode = output.ExitCode
              Error = Some(str) }