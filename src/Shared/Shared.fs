namespace Shared

open System

open Shared.CEError


type ChessGameResult =
    | WhiteWin
    | BlackWin
    | Draw
    static member describe =
        function
        | WhiteWin -> "WhiteWin"
        | BlackWin -> "BlackWin"
        | Draw -> "Draw"
    static member parse =
        function
        | "WhiteWin" -> WhiteWin
        | "BlackWin" -> BlackWin
        | _ -> Draw
    static member all =
        [WhiteWin; BlackWin; Draw]
    

[<CLIMutable>]
type ECO =
    { Id : Guid
      Eco: string
      Name: string
      Moves: string }
    static member defaultEco =
        { Id = Guid.NewGuid(); Eco = ""; Name = ""; Moves = ""}



[<CLIMutable>]
type ChessPlayer =
    { Id: Guid
      FirstName: string
      LastName: string
      NickName: string
      TwitchChannel: string
      YouTubeChannel: string
      TwitterHandle: string }
    static member defaultPlayer =
        { Id = Guid.NewGuid()
          FirstName = ""
          LastName = ""
          NickName = ""
          TwitchChannel = ""
          YouTubeChannel = ""
          TwitterHandle = "" }

module ChessPlayer =
    let create (firstName:string) (lastName:string) (nickName:string) (twitch:string) (yt:string) (twitter:string) =
        { Id = Guid.NewGuid()
          FirstName = firstName.Trim()
          LastName = lastName.Trim()
          NickName = nickName.Trim()
          TwitchChannel = twitch.Trim()
          YouTubeChannel = yt.Trim()
          TwitterHandle = twitter.Trim() }

    let createByName firstName lastName =
        { ChessPlayer.defaultPlayer with
              Id = Guid.NewGuid ()
              FirstName = firstName
              LastName = lastName
        }

    let isEqual chessPlayer1 chessPlayer2 =
        if String.IsNullOrWhiteSpace chessPlayer1.FirstName then
            chessPlayer1.NickName = chessPlayer2.NickName
        else
            chessPlayer1.FirstName = chessPlayer2.FirstName &&
            String.IsNullOrWhiteSpace chessPlayer1.LastName &&
            chessPlayer1.LastName = chessPlayer2.LastName &&
            chessPlayer1.NickName = chessPlayer2.NickName

    let isValid chessPlayer =
        match String.IsNullOrWhiteSpace chessPlayer.FirstName,
              String.IsNullOrWhiteSpace chessPlayer.LastName,
              String.IsNullOrWhiteSpace chessPlayer.NickName with
        | true, false, _
        | false, true, _
        | true, true, true -> false
        | _ -> true

    let getPlayerName (chessPlayer: ChessPlayer) =
        if chessPlayer.NickName = "" then chessPlayer.FirstName + " " + chessPlayer.LastName
        else chessPlayer.NickName

    let searchNameStart chessPlayer (string: string): bool =
        match String.IsNullOrWhiteSpace string,
              string = chessPlayer.NickName || string = getPlayerName chessPlayer,
              chessPlayer.NickName.StartsWith string,
              chessPlayer.FirstName.StartsWith (string.Trim()),
              chessPlayer.LastName.StartsWith (string.Trim()) with
        | false, true, _, _, _ -> false
        | true, _, _, _, _
        | _, _, true, _, _
        | _, _, _, true, _
        | _, _, _ ,_, true -> true
        | _ ->
            string.Split " "
            |> Array.indexed
            |> Array.forall
                (fun (idx, string) -> if idx = 0 then chessPlayer.FirstName.StartsWith string
                                      else chessPlayer.LastName.StartsWith string)


[<CLIMutable>]
type ChessGame =
    { Id: Guid
      PlayerIds : Guid list
      EloWhite: string option
      EloBlack: string option
      Year: string option
      Event: string option
      Result: ChessGameResult
      GameNotation: string
      Eco: ECO option
      TotalMoves : int
      Notes: string
      HasRecorded: bool }

module ChessGame =
    let isValid (chessGame : ChessGame) =
        chessGame.PlayerIds.Length = 2

    let isRoughlyEqual chessGame1 chessGame2 =
        chessGame1.PlayerIds = chessGame2.PlayerIds &&
        chessGame1.GameNotation = chessGame2.GameNotation

    let defaultGame =
        { Id = Guid.NewGuid()
          PlayerIds = [ Guid.Empty; Guid.Empty ]
          EloWhite = None
          EloBlack = None
          Year = None
          Event = None
          Result = Draw
          GameNotation = ""
          HasRecorded = false
          Eco = None
          TotalMoves = 0
          Notes = ""
          }
        
module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type PGNApi =
    { ImportFromPath : string -> Async<Result<unit,ServerError>> }

type ECOApi =
    {  UpdateECOs : unit -> Async<Result<unit,ServerError>>
       GetECOFromID : string -> Async<Result<ECO,ServerError>>
       GetECOFromMoves : string -> Async<Result<ECO,ServerError>> }

type ChessGameApi =
    { getChessGames : unit -> Async<ChessGame list>
      addChessGame : ChessGame -> Async<ChessGame>
      deleteChessGame : ChessGame -> Async<ChessGame>
      updateChessGame : ChessGame -> Async<ChessGame> }

type ChessPlayerApi =
    { getChessPlayers : unit -> Async<ChessPlayer list>
      addChessPlayer : ChessPlayer -> Async<ChessPlayer>
      deleteChessPlayer : ChessPlayer -> Async<ChessPlayer>
      updateChessPlayer : ChessPlayer -> Async<ChessPlayer> }
type ICEApi =
    { getChessPlayers : unit -> Async<ChessPlayer list>
      addChessPlayer : ChessPlayer -> Async<ChessPlayer>
      deleteChessPlayer : ChessPlayer -> Async<ChessPlayer>
      updateChessPlayer : ChessPlayer -> Async<ChessPlayer>
      getChessGames : unit -> Async<ChessGame list>
      addChessGame : ChessGame -> Async<ChessGame>
      deleteChessGame : ChessGame -> Async<ChessGame>
      updateChessGame : ChessGame -> Async<ChessGame> 
      UpdateECOs : unit -> Async<Result<unit,ServerError>>
      GetECOFromID : string -> Async<Result<ECO,ServerError>>
      GetECOFromMoves : string -> Async<Result<ECO,ServerError>>
      ImportFromPath : string -> Async<Result<unit, ServerError>> }