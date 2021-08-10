module Client.App.ChessBoard.SquareCoverage

open ChessPieces
open ChessSquares

let isStartingPieceBehindPawn startingPiece piece =
    if piece.Color = White then startingPiece.Square.Rank < piece.Square.Rank else startingPiece.Square.Rank > piece.Square.Rank

let rec private getMovementSquares (startingPiece:Piece) (direction: int*int) (allPieces: Piece list) =
    let newSquare = startingPiece.Square + direction
    if Square.isValid newSquare |> not then
        []
    elif (List.forall (fun piece -> newSquare <> piece.Square) allPieces) then
        List.append [newSquare] (getMovementSquares { startingPiece with Square = newSquare } direction allPieces)
    else
        allPieces
        |> List.find (fun piece -> newSquare = piece.Square)
        |> fun piece ->
            if piece.Color = startingPiece.Color then
                []
            else
                [newSquare]

let private getBishopMovementSquares (startingPiece:Piece) (allPieces: Piece list) =
    [(1,1); (-1,1); (1,-1); (-1,-1)]
    |> List.map
        (fun direction -> getMovementSquares startingPiece direction allPieces)
    |> List.concat

let private getRookMovementSquares (startingPiece:Piece) (allPieces: Piece list) =
    [(1,0); (-1,0); (0,-1); (0,1)]
    |> List.map
        (fun direction -> getMovementSquares startingPiece direction allPieces)
    |> List.concat

let private getQueenMovementSquares (startingPiece:Piece) (allPieces: Piece list) =
    [(1,0); (-1,0); (0,-1); (0,1); (1,1); (-1,1); (1,-1); (-1,-1)]
    |> List.map
        (fun direction -> getMovementSquares startingPiece direction allPieces)
    |> List.concat


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
    | Bishop, _ -> getBishopMovementSquares piece allPieces
    | Knight, _ -> [(1,2); (-1,2); (-1,-2); (1,-2); (2,1); (-2,1); (2,-1); (-2,-1)]
                    |> List.map (fun direction -> piece.Square + direction)
                    |> List.filter Square.isValid
    | Rook, _ -> getRookMovementSquares piece allPieces
    | Queen, _ -> getQueenMovementSquares piece allPieces
                

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
            | _, Queen, Queen
            | _, Queen, Bishop when (fileDelta * rankDelta <> 0) ->
                List.append [newSquare] (getCoveredSquares { startingPiece with Square = newSquare } direction allPieces)

            | _, Rook, Queen
            | _, Rook, Rook
            | _, Queen, Queen
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
                            (fun rankDelta -> piece.Square + (fileDelta, rankDelta))
                        |> List.filter(fun square -> square <> piece.Square))
                    |> List.concat
                    |> List.filter Square.isValid
    | Bishop, _ -> getBishopCoveredSquares piece allPieces
    | Knight, _ -> [(1,2); (-1,2); (-1,-2); (1,-2); (2,1); (-2,1); (2,-1); (-2,-1)]
                    |> List.map (fun direction -> piece.Square + direction)
                    |> List.filter Square.isValid
    | Rook, _ -> getRookCoveredSquares piece allPieces
    | Queen, _ -> getQueenCoveredSquares piece allPieces

let getSquareCoverageByColor color allPieces =
    allPieces
    |> List.filter(fun piece -> piece.Color = color && piece.IsPinned |> not)
    |> List.map (fun piece ->
        getPieceCoverage piece allPieces)
    |> List.concat 

let getSquareCoverage allPieces =
    getSquareCoverageByColor White allPieces,
        getSquareCoverageByColor Black allPieces


let rec findAllPiecesInDirection king allPieces direction =
    let newSquare = king.Square + direction
    match newSquare |> Square.isValid,
          allPieces
          |> List.tryFind
              (fun piece -> piece.Square = newSquare) with
    | false, _ -> []
    | _, Some piece ->
        List.append [piece] (findAllPiecesInDirection {king with Square = newSquare} allPieces  direction)
    | _, None -> 
        List.append [] (findAllPiecesInDirection {king with Square = newSquare} allPieces  direction)

let isDirectionDiagonal direction =
    (direction |> fst) * (direction |> snd) <> 0

let checkPieceListForPin king direction pieceList =
    match List.tryItem 0 pieceList,
          List.tryItem 1 pieceList,
          isDirectionDiagonal direction with
    | Some firstPiece, _, _ when firstPiece.Color <> king.Color -> None
    | Some firstPiece, Some secondPiece, _ when firstPiece.Color = secondPiece.Color -> None
    | Some firstPiece, Some secondPiece, true ->
        { firstPiece with
            IsPinned = match secondPiece.PieceType, firstPiece.PieceType with
                       | Bishop, Bishop
                       | Bishop, Queen
                       | Queen, Bishop
                       | Queen, Queen -> false
                       | Bishop, _
                       | Queen, _ -> true
                       | _ -> false }
        |> Some
    | Some firstPiece, Some secondPiece, false ->
        { firstPiece with
            IsPinned = match secondPiece.PieceType, firstPiece.PieceType with
                       | Rook, Rook
                       | Rook, Queen
                       | Queen, Rook
                       | Queen, Queen -> false
                       | Rook, _
                       | Queen, _ -> true
                       | _ -> false }
        |> Some
    | _ -> None
        
        
let updatePinnedPieces allPieces =
    let allPieces =
        allPieces
        |> List.map (fun piece -> { piece with IsPinned = false })

    let directions =
        [-1..1]
        |> List.map
            (fun fileDelta ->
                [-1..1]
                |> List.map
                    (fun rankDelta -> fileDelta, rankDelta))
        |> List.concat
        |> List.filter
            (fun (fileDelta, rankDelta) -> fileDelta <> 0 || rankDelta <> 0)

    let kings =
        allPieces
        |> List.filter (fun piece -> piece.PieceType = King)

    let updatedPieces =     
        kings
        |> List.map
            (fun king ->
                directions
                |> List.map
                    (fun direction ->
                        let pieceList = findAllPiecesInDirection king allPieces direction
                        checkPieceListForPin king direction pieceList ))
        |> List.concat
        |> List.choose id

    allPieces
    |> List.map
        (fun piece ->
            if List.contains { piece with IsPinned = true } updatedPieces then
                { piece with IsPinned = true }
            else
                piece )



    


