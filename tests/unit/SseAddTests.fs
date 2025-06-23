module Tests

open Expecto
open Simd.Sse

[<Tests>]
let tests =
    testList
        "SIMD Addition (Safe) Tests"
        [

          test "SPECIFIC :: Correct result with 4 elements (SIMD width)" {
              let a = [| 1.0f; 2.0f; 3.0f; 4.0f |]
              let b = [| 0.5f; 0.5f; 0.5f; 0.5f |]
              let expected = Array.map2 (+) a b
              let actual = sseAdd a b
              Expect.equal actual expected "Should add correctly using SSE"
          }

          test "GENERIC :: Correct result with 4 elements (SIMD width)" {
              let a = [| 1.0f; 2.0f; 3.0f; 4.0f |]
              let b = [| 0.5f; 0.5f; 0.5f; 0.5f |]
              let expected = Array.map2 (+) a b
              let actual = simdAddGeneric a b
              Expect.equal actual expected "Should add correctly using generic SIMD"
          }

          test "SPECIFIC :: Correct result with length not divisible by 4" {
              let a = [| 10.0f; 20.0f; 30.0f; 40.0f; 50.0f |]
              let b = [| 1.0f; 2.0f; 3.0f; 4.0f; 5.0f |]
              let expected = Array.map2 (+) a b
              let actual = sseAdd a b
              Expect.sequenceEqual actual expected "Should handle extra elements with scalar fallback"
          }

          test "GENERIC :: Correct result with length not divisible by 4" {
              let a = [| 10.0f; 20.0f; 30.0f; 40.0f; 50.0f |]
              let b = [| 1.0f; 2.0f; 3.0f; 4.0f; 5.0f |]
              let expected = Array.map2 (+) a b
              let actual = simdAddGeneric a b
              Expect.sequenceEqual actual expected "Should handle extra elements with scalar fallback"
          }

          test "SPECIFIC :: Throws on different-sized arrays" {
              let a = [| 1.0f; 2.0f; 3.0f |]
              let b = [| 1.0f; 2.0f |]
              Expect.throws (fun () -> sseAdd a b |> ignore) "Should fail on array length mismatch"
          }

          test "GENERIC :: Throws on different-sized arrays" {
              let a = [| 1.0f; 2.0f; 3.0f |]
              let b = [| 1.0f; 2.0f |]
              Expect.throws (fun () -> simdAddGeneric a b |> ignore) "Should fail on array length mismatch"
          }

          test "SPECIFIC :: Throws if SSE is not supported (simulated)" {
              if not System.Runtime.Intrinsics.X86.Sse.IsSupported then
                  let a = [| 1.0f; 2.0f |]
                  let b = [| 1.0f; 2.0f |]
                  Expect.throws (fun () -> sseAdd a b |> ignore) "Should throw if SSE is unavailable"
          } ]
