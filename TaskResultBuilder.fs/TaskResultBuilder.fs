namespace FSharp.Control.Tasks

open FSharp.Control.Tasks.V2
open System.Threading.Tasks


type TaskResult<'ok,'error> = Task<Result<'ok,'error>>

type TaskResultBuilder() =
    member __.Return(x) = Task.FromResult(Ok x)
    member __.Zero() = () |> Ok |> Task.FromResult

#nowarn "44"

module V2 =
    [<AutoOpen>]
    module ContextSensitive =
        type TaskResultBuilder with
            member __.Bind((taskResult: TaskResult<_,_>), continuation) =
                task {
                    let! result = taskResult
                    match result with
                    | Ok o -> return! continuation o
                    | Error e -> return e |> Error
                }
        let taskResult = TaskResultBuilder()

    [<AutoOpen>]
    module ContextInsensitive =
        // explicitly open the ContextInsentive TaskBuilder to ensure that's the one we use.
        open FSharp.Control.Tasks.V2.ContextInsensitive
        type TaskResultBuilder with
            member __.Bind((taskResult: TaskResult<_,_>), continuation) =
                task {
                    let! result = taskResult
                    match result with
                    | Ok o -> return! continuation o
                    | Error e -> return e |> Error
                }
        let taskResult = TaskResultBuilder()


