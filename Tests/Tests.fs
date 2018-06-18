module Tests


open FSharp.Control.Tasks.V2.ContextInsensitive
open System.Threading.Tasks
open Xunit

let okResult ok = ok |> Ok |> Task.FromResult
let errorResult error = error |> Error |> Task.FromResult

[<Fact>]
let ``Successful task result should be successful`` () =
    let expected = okResult 11
    let actual =
        taskResult {
            let! five = okResult 5
            let! six = okResult 6
            return five + six
        }
    let areEqual = expected.Result = actual.Result
    Assert.True areEqual

[<Fact>]
let ``Unsuccesful task result should be Unsuccesful`` () =
    let expected = Error "not six"
    let actual =
        taskResult {
            let! five =  okResult 5
            let! error = errorResult "not six"
            return five + error
        }
    let areEqual = expected = actual.Result
    Assert.True(areEqual)