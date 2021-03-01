module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn

open Shared

open LiteDB.FSharp
open LiteDB

type Storage () =
    let database =
        let mapper = FSharpBsonMapper()
        let connStr = "Filename=Todo.db;mode=Exclusive"
        new LiteDatabase (connStr, mapper)
    let todos = database.GetCollection<ChessGame> "todos"

    /// Retrieves all todo items.
    member _.GetTodos () =
        todos.FindAll () |> List.ofSeq

    /// Tries to add a todo item to the collection.
    member _.AddChessGame (chessGame:ChessGame) =
        todos.Insert chessGame |> ignore
        Ok ()

let storage = Storage()

let todosApi =
    { getChessGames = fun () -> async { return storage.GetTodos() }
      addChessGame =
        fun todo -> async {
            match storage.AddChessGame todo with
            | Ok () -> return todo
            | Error e -> return failwith e
        } }

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue todosApi
    |> Remoting.buildHttpHandler

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
