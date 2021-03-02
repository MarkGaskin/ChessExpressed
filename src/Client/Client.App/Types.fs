module Client.App.Types

open Shared
open Fable.Remoting.Client
open Fable.React
open Fable.Core.JsInterop

let darkSquareStyle = createObj ["backgroundColor" ==> "rgb(78, 181, 78)"]
let lightSquareStyle = createObj ["backgroundColor" ==> "rgb(240, 255, 240)"]

let squareObj = createObj ["backgroundColor" ==> "red"; "opacity" ==> "0.5"]
let squareStyle = createObj ["e4" ==> squareObj]

type ChessBoardProps =
    | Position of string
    | TransitionDuration of int
    | Width of int
    | DarkSquareStyle of obj
    | LightSquareStyle of obj
    | SquareStyles of obj


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
