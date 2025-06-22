module Simd.Main

[<EntryPoint>]
let entryPoint argv =
    try
        printfn "Hello, SIMD SSE World!"
        let a = [| 1.0f; 2.0f; 3.0f; 4.0f |]
        let b = [| 5.0f; 6.0f; 7.0f; 8.0f |]
        let result = Simd.Sse.sseAdd a b
        printfn "Result: %A" result

        0 // Return a zero exit code on success
    with ex ->
        eprintfn "An error occurred when starting the program: %s" ex.Message

        1 // Return a non-zero exit code on error
