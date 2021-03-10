module Client.App.ChessBoard.Types

open Shared
open Shared.CEError
open ChessPieces

let transitionDuration = 500
let moveDuration = 1000

type Model =
    { Api: ICEApi
      FENPosition: string
      AllPieces: Piece list
      PreviousPieceCount: int
      ChessGame: ChessGame
      WhitePlayer : ChessPlayer
      WhitePlayerImage: string
      BlackPlayer : ChessPlayer
      BlackPlayerImage : string
      WhiteToMove: bool
      MovesList: string list
      ErrorString: string
      SquareStyles : obj
      Exn : exn option }

type InternalMsg =
    | HandleExn of exn
    | UpdateErrorString of string
    | UpdatedEcos of Result<unit,ServerError>
    | StartGame of (ChessGame option * ChessPlayer option * ChessPlayer option)
    | StartRecording
    | ParseMove of unit
    | ParseCastle of unit
    | UpdateSquareStyles of obj


type ExternalMsg =
    | GameRecorded of ChessGame

type Msg =
    | Internal of InternalMsg
    | External of ExternalMsg