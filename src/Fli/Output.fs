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
