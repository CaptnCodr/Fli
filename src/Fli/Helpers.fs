namespace Fli

#nowarn "9"

open System.Runtime.InteropServices
open System.Security

module Helpers =

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
