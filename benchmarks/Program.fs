open BenchmarkDotNet.Running
open SimdBenchmarks

[<EntryPoint>]
let main _ =
    BenchmarkRunner.Run<AddBenchmark>() |> ignore
    0
