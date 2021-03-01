namespace Shared

open System

type ChessGameResult =
    | WhiteWin
    | BlackWin
    | Draw

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

type IChessGameApi =
    { getChessGames : unit -> Async<ChessGame list>
      addChessGame : ChessGame -> Async<ChessGame> }