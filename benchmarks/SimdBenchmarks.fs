module SimdBenchmarks

open BenchmarkDotNet.Attributes
open System
open Simd.Sse

let private scalarAdd (a: float32[]) (b: float32[]) =
    Array.map2 (+) a b

type AddBenchmark() =

    let size = 1_000_000
    let mutable a = [||]
    let mutable b = [||]

    [<GlobalSetup>]
    member _.Setup() =
        let rand = Random 42
        a <- Array.init size (fun _ -> float32 (rand.NextDouble()))
        b <- Array.init size (fun _ -> float32 (rand.NextDouble()))

    [<Benchmark(Baseline = true)>]
    member _.ScalarAdd() =
        scalarAdd a b

    [<Benchmark>]
    member _.SimdAdd() =
        sseAdd a b