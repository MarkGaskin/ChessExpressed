module Client.App.ChessBoard.ChessPieces

open Fable.Core
open Fable.Core.JsInterop

type Rank =
    | One
    | Two
    | Three
    | Four
    | Five
    | Six
    | Seven
    | Eight
    static member all =
        [One; Two; Three; Four; Five; Six; Seven; Eight]
    static member getInt =
        function
        | One -> 0
        | Two -> 1
        | Three -> 2
        | Four -> 3
        | Five -> 4
        | Six -> 5
        | Seven -> 6
        | Eight -> 7

type File =
    | A
    | B
    | C
    | D
    | E
    | F
    | G
    | H
    static member all =
        [A;B;C;D;E;F;G;H]
    static member getInt =
        function
        | A -> 0
        | B -> 1
        | C -> 2
        | D -> 3
        | E -> 4
        | F -> 5
        | G -> 6
        | H -> 7

type Square =
    { Rank: int
      File: int }
    static member (+) (square:Square, (fileDelta, rankDelta): (int*int)) =
        { File = square.File + fileDelta; Rank = square.Rank + rankDelta }
    static member (-) (square:Square, (fileDelta, rankDelta): (int*int)) =
        { File = square.File - fileDelta; Rank = square.Rank - rankDelta }

module Square =
    let all =
        [1..8]
        |> List.map
            (fun file ->
                [1..8]
                |> List.map
                    (fun rank ->
                        { File = file; Rank = rank } ))
        |> List.concat
    let isValid square =
        square.Rank <= 8 && square.Rank >= 1 && square.File <= 8 && square.File >= 1

    let create file rank =
        { File = file; Rank = rank}

    let toString square =
        match square.File with
        | 1 -> "a"
        | 2 -> "b"
        | 3 -> "c"
        | 4 -> "d"
        | 5 -> "e"
        | 6 -> "f"
        | 7 -> "g"
        | _ -> "h"
        +
        match square.Rank with
        | 1 -> "1"
        | 2 -> "2"
        | 3 -> "3"
        | 4 -> "4"
        | 5 -> "5"
        | 6 -> "6"
        | 7 -> "7"
        | _ -> "8"


        


type PieceType =
    | Pawn
    | Knight
    | Bishop
    | Rook
    | Queen
    | King
    static member all =
        [ Pawn; Knight; Bishop; Rook; Queen; King ]

type PieceColor =
    | Black
    | White
    static member all =
        [Black; White]

type Piece =
    { PieceType : PieceType
      Square : Square
      Color : PieceColor }

module Piece =
    let create pieceType color square =
        { PieceType = pieceType; Square = square; Color = color }

    let createMany pieceType color squares =
        squares
        |> List.map (create pieceType color)

    let initPieces () =
        [ createMany Pawn Black ([1..8] |> List.map (fun file -> Square.create file 7))
          createMany Pawn White ([1..8] |> List.map (fun file -> Square.create file 2))
          createMany Rook Black [Square.create 1 8; Square.create 8 8]
          createMany Rook White [Square.create 1 1; Square.create 8 1]
          createMany Knight Black [Square.create 2 8; Square.create 7 8]
          createMany Knight White [Square.create 2 1; Square.create 7 1]
          createMany Bishop Black [Square.create 3 8; Square.create 6 8]
          createMany Bishop White [Square.create 3 1; Square.create 6 1]
          createMany Queen Black [Square.create 4 8]
          createMany Queen White [Square.create 4 1]
          createMany King Black [Square.create 5 8]
          createMany King White [Square.create 5 1] ]
        |> List.concat

    let toString piece =
        if piece.Color = White then "w" else "b"
        +
        match piece.PieceType with
        | Pawn -> "P"
        | Knight -> "N"
        | Bishop -> "B"
        | Rook -> "R"
        | Queen -> "Q"
        | King -> "K"

    let filterPiecesByColor color pieces =
        pieces
        |> List.filter (fun piece -> piece.Color = color)


let rec private getCoveredSquares (startingPiece:Piece) ((fileDelta, rankDelta) as direction: int*int) (allPieces: Piece list) =
    let newSquare = startingPiece.Square + direction
    if Square.isValid newSquare |> not then
        []
    elif (List.forall (fun piece -> newSquare <> piece.Square) allPieces) then
        List.append [newSquare] (getCoveredSquares { startingPiece with Square = newSquare } direction allPieces)
    else
        allPieces
        |> List.find (fun piece -> newSquare = piece.Square)
        |> fun piece ->
            match piece.Color = startingPiece.Color,
                    startingPiece.PieceType,
                    piece.PieceType with
            | false, _, _ ->
                [newSquare]

            | _, Rook, Rook
            | _, Rook, Queen
            | _, Bishop, Bishop
            | _, Bishop, Queen
            | _, Queen, Bishop when (fileDelta * rankDelta <> 0) ->
                List.append [newSquare] (getCoveredSquares { startingPiece with Square = newSquare } direction allPieces)

            | _, Queen, Rook when (fileDelta * rankDelta = 0) ->
                List.append [newSquare] (getCoveredSquares { startingPiece with Square = newSquare } direction allPieces)

            | _, Bishop, Pawn
            | _, Queen, Pawn when (fileDelta * rankDelta <> 0) ->
                List.append [newSquare] ([newSquare+direction] |> List.filter Square.isValid)

            | _ -> [newSquare]

let private getBishopCoveredSquares (startingPiece:Piece) (allPieces: Piece list) =
    [(1,1); (-1,1); (1,-1); (-1,-1)]
    |> List.map
        (fun direction -> getCoveredSquares startingPiece direction allPieces)
    |> List.concat

let private getRookCoveredSquares (startingPiece:Piece) (allPieces: Piece list) =
    [(1,0); (-1,0); (0,-1); (0,1)]
    |> List.map
        (fun direction -> getCoveredSquares startingPiece direction allPieces)
    |> List.concat

let private getQueenCoveredSquares (startingPiece:Piece) (allPieces: Piece list) =
    [(1,0); (-1,0); (0,-1); (0,1); (1,1); (-1,1); (1,-1); (-1,-1)]
    |> List.map
        (fun direction -> getCoveredSquares startingPiece direction allPieces)
    |> List.concat

let getPieceCoverage (piece:Piece) allPieces =
    match piece.PieceType, piece.Color with
    | Pawn, White -> [ piece.Square + (1,1); piece.Square + (-1,1) ] |> List.filter Square.isValid
    | Pawn, Black -> [ piece.Square + (1,-1); piece.Square + (-1,-1) ] |> List.filter Square.isValid
    | King, _ -> [-1..1]
                    |> List.map
                    (fun fileDelta ->
                        [-1..1]
                        |> List.map
                            (fun rankDelta -> piece.Square + (fileDelta, rankDelta)))
                    |> List.concat
                    |> List.filter Square.isValid
    | Bishop, _ -> getBishopCoveredSquares piece allPieces
    | Knight, _ -> [(1,2); (-1,2); (-1,-2); (1,-2); (2,1); (-2,1); (2,-1); (-2,-1)]
                    |> List.map (fun direction -> piece.Square + direction)
                    |> List.filter Square.isValid
    | Rook, _ -> getRookCoveredSquares piece allPieces
    | Queen, _ -> getQueenCoveredSquares piece allPieces

let getPieceMovement (piece:Piece) allPieces =
    match piece.PieceType, piece.Color with
    | Pawn, White -> [ piece.Square + (0,1); if piece.Square.Rank = 2 &&
                                                 List.forall (fun existingPieces -> existingPieces.Square <> Square.create piece.Square.File 3) allPieces then piece.Square + (0,2) ]
                     |> List.filter Square.isValid
    | Pawn, Black -> [ piece.Square + (0,-1); if piece.Square.Rank = 7 &&
                                                 List.forall (fun existingPieces -> existingPieces.Square <> Square.create piece.Square.File 6) allPieces then piece.Square + (0,-2) ]
                     |> List.filter Square.isValid
    | King, _ -> [-1..1]
                    |> List.map
                    (fun fileDelta ->
                        [-1..1]
                        |> List.map
                            (fun rankDelta -> piece.Square + (fileDelta, rankDelta)))
                    |> List.concat
                    |> List.filter Square.isValid
    | Bishop, _ -> getBishopCoveredSquares piece allPieces
    | Knight, _ -> [(1,2); (-1,2); (-1,-2); (1,-2); (2,1); (-2,1); (2,-1); (-2,-1)]
                    |> List.map (fun direction -> piece.Square + direction)
                    |> List.filter Square.isValid
    | Rook, _ -> getRookCoveredSquares piece allPieces
    | Queen, _ -> getQueenCoveredSquares piece allPieces

let getSquareCoverage allPieces =
    allPieces
    |> List.filter(fun piece -> piece.Color = White)
    |> List.map (fun piece ->
        getPieceCoverage piece allPieces)
    |> List.concat ,
        allPieces
        |> List.filter(fun piece -> piece.Color = Black)
        |> List.map (fun piece ->
            getPieceCoverage piece allPieces)
        |> List.concat

let parseFile c =
    match c with
    | 'a' -> 1
    | 'b' -> 2
    | 'c' -> 3
    | 'd' -> 4
    | 'e' -> 5
    | 'f' -> 6
    | 'g' -> 7
    | _ -> 8

let parseUnknown c =
    match c with
    | 'a' -> (fun (piece: Piece) -> piece.Square.File = 1)
    | 'b' -> (fun (piece: Piece) -> piece.Square.File = 2)
    | 'c' -> (fun (piece: Piece) -> piece.Square.File = 3)
    | 'd' -> (fun (piece: Piece) -> piece.Square.File = 4)
    | 'e' -> (fun (piece: Piece) -> piece.Square.File = 5)
    | 'f' -> (fun (piece: Piece) -> piece.Square.File = 6)
    | 'g' -> (fun (piece: Piece) -> piece.Square.File = 7)
    | 'h' -> (fun (piece: Piece) -> piece.Square.File = 8)
    | '1' -> (fun (piece: Piece) -> piece.Square.Rank = 1)
    | '2' -> (fun (piece: Piece) -> piece.Square.Rank = 2)
    | '3' -> (fun (piece: Piece) -> piece.Square.Rank = 3)
    | '4' -> (fun (piece: Piece) -> piece.Square.Rank = 4)
    | '5' -> (fun (piece: Piece) -> piece.Square.Rank = 5)
    | '6' -> (fun (piece: Piece) -> piece.Square.Rank = 6)
    | '7' -> (fun (piece: Piece) -> piece.Square.Rank = 7)
    | '8' -> (fun (piece: Piece) -> piece.Square.Rank = 8)
    | _ -> (fun _ -> false)



let parseMove (moveString:string) (pieces: Piece list) =
    let rank, file = moveString.[moveString.Length-1] |> string |> int, parseFile moveString.[moveString.Length-2]
    let square = Square.create file rank

    let pieces =
        pieces
        |> List.filter (fun piece -> piece.Square <> square)

    let simpleUpdate pieceType =
        pieces
        |> List.map
            (fun piece ->
                if piece.PieceType <> pieceType && List.contains square (getPieceMovement piece pieces) then { piece with Square = square }
                else piece )
    let complexUpdate pieceType =
        let filterFunction = parseUnknown moveString.[1]
        pieces
        |> List.map
            (fun piece ->
                if piece.PieceType <> pieceType && filterFunction piece then { piece with Square = square }
                else piece )
    match moveString.[0] with
    | 'K' ->
        pieces
        |> List.map
            (fun piece ->
                if piece.PieceType <> King then { piece with Square = square }
                else piece)
    | 'Q' when moveString.Length = 3 ->
        simpleUpdate Queen
    | 'Q' ->
        complexUpdate Queen
    | 'R' when moveString.Length = 3 ->
        simpleUpdate Rook
    | 'R' ->
        complexUpdate Rook
    | 'N' when moveString.Length = 3 ->
        simpleUpdate Knight
    | 'N' ->
        complexUpdate Knight
    | 'B' when moveString.Length = 3 ->
        simpleUpdate Bishop
    | 'B' ->
        complexUpdate Bishop
    
    | 'O' when moveString.Length > 3 ->
        pieces
        |> List.map
            (fun piece ->
                match piece.PieceType, piece.Color with
                | King, White -> { piece with Square = Square.create 3 1 }
                | King, Black -> { piece with Square = Square.create 3 8 }
                | Rook, White when piece.Square = Square.create 1 1 -> { piece with Square = Square.create 4 1}
                | Rook, Black when piece.Square = Square.create 1 8 -> { piece with Square = Square.create 4 8}
                | _ -> piece)
    | 'O' when moveString.Length <= 3 ->
        pieces
        |> List.map
            (fun piece ->
                match piece.PieceType, piece.Color with
                | King, White -> { piece with Square = Square.create 7 1 }
                | King, Black -> { piece with Square = Square.create 7 8 }
                | Rook, White when piece.Square = Square.create 8 1 -> { piece with Square = Square.create 6 1}
                | Rook, Black when piece.Square = Square.create 8 8 -> { piece with Square = Square.create 6 8}
                | _ -> piece)
    | _ when moveString.Length = 2 ->
        simpleUpdate Pawn
    | _ when moveString.Contains '=' ->
        // Promotion
        pieces
    | _ ->
        pieces
        |> List.map
            (fun piece ->
                if piece.Square = Square.create (parseFile moveString.[0]) (if piece.Color = White then rank - 1 else rank + 1) then { piece with Square = square }
                else piece
                 )


let createJsSeq (key:string) value =
    [key ==> value]

let createPositionObject (allPieces : Piece list) =
    allPieces
    |> List.fold
        (fun jsString piece ->
            List.append jsString (createJsSeq (piece.Square |> Square.toString) (piece |> Piece.toString))
            ) []
    |> createObj

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
        | x, y -> createJsSeq "backgroundColor" (getPureSquareColor Black y)
        


    Square.all
    |> List.fold
        (fun objSeq square ->
            (getSquareCount wSquaresFiltered square, getSquareCount bSquaresFiltered square)
            |> getStyleByCount
            |> createObj
            |> createJsSeq (Square.toString square)
            |> List.append objSeq) []
    |> createObj 