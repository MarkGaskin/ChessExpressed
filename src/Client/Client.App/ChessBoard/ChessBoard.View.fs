module Client.App.ChessBoard.View

open Shared
open Fable.Remoting.Client
open Fable.React
open Fable.Core.JsInterop
open Browser
open Browser.Types
open Browser.Blob
open Fable.Core
open Fable.React.Props

type ChessBoardProps =
    | Position of string
    | TransitionDuration of int
    | Width of int
    | DarkSquareStyle of obj
    | LightSquareStyle of obj
    | SquareStyles of obj
    | ShowNotation of bool


let darkSquareStyle = createObj ["backgroundColor" ==> "rgba(220, 220, 220)"]
let lightSquareStyle = createObj ["backgroundColor" ==> "rgb(255, 255, 255)"]

let squareObj = createObj ["backgroundColor" ==> "rgba(255, 150, 150,0.6)"]
let dark1squareObj = createObj ["backgroundColor" ==> "rgba(255, 80, 80,0.7)"]
let dark2squareObj = createObj ["backgroundColor" ==> "rgba(255, 0, 0,0.8)"]
let dark4squareObj = createObj ["backgroundColor" ==> "rgba(180, 0, 0,0.9)"]

let bsquareObj = createObj ["backgroundColor" ==> "rgba(150, 200, 255, 0.6)"]
let bdark1squareObj = createObj ["backgroundColor" ==> "rgba(45, 110, 255,0.7)"]
let bdark2squareObj = createObj ["backgroundColor" ==> "rgba(0, 40, 255,0.7)"]
let bdark4squareObj = createObj ["backgroundColor" ==> "rgba(0, 40, 150,0.8)"]

//let contestedSquare = createObj ["background"]

let squareStyle = createObj ["e5" ==> squareObj; "e4" ==> squareObj; "e3" ==> dark1squareObj; "d4" ==> dark2squareObj; "d3" ==> dark2squareObj; "c3" ==> dark4squareObj;
                             "d6" ==> bsquareObj; "d7" ==> bdark1squareObj; "e7" ==> bdark4squareObj; "e8" ==> bdark2squareObj; "f7" ==> bdark2squareObj; "c7" ==> bdark4squareObj]
