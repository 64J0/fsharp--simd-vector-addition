module Tests

open Expecto
open Simd.Sse

[<Tests>]
let tests =
    testList
        "SSE SIMD Addition (Safe) Tests"
        [

          test "Correct result with 4 elements (SIMD width)" {
              let a = [| 1.0f; 2.0f; 3.0f; 4.0f |]
              let b = [| 0.5f; 0.5f; 0.5f; 0.5f |]
              let expected = Array.map2 (+) a b
              let actual = sseAdd a b
              Expect.equal actual expected "Should add correctly using SSE"
          }

          test "Correct result with length not divisible by 4" {
              let a = [| 10.0f; 20.0f; 30.0f; 40.0f; 50.0f |]
              let b = [| 1.0f; 2.0f; 3.0f; 4.0f; 5.0f |]
              let expected = Array.map2 (+) a b
              let actual = sseAdd a b
              Expect.sequenceEqual actual expected "Should handle extra elements with scalar fallback"
          }

          test "Throws on different-sized arrays" {
              let a = [| 1.0f; 2.0f; 3.0f |]
              let b = [| 1.0f; 2.0f |]
              Expect.throws (fun () -> sseAdd a b |> ignore) "Should fail on array length mismatch"
          }

          test "Throws if SSE is not supported (simulated)" {
              if not System.Runtime.Intrinsics.X86.Sse.IsSupported then
                  let a = [| 1.0f; 2.0f |]
                  let b = [| 1.0f; 2.0f |]
                  Expect.throws (fun () -> sseAdd a b |> ignore) "Should throw if SSE is unavailable"
          } ]
