module Client.App.ChessBoard.ChessSquares

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



