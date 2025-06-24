module SimdBenchmarks

open BenchmarkDotNet.Attributes
open System

[<MemoryDiagnoser>]
type AddTwoArraysBenchmark() =

    [<Params(100, 10_000, 1_000_000)>]
    member val Length: int = 0 with get, set

    member val a: float32[] = [||] with get, set
    member val b: float32[] = [||] with get, set

    [<GlobalSetup>]
    member self.Setup() =
        let rng = Random(42)
        self.a <- Array.init self.Length (fun _ -> float32 (rng.NextDouble()))
        self.b <- Array.init self.Length (fun _ -> float32 (rng.NextDouble()))

    [<Benchmark(Baseline = true)>]
    member self.ScalarAdd() = Array.map2 (+) self.a self.b

    [<Benchmark>]
    member self.SimdAddGeneric() = Simd.Sse.simdAddGeneric self.a self.b

    [<Benchmark>]
    member self.SimdAdd() = Simd.Sse.sseAdd self.a self.b

    [<Benchmark>]
    member self.SimdUnsafe() = Simd.Unsafe.unsafeAdd self.a self.b
