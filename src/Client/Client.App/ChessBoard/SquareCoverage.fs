module Client.App.ChessBoard.SquareCoverage

open ChessPieces
open ChessSquares

let isStartingPieceBehindPawn startingPiece piece =
    if piece.Color = White then startingPiece.Square.Rank < piece.Square.Rank else startingPiece.Square.Rank > piece.Square.Rank

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

            | _, Bishop, Bishop
            | _, Bishop, Queen
            | _, Queen, Bishop when (fileDelta * rankDelta <> 0) ->
                List.append [newSquare] (getCoveredSquares { startingPiece with Square = newSquare } direction allPieces)

            | _, Rook, Queen
            | _, Rook, Rook
            | _, Queen, Rook when (fileDelta * rankDelta = 0) ->
                List.append [newSquare] (getCoveredSquares { startingPiece with Square = newSquare } direction allPieces)

            | _, Bishop, Pawn when isStartingPieceBehindPawn startingPiece piece ->
                List.append [newSquare] ([newSquare+direction] |> List.filter Square.isValid)
            | _, Queen, Pawn when (fileDelta * rankDelta <> 0 && isStartingPieceBehindPawn startingPiece piece) ->
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


