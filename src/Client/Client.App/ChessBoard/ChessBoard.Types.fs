module Client.App.ChessBoard.Types

open Shared

type Model =
    { Api: ICEApi
      FENPosition: string }

type Msg =
    | StartGame of string