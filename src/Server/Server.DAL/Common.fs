module Server.DAL.Common

open Shared
open Shared.CEError

open LiteDB.FSharp
open LiteDB
open Server.DAL.PgnImport
open System
open System.IO


type Storage () =
    let database =
        let mapper = FSharpBsonMapper()
        let connStr = "Filename=ChessExpressed.db; mode=exclusive"
        new LiteDatabase (connStr, mapper)
    let chessGames = database.GetCollection<ChessGame> "chessGames"
    let chessPlayers = database.GetCollection<ChessPlayer> "chessPlayers"
    let ecos = database.GetCollection<ECO> "eco"
    
    /// Retrieves all eco items.
    member _.GetECOs () =
        ecos.FindAll () |> List.ofSeq
    
    /// Tries to add eco items to the collection.
    member _.UpsertECOs (eco:seq<ECO>) =
        try
            ecos.Insert eco |> (fun i -> sprintf "ECOsAdded_%i" i |> (fun string -> Path.Combine("C:\\ECO\\", string)) |> File.Create ) |> ignore
            Ok ()
        with _ ->
            Error FailedToUpdateEcosInDB

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
        chessPlayers.Update chessPlayer |> Ok

    /// Retrieves all chess game items.
    member _.GetChessGames () =
        chessGames.FindAll () |> List.ofSeq

    /// Tries to add a chess game item to the collection.
    member _.AddChessGame (chessGame:ChessGame) =
        chessGames.Insert chessGame |> ignore
        Ok ()

    /// Tries to add a chess game item to the collection.
    member _.DeleteChessGame (chessGame:ChessGame) =
        chessGames.Delete (BsonValue(chessGame.Id)) |> Ok

    /// Tries to update a chess game item to the collection.
    member _.UpdateChessGame (chessGame:ChessGame) =
        chessGames.Update chessGame |> Ok

let storage = Storage()


let txtFilePath = Path.Combine[|"C:";"ECO";"eco.txt"|]
let dbFilePath = Path.Combine[|"C:"; "Users"; "markr"; "git-repos"; "ChessExpressed"; "src"; "Server"; "Server.DAL"|]

let updateEcos () =
    async {
        return result{
            try
                let chunkedArray =
                    File.ReadLines txtFilePath
                    |> Seq.chunkBySize 2

                let resultArray =
                    chunkedArray
                    |> Seq.map
                         (fun (lineArray: string[]) ->
                                try 
                                     { Id = Guid.NewGuid()
                                       Eco = Array.tryItem 0 lineArray |> Option.map (fun string -> string.Substring(0,3)) |> Option.defaultValue ""
                                       Name = Array.tryItem 0 lineArray |> Option.map (fun string -> string.Substring(4)) |> Option.defaultValue ""
                                       Moves = Array.tryItem 1 lineArray |> Option.defaultValue "" } |> Ok
                                with _ ->
                                    Error FailedToCreateEcosForUpsert )
                     
                let! ecoArray =
                    if Seq.forall (Result.isOk) resultArray then
                        resultArray |> Seq.map (fun res -> match res with
                                                           | Ok eco -> eco
                                                           | Error _ -> ECO.defaultEco) |> Ok
                    else
                        Error FailedToCreateEcosForUpsert
                         
                return! ecoArray
                        |> storage.UpsertECOs
            with _ ->
                return! Error FailedToUpdateEcos
        }
   }

let getEcoFromId id =
    async{
        return storage.GetECOs ()
               |> List.tryFind (fun eco -> eco.Eco = id)
               |> function
                  | Some eco -> Ok eco
                  | None -> Error FailedToMatchEcoId
    }    
    
let getEcoFromMoves moves =
    async{
        return storage.GetECOs ()
               |> List.tryFind (fun eco -> eco.Moves = moves)
               |> function
                  | Some eco -> Ok eco
                  | None -> Error FailedToMatchEcoMoves
    }    

let ecoApi =
    { UpdateECOs = updateEcos
      GetECOFromID = getEcoFromId
      GetECOFromMoves = getEcoFromMoves }

let chessGameApi : ChessGameApi =
    {  getChessGames = fun () -> async { return storage.GetChessGames() }
       addChessGame =
            fun chessGame -> async {
                match storage.AddChessGame chessGame with
                | Ok () -> return chessGame
                | Error e -> return failwith e
            }
       deleteChessGame =
            fun chessGame -> async {
                match storage.DeleteChessGame chessGame with
                | Ok true -> return chessGame
                | Ok false -> return failwith "Failed to delete chess game"
                | Error e -> return failwith e }
       updateChessGame =
            fun chessGame -> async {
                match storage.UpdateChessGame chessGame with
                | Ok true-> return chessGame
                | Ok false -> return failwith "Failed to update chess game"
                | Error e -> return failwith e } }

let chessPlayerApi : ChessPlayerApi =
    { 
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
              | Ok true -> return chessPlayer
              | Ok false -> return failwith "Failed to update chess player"
              | Error e -> return failwith e
          }
    }
    

let CEApi =
    { getChessPlayers = chessPlayerApi.getChessPlayers
      addChessPlayer = chessPlayerApi.addChessPlayer
      deleteChessPlayer = chessPlayerApi.deleteChessPlayer
      updateChessPlayer = chessPlayerApi.updateChessPlayer
      getChessGames = chessGameApi.getChessGames
      addChessGame = chessGameApi.addChessGame
      deleteChessGame = chessGameApi.deleteChessGame
      updateChessGame = chessGameApi.updateChessGame
      UpdateECOs = ecoApi.UpdateECOs
      GetECOFromID = ecoApi.GetECOFromID
      GetECOFromMoves = ecoApi.GetECOFromMoves
      ImportFromPath = pgnApi.ImportFromPath }