module Client.App.ChessBoard.FEN

open ChessPieces
open ChessSquares
open System

let rec concatFen (fenString: string list) current =
    if List.isEmpty fenString then
        if current > 0 then [current |> string]
        else []
    elif fenString.Head = "1" then
        concatFen fenString.Tail (current + 1)
    elif current > 0 then
        List.append [ current |> string; fenString.Head ] (concatFen fenString.Tail 0)
    else
        List.append [ fenString.Head ] (concatFen fenString.Tail 0)

let createRankFen (pieces: Piece list) =
    let startString = Array.create 8 '1'
    pieces
    |> List.fold
        (fun fenString piece ->
            fenString
            |> Array.mapi
                (fun i c ->
                    if i+1 = piece.Square.File then Piece.toChar piece
                    else c)) startString
    |> fun rankString ->
        concatFen (rankString |> Array.toList |> List.map string) 0
    |> fun strings ->
        String.concat "" strings

let canCastleFen (allPieces: Piece list) =
    ( allPieces
      |> List.tryFind ((=) {PieceType = Rook; Square = Square.create 8 1; Color = White; IsPinned = false })
      |> function
         | Some piece -> "K"
         | None -> "" )
    +
    ( allPieces
      |> List.tryFind ((=) {PieceType = Rook; Square = Square.create 1 1; Color = White; IsPinned = false })
      |> function
         | Some piece -> "Q"
         | None -> "" )
    +
    ( allPieces
      |> List.tryFind ((=) {PieceType = Rook; Square = Square.create 1 8; Color = Black; IsPinned = false })
      |> function
         | Some piece -> "k"
         | None -> "" )
    +
    ( allPieces
      |> List.tryFind ((=) {PieceType = Rook; Square = Square.create 8 8; Color = Black; IsPinned = false })
      |> function
         | Some piece -> "q"
         | None -> "" )

let createFen (allPieces: Piece list) colorToMove  =
    [1..8]
    |> List.map
        (fun rank ->
            allPieces
            |> List.filter (fun piece -> piece.Square.Rank = (9 - rank))
            |> createRankFen )
    |> fun strings ->
        String.concat "/" strings
    |> fun boardString ->
        boardString + if colorToMove = White then " w " else " b " + (canCastleFen allPieces) + " - 6 50"

let rec rankStringToPieces rank file (rankString : char list) =
    if rankString.IsEmpty then
        []
    elif rankString.Head |> Char.IsDigit then
        rankStringToPieces rank (file + (rankString.Head |> string |> int)) (rankString.Tail)
    else
        List.append [Piece.fromChar rankString.Head (Square.create file rank)]
                    (rankStringToPieces rank (file + 1) (rankString.Tail))

let fenToPieces (fenString : string) =
    fenString.Split('/')
    |> List.ofArray
    |> List.mapi
        (fun rankIdx rankString ->
            let rankStringList = Seq.toList rankString
            let rank = 8 - rankIdx
            rankStringToPieces rank 1 rankStringList
            )
    |> List.concat
            
