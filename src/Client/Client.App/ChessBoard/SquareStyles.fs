module Client.App.ChessBoard.SquareStyles

open Fable.Core.JsInterop
open ChessPieces
open ChessSquares

let createJsSeq (key:string) value =
    [key ==> value]

let getPureSquareColor pieceColor count =
    match pieceColor, count with
    | White, 1 -> "rgba(255, 160, 160,0.6)"
    | White, 2 -> "rgba(255, 80, 80,0.7)"
    | White, 3 -> "rgba(220, 0, 0,0.7)"
    | White, _ -> "rgba(130, 0, 0,0.8)"
    | Black, 1 -> "rgba(180, 220, 255, 0.6)"
    | Black, 2 -> "rgba(80, 150, 255,0.7)"
    | Black, 3 -> "rgba(0, 80, 250,0.7)"
    | Black, _ -> "rgba(0, 60, 160,0.8)"


let getContestedSquareColor whiteCount blackCount =
    let wString = getPureSquareColor White whiteCount
    let bString = getPureSquareColor Black blackCount
    //"linear-gradient(" + bString + ", " + wString + ", 80%)"
    "linear-gradient(" + bString + "20%, " + wString + " 80%, " + wString + " 80%)"

let createSquareStyleObject (wSquareCoverage: Square list) (bSquareCoverage: Square list) =

    let wSquaresFiltered = List.countBy id wSquareCoverage
    let bSquaresFiltered = List.countBy id bSquareCoverage

    let getSquareCount (squares: (Square*int) list) (square: Square) =
        squares
        |> List.tryFind
            (fun (fSquare,count) -> square = fSquare)
        |> Option.map snd
        |> Option.defaultValue 0

    let getStyleByCount (wCount, bCount) =
        match wCount, bCount with
        | 0, 0 -> []
        | x, 0 -> createJsSeq "backgroundColor" (getPureSquareColor White x)
        | 0, y -> createJsSeq "backgroundColor" (getPureSquareColor Black y)
        | x, y -> createJsSeq "background" (getContestedSquareColor x y)
        


    Square.all
    |> List.fold
        (fun objSeq square ->
            (getSquareCount wSquaresFiltered square, getSquareCount bSquaresFiltered square)
            |> getStyleByCount
            |> createObj
            |> createJsSeq (Square.toString square)
            |> List.append objSeq) []
    |> createObj



let createPositionObject (allPieces : Piece list) =
    allPieces
    |> List.fold
        (fun jsString piece ->
            List.append jsString (createJsSeq (piece.Square |> Square.toString) (piece |> Piece.toString))
            ) []
    |> createObj