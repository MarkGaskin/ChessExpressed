module Server.DAL

open Shared

open LiteDB.FSharp
open LiteDB

type Storage () =
    let database =
        let mapper = FSharpBsonMapper()
        let connStr = "Filename=Todo.db;mode=Exclusive"
        new LiteDatabase (connStr, mapper)
    let chessGames = database.GetCollection<ChessGame> "chessGames"
    let chessPlayers = database.GetCollection<ChessPlayer> "chessPlayers"

    /// Retrieves all chessPlayers items.
    member _.GetChessPlayers () =
        chessPlayers.FindAll () |> List.ofSeq

    /// Tries to add a todo item to the collection.
    member _.AddChessPlayer (chessPlayer:ChessPlayer) =
        chessPlayers.Insert chessPlayer |> ignore
        Ok ()

    /// Retrieves all todo items.
    member _.GetChessGames () =
        chessGames.FindAll () |> List.ofSeq

    /// Tries to add a todo item to the collection.
    member _.AddChessGame (chessGame:ChessGame) =
        chessGames.Insert chessGame |> ignore
        Ok ()

let storage = Storage()

let todosApi =
    { getChessGames = fun () -> async { return storage.GetChessGames() }
      addChessGame =
        fun chessGame -> async {
            match storage.AddChessGame chessGame with
            | Ok () -> return chessGame
            | Error e -> return failwith e
        }
      getChessPlayers = fun () -> async { return storage.GetChessPlayers() }
      addChessPlayer =
          fun chessPlayer -> async {
              match storage.AddChessPlayer chessPlayer with
              | Ok () -> return chessPlayer
              | Error e -> return failwith e
          }
    }