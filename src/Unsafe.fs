module Simd.Unsafe

open System
open System.Numerics
open Microsoft.FSharp.NativeInterop

#nowarn "9"

module private Span =
    let inline vmap2< ^T when ^T: unmanaged>
        ([<InlineIfLambda>] vmap)
        ([<InlineIfLambda>] map)
        (lhs: Span< ^T >)
        (rhs: Span< ^T >)
        (dst: Span< ^T >)
        =
        assert (lhs.Length = rhs.Length && lhs.Length = dst.Length)

        if lhs.Length < Vector< ^T>.Count then
            for i in 0 .. lhs.Length - 1 do
                dst[i] <- map lhs[i] rhs[i]
        else

            use lptr = fixed (&lhs.GetPinnableReference())
            use rptr = fixed (&rhs.GetPinnableReference())
            use dptr = fixed (&dst.GetPinnableReference())

            let mutable offset = 0
            let last = lhs.Length - Vector< ^T>.Count

            while offset <= last do
                let lvec = Vector.Load(NativePtr.add lptr offset)
                let rvec = Vector.Load(NativePtr.add rptr offset)
                Vector.Store(vmap lvec rvec, NativePtr.add dptr offset)
                offset <- offset + Vector< ^T>.Count

            let lvend = NativePtr.add lptr (lhs.Length - Vector< ^T>.Count)
            let rvend = NativePtr.add rptr (rhs.Length - Vector< ^T>.Count)
            let dvend = NativePtr.add dptr (dst.Length - Vector< ^T>.Count)
            Vector.Store(vmap (Vector.Load lvend) (Vector.Load rvend), dvend)

module private Array =
    let inline vmap2< ^T when ^T: unmanaged>
        ([<InlineIfLambda>] vmap)
        ([<InlineIfLambda>] map)
        (a: array< ^T >)
        (b: array< ^T >)
        =

        if a.Length <> b.Length then
            raise (ArgumentOutOfRangeException "Arrays must have the same length")

        let dst = Array.zeroCreate< ^T> a.Length
        dst.Length |> ignore // sigh
        Span.vmap2 vmap map (Span a) (Span b) (Span dst)
        dst

let unsafeAdd (a: float32[]) (b: float32[]) = (a, b) ||> Array.vmap2 (+) (+)
