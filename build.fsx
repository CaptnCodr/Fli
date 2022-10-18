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
    Shell CMD
    Command "paket pack .nupkg"
    WorkingDirectory "src/"
}
|> Command.execute
|> Output.toText
|> printfn "%s"

// Push Nuget Package
cli {
    Shell CMD
    Command "paket push Fli.0.9.0.0.nupkg"
    WorkingDirectory "src/.nupkg/"
}
|> Command.execute
|> Output.toText
|> printfn "%s"
