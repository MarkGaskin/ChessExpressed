module Client.App.ChessBoard.ChessPieces

open Fable.Core
open Fable.Core.JsInterop
open ChessSquares
open System

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

    let toChar piece =
        match piece.PieceType, piece.Color with
        | Pawn, White -> 'P'
        | Pawn, Black -> 'p'
        | Knight, White -> 'N'
        | Knight, Black -> 'n'
        | Bishop, White -> 'B'
        | Bishop, Black -> 'b'
        | Rook, White -> 'R'
        | Rook, Black -> 'r'
        | Queen, White -> 'Q'
        | Queen, Black -> 'q'
        | King, White -> 'K'
        | King, Black -> 'k'

    let fromChar char square =
        let piece = {PieceType = Pawn; Color = (if Char.IsUpper char then White else Black); Square = square }
        match Char.ToUpper char with
        | 'B' -> { piece with PieceType = Bishop }
        | 'N' -> { piece with PieceType = Knight }
        | 'R' -> { piece with PieceType = Rook }
        | 'Q' -> { piece with PieceType = Queen }
        | 'K' -> { piece with PieceType = King }
        | _ -> { piece with PieceType = Pawn }

    let filterPiecesByColor color pieces =
        pieces
        |> List.filter (fun piece -> piece.Color = color)

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
