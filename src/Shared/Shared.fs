namespace Shared

open System


type ChessGameResult =
    | WhiteWin
    | BlackWin
    | Draw

[<CLIMutable>]
type ChessPlayer =
    { PlayerId: Guid
      FirstName: string
      LastName: string
      NickName: string
      TwitchChannel: string
      YouTubeChannel: string
      TwitterHandle: string }
    static member defaultPlayer =
        { PlayerId = Guid.Empty
          FirstName = ""
          LastName = ""
          NickName = ""
          TwitchChannel = ""
          YouTubeChannel = ""
          TwitterHandle = "" }

module ChessPlayer =
    let create (firstName:string) (lastName:string) (nickName:string) (twitch:string) (yt:string) (twitter:string) =
        { PlayerId = Guid.NewGuid()
          FirstName = firstName.Trim()
          LastName = lastName.Trim()
          NickName = nickName.Trim()
          TwitchChannel = twitch.Trim()
          YouTubeChannel = yt.Trim()
          TwitterHandle = twitter.Trim() }

    let createByName firstName lastName =
        { ChessPlayer.defaultPlayer with
              PlayerId = Guid.NewGuid ()
              FirstName = firstName
              LastName = lastName
        }

    let isEqual chessPlayer1 chessPlayer2 =
        chessPlayer1.FirstName = chessPlayer2.FirstName &&
        chessPlayer1.LastName = chessPlayer2.LastName

[<CLIMutable>]
type ChessGame =
    { GameId: Guid
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
        { GameId = Guid.Empty
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
      getChessPlayers : unit -> Async<ChessPlayer list>
      addChessPlayer : ChessPlayer -> Async<ChessPlayer> }