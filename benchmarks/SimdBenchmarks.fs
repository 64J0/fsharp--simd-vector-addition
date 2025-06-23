module SimdBenchmarks

open BenchmarkDotNet.Attributes
open System

let private scalarAdd (a: float32[]) (b: float32[]) = Array.map2 (+) a b

let private getRandomNumber () =
    let rand = Random 42
    float32 (rand.NextDouble())

[<MemoryDiagnoser>]
type AddBenchmark() =

    [<Params(10_000, 1_000_000, 100_000_000)>]
    member val size: int = 0 with get, set

    member self.a: float32[] = Array.init self.size (fun _ -> getRandomNumber ())
    member self.b: float32[] = Array.init self.size (fun _ -> getRandomNumber ())

    [<Benchmark(Baseline = true)>]
    member self.ScalarAdd() = scalarAdd self.a self.b

    [<Benchmark>]
    member self.SimdAdd() = Simd.Sse.sseAdd self.a self.b
