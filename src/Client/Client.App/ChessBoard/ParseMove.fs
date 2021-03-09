module Client.App.ChessBoard.ParseMove

open ChessPieces
open ChessSquares
open SquareCoverage

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



let parseMove (moveString:string) (pieces: Piece list) (colorToMove: PieceColor) =
    match moveString.[0] with  
    | 'O' when moveString.Contains "O-O-O" ->
        pieces
        |> List.map
            (fun piece ->
                match piece.Color = colorToMove, piece.PieceType, piece.Color with
                | false, _, _ -> piece
                | _, King, White -> { piece with Square = Square.create 3 1 }
                | _, King, Black -> { piece with Square = Square.create 3 8 }
                | _, Rook, White when piece.Square = Square.create 1 1 -> { piece with Square = Square.create 4 1}
                | _, Rook, Black when piece.Square = Square.create 1 8 -> { piece with Square = Square.create 4 8}
                | _ -> piece)
    | 'O' ->
        pieces
        |> List.map
            (fun piece ->
                match piece.Color = colorToMove, piece.PieceType, piece.Color with
                | false, _, _ -> piece
                | _, King, White -> { piece with Square = Square.create 7 1 }
                | _, King, Black -> { piece with Square = Square.create 7 8 }
                | _, Rook, White when piece.Square = Square.create 8 1 -> { piece with Square = Square.create 6 1}
                | _, Rook, Black when piece.Square = Square.create 8 8 -> { piece with Square = Square.create 6 8}
                | _ -> piece)
    | _ when (moveString.Contains '=') ->
        // Promotion
        let moveString = moveString.Trim()
        let pawnFile = moveString.[0] |> parseFile
        let pawnSquare = Square.create pawnFile 7

        let promotionFile =
            if moveString.Length = 5 then
                moveString.[1]
            else
                moveString.[0]
            |> parseFile

        let promotionSquare = Square.create promotionFile 8
        let piece = Piece.fromChar moveString.[moveString.Length-1] promotionSquare

        pieces
        |> List.filter (fun piece -> piece.Square <> promotionSquare && piece.Square <> pawnSquare)
        |> List.append [piece]

    | _ ->
        let moveString = moveString.Trim()
        let rank, file = moveString.[moveString.Length-1] |> string |> int, parseFile moveString.[moveString.Length-2]
        let square = Square.create file rank

        let pieces =
            pieces
            |> List.filter (fun piece -> piece.Square <> square)

        let simpleUpdate pieceType =
            pieces
            |> List.map
                (fun piece ->
                    if piece.PieceType = pieceType && piece.Color = colorToMove && List.contains square (getPieceMovement piece pieces) then { piece with Square = square }
                    else piece )
        let complexUpdate pieceType =
            let filterFunction = parseUnknown moveString.[1]
            pieces
            |> List.map
                (fun piece ->
                    if piece.PieceType = pieceType && piece.Color = colorToMove && filterFunction piece then { piece with Square = square }
                    else piece )
        match moveString.[0] with
        | 'K' ->
            simpleUpdate King
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
        | _ when moveString.Length = 2 ->
            simpleUpdate Pawn
        | _ ->
            pieces
            |> List.map
                (fun piece ->
                    if piece.Color = colorToMove &&
                       piece.PieceType = Pawn &&
                       piece.Square = Square.create (parseFile moveString.[0]) (if piece.Color = White then rank - 1 else rank + 1) then
                        { piece with Square = square }
                    else piece )
