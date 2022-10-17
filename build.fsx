#r "nuget: Fli"

open Fli

// Empty output directory
cli {
    Shell PWSH
    Command "Remove-Item * -Include *.nupkg"
    WorkingDirectory "src/.nupkg/"
}
|> Command.execute
|> Output.toText
|> printfn "%s"

// Build in Release configuration
cli {
    Exec "dotnet"
    Arguments [| "build"; "-c"; "Release" |]
    WorkingDirectory "src/"
}
|> Command.execute
|> Output.toText
|> printfn "%s"

// Pack Nuget Package
cli {
    Exec "paket"
    Arguments [| "pack"; ".nupkg" |]
    WorkingDirectory "src/"
}
|> Command.execute
|> Output.toText
|> printfn "%s"

// Push Nuget Package
cli {
    Exec "paket"
    Arguments [| "push"; ".nupkg" |]
    WorkingDirectory "src/"
}
|> Command.execute
|> Output.toText
|> printfn "%s"