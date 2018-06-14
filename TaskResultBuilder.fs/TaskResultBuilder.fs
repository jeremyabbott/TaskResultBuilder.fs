namespace FSharp.Control.Tasks

open FSharp.Control.Tasks.V2
open System.Threading.Tasks

module TaskResult =
    type TaskResult<'ok,'error> = Task<Result<'ok,'error>>

module Helpers =
    open TaskResult
    let bind (onOk: 'a -> TaskResult<'b,'c>) (tr: TaskResult<'a,'c>) : TaskResult<'b,'c> =
        task {
            let! result = tr
            match result with
            | Ok o -> return! onOk o
            | Error e -> return e |> Error
        }

module Operators =
    open Helpers
    let (>>=) taskResult onOk =
        bind onOk taskResult

type TaskResultBuilder() =
    member __.Bind(resultTask : Result<'ok, 'err> Task, onOk : 'ok -> Task<Result<'out, 'err>>) =
          task {
              let! maybe = resultTask
              match maybe with
              | Ok yep -> return! onOk yep
              | Error e -> return Error e
          }
    member __.Return(x) = Task.FromResult(Ok x)
    member __.Zero() = () |> Ok |> Task.FromResult

[<AutoOpen>]
module ContextSensitive =
    let taskResult = TaskResultBuilder()

module Test =
    open Operators
    open TaskResult
    let a: TaskResult<int, string> = task { return Ok 5}
    let b: TaskResult<int, string> = task { return Ok 6}

    let add5 x: TaskResult<int, string> = task { return x + 5 |> Ok}

    let test x = add5 x >>= add5


