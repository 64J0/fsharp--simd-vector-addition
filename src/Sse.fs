module Simd.Sse

open System
open System.Runtime.Intrinsics
open System.Runtime.Intrinsics.X86
open System.Runtime.InteropServices

let sseAdd (a: float32[]) (b: float32[]) =
    if not Sse.IsSupported then
        failwith "SSE not supported on this CPU"

    if a.Length <> b.Length then
        failwith "Arrays must have the same length"

    let len = a.Length
    let result = Array.zeroCreate<float32> len
    let simdWidth = Vector128<float32>.Count // 4 for SSE

    let spanA = a.AsSpan()
    let spanB = b.AsSpan()
    let spanR = result.AsSpan()

    let mutable i = 0

    while i <= len - simdWidth do
        // Get 4-element slices and cast them to bytes
        let va =
            MemoryMarshal.Cast<float32, Vector128<float32>>(spanA.Slice(i, simdWidth)).[0]

        let vb =
            MemoryMarshal.Cast<float32, Vector128<float32>>(spanB.Slice(i, simdWidth)).[0]

        let vsum = Sse.Add(va, vb)
        MemoryMarshal.Cast<float32, Vector128<float32>>(spanR.Slice(i, simdWidth)).[0] <- vsum
        i <- i + simdWidth

    // Fallback for leftovers
    for j in i .. len - 1 do
        result.[j] <- a.[j] + b.[j]

    result
