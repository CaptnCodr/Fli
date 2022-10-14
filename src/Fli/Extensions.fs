namespace Fli

module Extensions =
    open System.Diagnostics

    type ProcessStartInfo with

        member x.With() = x
