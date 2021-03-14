module Server.DAL.Common

open Shared
open Shared.CEError

open LiteDB.FSharp
open LiteDB
open System
open System.IO
open System.Text
open Newtonsoft.Json


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

let findPlayerIdOrCreate playerName =
    if String.exists ((=) ',') playerName then
        let names = playerName.Split(',')
        let firstName = names |> Array.tryItem 1 |> Option.defaultValue ""
        let lastName = names |> Array.tryItem 0 |> Option.defaultValue ""
        storage.GetChessPlayers()
        |> List.tryFind
            (fun chessPlayer ->
                chessPlayer.LastName = lastName && (chessPlayer.FirstName.StartsWith firstName || firstName.StartsWith chessPlayer.FirstName ))
        |> function
           | Some chessPlayer when chessPlayer.FirstName < firstName ->
               { chessPlayer with FirstName = firstName }
               |> storage.UpdateChessPlayer
               |> ignore
               chessPlayer.Id
           | Some chessPlayer ->
               chessPlayer.Id
           | None ->
               let playerId = Guid.NewGuid()
               { ChessPlayer.defaultPlayer with Id = playerId; FirstName = firstName; LastName = lastName }
               |> storage.AddChessPlayer
               |> ignore
               playerId
    else
        let nickName = playerName
        storage.GetChessPlayers()
        |> List.tryFind
            (fun chessPlayer ->
                chessPlayer.NickName = nickName)
        |> function
           | Some chessPlayer ->
               chessPlayer.Id
           | None ->
               let playerId = Guid.NewGuid()
               { ChessPlayer.defaultPlayer with Id = playerId; NickName = nickName }
               |> storage.AddChessPlayer
               |> ignore
               playerId

open TimHanewich.Chess.BatchAnalysis
open TimHanewich.Chess.Pgn
open System.Text.RegularExpressions

let stripComments (gameString:string) = 
    Regex.Replace(gameString, @" ?\{[\s\S]*?\}", String.Empty)
    |> fun updatedString ->
        Regex.Replace(updatedString, @" ?\([\s\S]*?\)", String.Empty)
    |> fun updatedString ->
        Regex.Replace(updatedString, @" ?\[[\s\S]*?\]", String.Empty)

let stripMoveNumbers (gameString:string) = 
    Regex.Replace(gameString, @"\d+\.", String.Empty)

let stripResult (gameString:string) =
    gameString.Replace("1/2-1/2", "").Replace("1-0", "").Replace("0-1", "")

let parseMoves (gameString: string) =
    let gameString = gameString |> stripComments
    let gameString = gameString.Substring(gameString.IndexOf("1."))
    let gameString = gameString.Replace("?", "")
                               .Replace("+", "")
                               .Replace("x", "")
                               .Replace("#", "")
                               .Replace("!", "")
                               .Replace("\n", " ")
                               .Replace("...", ".")
                               .Replace("*", "")
    let gameString = gameString |> stripMoveNumbers |> stripResult
    let gameString = gameString.Replace("   ", "  ")
                               .Replace("  ", " ")
    gameString.Split(" ")
    |> Array.map (fun (string:string) -> string.Trim())
    |> Array.filter(fun string -> string |> String.IsNullOrWhiteSpace |> not)

let getDisplayName (pgnParser: PgnParserLite)  =
    pgnParser.White + " vs. " + pgnParser.Black + ", " + pgnParser.Event + "R" + pgnParser.Round + " " + (pgnParser.Date.Year |> string)

let createNotes (gameString: string) =
    try
        let idx = 
            gameString
            |> fun string -> string.IndexOf("1.")

        gameString.Substring(0,idx)
    with _ ->
        ""

let pgnParserToChessGame (pgnParser: PgnParserLite) (gameString: string) =
    { Event = pgnParser.Event |> Some
      DisplayName = getDisplayName pgnParser
      Id = Guid.NewGuid()
      PlayerIds = [ findPlayerIdOrCreate pgnParser.White; findPlayerIdOrCreate pgnParser.Black]
      EloWhite = pgnParser.WhiteElo |> string |> Some
      EloBlack = pgnParser.BlackElo |> string |> Some
      Year = pgnParser.Date.Year |> string |> Some
      Result = match pgnParser.Result with
               | "1-0" -> WhiteWin
               | "0-1" -> BlackWin
               | _ -> Draw
      GameNotation = gameString |> stripComments |> fun string -> string.Substring(string.IndexOf("1."))
      MovesList = gameString |> parseMoves
      HasRecorded = false
      Eco = pgnParser.ECO
      Round = pgnParser.Round |> Some
      Notes = createNotes gameString }

let rec parseAllPgn (pgnSplitter: MassivePgnFileSplitter) =
    try
        let gameString = pgnSplitter.GetNextGame()
        if gameString |> String.IsNullOrWhiteSpace then [||]
        else
            PgnParserLite.ParsePgn(gameString)
            |> fun pgnParser ->
                [|pgnParserToChessGame pgnParser gameString|]
                |> Array.append (parseAllPgn pgnSplitter)
    with _ ->
        [||]


let importGames (directoryPath: string) =
    async {
        return result{
            try
                Directory.GetFiles(directoryPath)
                |> Array.iter
                    (fun filePath ->
                        try
                            use stream = File.OpenRead(filePath)
                        
                            let pgnSplitter = new MassivePgnFileSplitter(stream)

                            let parsedGames = parseAllPgn pgnSplitter
                        
                            parsedGames
                            |> Array.iter
                                (fun game ->
                                    storage.GetChessGames ()
                                    |> List.exists (ChessGame.isRoughlyEqual game)
                                    |> function
                                       | true -> ()
                                       | false ->
                                            storage.AddChessGame game |> ignore)
                        with _ ->
                            ()
                        )

                |> ignore
                return! Ok ()
            with
            | :? System.IO.FileNotFoundException
            | :? System.IO.DirectoryNotFoundException ->
                return! Error FileForParsingNotFound
            | e ->
                return! Error (FailedToImportGames e)
        }
    }

let importGame (directoryPath : string) =
    async {
        return result{
            try
                Directory.GetFiles(directoryPath)
                |> Array.iter
                    (fun filePath ->
                        let gameString = File.ReadAllText(filePath)
                        let pgnParser = PgnParserLite.ParsePgn(gameString)
                        let game = pgnParserToChessGame pgnParser gameString
                
                        storage.GetChessGames ()
                        |> List.exists (ChessGame.isRoughlyEqual game)
                        |> function
                            | true -> ()
                            | false ->
                                storage.AddChessGame game |> ignore
                        |> ignore)
                return! Ok ()
            with
            | :? System.IO.FileNotFoundException
            | :? System.IO.DirectoryNotFoundException ->
                return! Error FileForParsingNotFound
            | e ->
                return! Error (FailedToImportGames e)
        }
    }

let pgnApi : PGNApi =
    { ImportFromPath = importGames
      ImportGame = importGame }


let chessGameApi : ChessGameApi =
    {  getChessGames = fun () -> async { return storage.GetChessGames() }
       addChessGame =
            fun chessGame -> async {
                let newChessGame = { chessGame with MovesList = chessGame.GameNotation |> parseMoves }
                match storage.AddChessGame newChessGame with
                | Ok () -> return newChessGame
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

let createShoutout (chessPlayer: ChessPlayer) =
    "Follow "
    + ChessPlayer.getPlayerName chessPlayer
    + ":\n"
    +
    if chessPlayer.TwitterHandle |> String.IsNullOrWhiteSpace then
        ""
    else
        "Twitter: " + chessPlayer.TwitterHandle + "\n"
    +
    if chessPlayer.YouTubeChannel |> String.IsNullOrWhiteSpace then
        ""
    else
        "YouTube: " + chessPlayer.YouTubeChannel + "\n"
    +
    if chessPlayer.TwitchChannel |> String.IsNullOrWhiteSpace then
        ""
    else
        "Twitch: " + chessPlayer.TwitchChannel + "\n"

let selfPromo =
    "Follow me on Twitter: https://twitter.com/ChessExpressed"


let createTextFile ((whitePlayer, blackPlayer, chessGame) : ChessPlayer * ChessPlayer * ChessGame) =
    async{
        let filename =
            ChessPlayer.getPlayerName whitePlayer
            +
            " vs. "
            +
            ChessPlayer.getPlayerName blackPlayer
            +
            " "
            +
            (chessGame.Event |> Option.defaultValue "")
            +
            DateTime.Now.ToString("yyyy-MM-dd")
            +
            ".txt"

        let directory = "D:\ChessVideos\Notes"
           
        let filePath = Path.Combine(directory, filename)

        let fileString =
            [ selfPromo; chessGame.Notes + "\n"; chessGame.GameNotation + "\n"; createShoutout whitePlayer; createShoutout blackPlayer ] 

        File.WriteAllLines(filePath, fileString)

        return Ok ()
    }

type JSONWrite =
    { FENArray: string[]
      SquareStylesArray: obj[] }

let createJSFile ((fenArray, squareStyles) : string[] * obj[]) =
    async{
        let filename = "game.js"

        let directory = "C:\Users\markr\git-repos\RemotionChess\pgn"
           
        let filePath = Path.Combine(directory, filename)

        let header = "const text ="
        let footer = "export default text;"

        let fileString =
            "`" + JsonConvert.SerializeObject { FENArray = fenArray; SquareStylesArray = squareStyles } + "`"

        File.WriteAllLines(filePath, [header; fileString; footer])

        return Ok ()
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
      ImportFromPath = pgnApi.ImportFromPath
      ImportGame = pgnApi.ImportGame
      CreateTextFile = createTextFile
      CreateJSFile = createJSFile }