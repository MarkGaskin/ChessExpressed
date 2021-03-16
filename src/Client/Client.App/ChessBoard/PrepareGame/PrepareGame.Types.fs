module Client.App.ChessBoard.PrepareGameTypes

open Shared
open Shared.CEError
open ChessPieces

let transitionDuration = 500
let moveDuration = 1000

type Model =
    { Api: ICEApi
      FENPosition: string
      AllPieces: Piece list
      GameIndex: int
      ChessGame: ChessGame
      WhitePlayer : ChessPlayer
      BlackPlayer : ChessPlayer
      CastleMoveNumbers: int[]
      WhiteToMove: bool
      MovesList: string list
      ErrorString: string
      SquareStyles : obj []
      FENArray : string []
      Exn : exn option }

type InternalMsg =
    | HandleExn of exn
    | UpdateErrorString of string
    | UpdatedEcos of Result<unit,ServerError>
    | StartGame of (ChessGame option * ChessPlayer option * ChessPlayer option)
    | ParseMove
    | ParseCastle
    | UpdateSquareStyles of obj
    | CreateTextFile of Result<unit, ServerError>
    | MoveForward
    | MoveBackward
    | GameComplete


type Msg =
    | Internal of InternalMsg