open BenchmarkDotNet.Running
open SimdBenchmarks

[<EntryPoint>]
let main _ =
    BenchmarkRunner.Run<AddTwoArraysBenchmark>() |> ignore
    0
