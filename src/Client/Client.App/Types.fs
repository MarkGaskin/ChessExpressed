module Client.App.Types

open Shared
open Fable.Remoting.Client

let todosApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IChessGameApi>

type Model =
    { ChessGames: ChessGame list
      Input: string }

type Msg =
    | GotChessGames of ChessGame list
    | SetInput of string
    | AddChessGame
    | AddedChessGame of ChessGame
