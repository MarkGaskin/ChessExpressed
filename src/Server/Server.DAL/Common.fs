module Server.DAL

open Shared

open LiteDB.FSharp
open LiteDB

type Storage () =
    let database =
        let mapper = FSharpBsonMapper()
        let connStr = "Filename=ChessExpressed.db;mode=Exclusive"
        new LiteDatabase (connStr, mapper)
    let chessGames = database.GetCollection<ChessGame> "chessGames"
    let chessPlayers = database.GetCollection<ChessPlayer> "chessPlayers"

    /// Retrieves all chessPlayers items.
    member _.GetChessPlayers () =
        chessPlayers.FindAll () |> List.ofSeq

    /// Tries to add a chess player item to the collection.
    member _.AddChessPlayer (chessPlayer:ChessPlayer) =
        chessPlayers.Insert chessPlayer |> ignore
        Ok ()

    /// Tries to delete a chess player item from the collection.
    member _.DeleteChessPlayer (chessPlayer:ChessPlayer) =
        chessPlayers.Delete (BsonValue(chessPlayer.Id)) |> ignore
        Ok ()

    /// Tries to update a chess player item in the collection.
    member _.UpdateChessPlayer (chessPlayer:ChessPlayer) =
        chessPlayers.Update chessPlayer |> ignore
        Ok ()

    /// Retrieves all chess game items.
    member _.GetChessGames () =
        chessGames.FindAll () |> List.ofSeq

    /// Tries to add a chess game item to the collection.
    member _.AddChessGame (chessGame:ChessGame) =
        chessGames.Insert chessGame |> ignore
        Ok ()

    /// Tries to add a chess game item to the collection.
    member _.DeleteChessGame (chessGame:ChessGame) =
        chessGames.Delete (BsonValue(chessGame.Id)) |> ignore
        Ok ()

    /// Tries to update a chess game item to the collection.
    member _.UpdateChessGame (chessGame:ChessGame) =
        chessGames.Update chessGame |> ignore
        Ok ()

let storage = Storage()

let CEApi =
    { getChessGames = fun () -> async { return storage.GetChessGames() }
      addChessGame =
        fun chessGame -> async {
            match storage.AddChessGame chessGame with
            | Ok () -> return chessGame
            | Error e -> return failwith e
        }
      deleteChessGame =
          fun chessGame -> async {
              match storage.DeleteChessGame chessGame with
              Ok () -> return chessGame
              | Error e -> return failwith e }
      updateChessGame =
          fun chessGame -> async {
              match storage.UpdateChessGame chessGame with
              Ok () -> return chessGame
              | Error e -> return failwith e }
      getChessPlayers = fun () -> async { return storage.GetChessPlayers() }
      addChessPlayer =
          fun chessPlayer -> async {
              match storage.AddChessPlayer chessPlayer with
              | Ok () -> return chessPlayer
              | Error e -> return failwith e
          }
      deleteChessPlayer =
          fun chessPlayer -> async {
              match storage.DeleteChessPlayer chessPlayer with
              Ok () -> return chessPlayer
              | Error e -> return failwith e }
      updateChessPlayer =
          fun chessPlayer -> async {
              match storage.UpdateChessPlayer chessPlayer with
              | Ok () -> return chessPlayer
              | Error e -> return failwith e
          }
    }