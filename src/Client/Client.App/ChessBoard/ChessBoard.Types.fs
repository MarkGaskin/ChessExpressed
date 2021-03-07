module Client.App.ChessBoard.Types

open Shared
open Shared.CEError

type Model =
    { Api: ICEApi
      FENPosition: string
      ErrorString: string
      Exn : exn option }

type Msg =
    | HandleExn of exn
    | UpdateErrorString of string
    | UpdatedEcos of Result<unit,ServerError>
    | StartGame of (ChessGame option * ChessPlayer option * ChessPlayer option)