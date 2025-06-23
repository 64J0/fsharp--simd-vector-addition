module Simd.Sse

open System
open System.Numerics
open System.Runtime.Intrinsics
open System.Runtime.Intrinsics.X86
open System.Runtime.InteropServices

/// System.Numerics.Vector<T> is part of .NET's hardware-accelerated numerics
/// API, introduced to allow portable SIMD code.
///
/// Think of it as "high-level SIMD" — you're asking for vector math, and the
/// runtime decides how best to do it. So we have less control over the
/// intrinsics, but we gain portability and ease of use.
///
/// This is a good option if you want to write SIMD code that works on both x86
/// and ARM architectures, as well as on platforms that may not support SSE or
/// AVX directly.
///
/// The Vector<T> type is designed to abstract away the underlying SIMD
/// instructions and provide a consistent interface for vector operations across
/// different hardware architectures. It uses the best available SIMD
/// instructions for the current platform, which may include SSE, AVX, or NEON,
/// depending on the target architecture.
let simdAddGeneric (a: float32[]) (b: float32[]) =
    if a.Length <> b.Length then
        failwith "Arrays must have the same length"

    let len = a.Length
    let result = Array.zeroCreate<float32> len
    let simdWidth = Vector<float32>.Count

    let mutable i = 0

    while i <= len - simdWidth do
        let va = Vector<float32>(a, i)
        let vb = Vector<float32>(b, i)
        (va + vb).CopyTo(result, i)
        i <- i + simdWidth

    for j in i .. len - 1 do
        result.[j] <- a.[j] + b.[j]

    result

/// Specific intrinsics for SSE (Streaming SIMD Extensions) using Span.
let sseAdd (a: float32[]) (b: float32[]) =
    if not Sse.IsSupported then
        failwith "SSE not supported on this CPU"

    if a.Length <> b.Length then
        failwith "Arrays must have the same length"

    let len = a.Length
    let result = Array.zeroCreate<float32> len
    let simdWidth = Vector128<float32>.Count // 4 for SSE

    // Span<T> provides a type-safe and memory-safe representation of a
    // contiguous region of arbitrary memory.
    //
    // At first it seems similar to using arrays, but it allows for more
    // efficient memory access patterns.
    //
    // More information:
    // https://learn.microsoft.com/en-us/archive/msdn-magazine/2018/january/csharp-all-about-span-exploring-a-new-net-mainstay
    let spanA = a.AsSpan()
    let spanB = b.AsSpan()
    let spanR = result.AsSpan()

    let mutable i = 0

    while i <= len - simdWidth do
        // This MemoryMarshal.Cast is used to convert the Span<float32> to a
        // Span<Vector128<float32>>.
        //
        // A Span<float32> is a contiguous region of memory that can be used to
        // represent an array or a portion of an array. Since it has this
        // float32 type, our program knows that each element takes 4 bytes, and
        // with this information it knows how to slice it properly.
        let va =
            MemoryMarshal.Cast<float32, Vector128<float32>>(spanA.Slice(i, simdWidth)).[0]
        // val va: Vector128<float32> = <a_{i}, a_{i+1}, a_{i+2}, a_{i+3}>

        let vb =
            MemoryMarshal.Cast<float32, Vector128<float32>>(spanB.Slice(i, simdWidth)).[0]
        // val vb: Vector128<float32> = <b_{i}, b_{i+1}, b_{i+2}, b_{i+3}>

        let vsum = Sse.Add(va, vb)
        MemoryMarshal.Cast<float32, Vector128<float32>>(spanR.Slice(i, simdWidth)).[0] <- vsum
        i <- i + simdWidth

    // Fallback for leftovers
    for j in i .. len - 1 do
        result.[j] <- a.[j] + b.[j]

    result
