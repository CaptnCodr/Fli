namespace Fli

#nowarn "9"

module Helpers =

    open System.Runtime.InteropServices
    open System.Security

    let toSecureString (unsureString: string) =
        if isNull unsureString then
            raise <| System.ArgumentNullException("s")

        let gcHandle = GCHandle.Alloc(unsureString, GCHandleType.Pinned)

        try
            let secureString =
                new SecureString(
                    NativeInterop.NativePtr.ofNativeInt (gcHandle.AddrOfPinnedObject()),
                    unsureString.Length
                )

            secureString.MakeReadOnly()
            secureString
        finally
            gcHandle.Free()

    let toOption =
        function
        | null
        | "" -> None
        | s -> Some s

    let toOptionWithDefault defaultValue value =
        match value with
        | null
        | "" -> None
        | _ -> Some defaultValue
