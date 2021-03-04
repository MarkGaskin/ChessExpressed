namespace Shared

open System


type ChessGameResult =
    | WhiteWin
    | BlackWin
    | Draw

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

[<CLIMutable>]
type ChessGame =
    { Id: Guid
      PlayerIdWhite: Guid
      PlayerIdBlack: Guid
      EloWhite: int
      EloBlack: int
      Date: DateTime
      Event: string
      Result: ChessGameResult
      GameNotation: string}

module ChessGame =
    let isValid (description: string) =
        String.IsNullOrWhiteSpace description |> not

    let defaultGame =
        { Id = Guid.Empty
          PlayerIdWhite = Guid.Empty
          PlayerIdBlack = Guid.Empty
          EloWhite = 0
          EloBlack = 0
          Date = DateTime.Now
          Event = ""
          Result = Draw
          GameNotation = ""}
        

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ICEApi =
    { getChessGames : unit -> Async<ChessGame list>
      addChessGame : ChessGame -> Async<ChessGame>
      deleteChessGame : ChessGame -> Async<ChessGame>
      updateChessGame : ChessGame -> Async<ChessGame>
      getChessPlayers : unit -> Async<ChessPlayer list>
      addChessPlayer : ChessPlayer -> Async<ChessPlayer>
      deleteChessPlayer : ChessPlayer -> Async<ChessPlayer>
      updateChessPlayer : ChessPlayer -> Async<ChessPlayer> }