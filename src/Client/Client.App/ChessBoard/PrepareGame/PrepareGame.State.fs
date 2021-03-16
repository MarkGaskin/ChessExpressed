module Client.App.ChessBoard.PrepareGameState

open Shared
open Shared.CEError
open Fable.Remoting.Client
open Fable.React
open Fable.Core.JsInterop
open Browser
open Browser.Types
open Browser.Blob
open Fable.Core
open Fable.React.Props
open PrepareGameTypes
open Elmish
open ChessPieces
open SquareCoverage
open SquareStyles
open FEN
open ParseMove
open System


let init api =

    let startFEN = "r1bq1r2/b1p2p2/p1pp1n1p/4p2k/3PP2B/2P3Q1/PP1N1PPP/R3R1K1 b - - 12 20"

    let startFEN = "start"

    let pieces = if startFEN = "start" then (Piece.initPieces ())
                 else fenToPieces startFEN

    let wSquareCoverage, bSquareCoverage = getSquareCoverage pieces

    let squareStyle = createSquareStyleObject wSquareCoverage bSquareCoverage

    { Api = api
      FENPosition = startFEN
      AllPieces = pieces
      GameIndex = 0
      ChessGame = ChessGame.defaultGame
      WhitePlayer = ChessPlayer.defaultPlayer
      BlackPlayer = ChessPlayer.defaultPlayer
      CastleMoveNumbers = [||]
      WhiteToMove = true
      MovesList = List.empty
      SquareStyles = [|squareStyle|]
      FENArray = [|startFEN|]
      ErrorString = ""
      Exn = None }, Cmd.none // Cmd.OfAsync.either api.UpdateECOs () UpdatedEcos HandleExn

let update msg (model:Model) =
    match msg with
    | UpdatedEcos (Ok()) ->
        JS.console.warn "Updated ecos"
        model, Cmd.none

    | UpdatedEcos (Error e) ->
        JS.console.error (e |> ServerError.describe)
        model, Cmd.none

    | StartGame (Some chessGame, Some whitePlayer, Some blackPlayer) ->
        let freshModel, _ = init model.Api
        { freshModel with ChessGame = chessGame
                          WhitePlayer = whitePlayer
                          BlackPlayer = blackPlayer
                          MovesList = chessGame.MovesList |> List.ofArray }, ParseMove |> Internal |> Cmd.ofMsg

    | StartGame _ ->
        window.alert "Received invalid parameter data in StartGame. Unable to start the game"
        model, Cmd.none

    | ParseMove when ((model.MovesList.IsEmpty || model.MovesList.Head |> String.IsNullOrWhiteSpace)) ->
        let squareStyle =
            match model.ChessGame.Result with
            | WhiteWin ->
                model.AllPieces
                |> List.find (fun piece -> piece.Color = Black && piece.PieceType = King)
                |> createGameOverStyle
            | BlackWin ->
                model.AllPieces
                |> List.find (fun piece -> piece.Color = White && piece.PieceType = King)
                |> createGameOverStyle
            | Draw ->
                model.SquareStyles.[model.SquareStyles.Length-1]

        model, [ squareStyle |> UpdateSquareStyles |> Internal |> Cmd.ofMsg
                 GameComplete |> Internal |> Cmd.ofMsg ]
               |> Cmd.batch

    | GameComplete ->
        model, [ Cmd.OfAsync.either model.Api.CreateTextFile (model.WhitePlayer, model.BlackPlayer, model.ChessGame) CreateTextFile HandleExn |> Cmd.map Internal
                 Cmd.OfAsync.either model.Api.CreateJSFile
                    (model.FENArray, model.SquareStyles, ChessPlayer.getPlayerName model.WhitePlayer, ChessPlayer.getPlayerName model.BlackPlayer, model.CastleMoveNumbers)
                    CreateTextFile HandleExn |> Cmd.map Internal ]
               |> Cmd.batch


    | ParseMove when model.MovesList.Head.StartsWith("O") ->
        model, ParseCastle |> Internal |> Cmd.ofMsg

    | ParseCastle ->
        let move = model.MovesList.Head

        let moveNumber = model.ChessGame.MovesList.Length - model.MovesList.Length
        
        let newPieceList =
            if model.WhiteToMove then
                parseMove move model.AllPieces White
            else
                parseMove move model.AllPieces Black
        
        let wSquareCoverage, bSquareCoverage = getSquareCoverage newPieceList
        
        let squareStyle = createSquareStyleObject wSquareCoverage bSquareCoverage
        
        { model with MovesList = model.MovesList.Tail
                     AllPieces = newPieceList
                     CastleMoveNumbers = Array.append model.CastleMoveNumbers [|moveNumber|]
                     FENPosition = createFen newPieceList (if model.WhiteToMove then White else Black)
                     WhiteToMove = model.WhiteToMove |> not },
            [ squareStyle |> UpdateSquareStyles |> Internal |> Cmd.ofMsg
              ParseMove |> Internal |> Cmd.ofMsg ]
            |> Cmd.batch
                

    | ParseMove ->
        let move = model.MovesList.Head

        let newPieceList =
            if model.WhiteToMove then
                parseMove move model.AllPieces White
            else
                parseMove move model.AllPieces Black

        let wSquareCoverage, bSquareCoverage = getSquareCoverage newPieceList

        let squareStyle = createSquareStyleObject wSquareCoverage bSquareCoverage

        { model with MovesList = model.MovesList.Tail
                     AllPieces = newPieceList
                     FENPosition = createFen newPieceList (if model.WhiteToMove then White else Black)
                     WhiteToMove = model.WhiteToMove |> not },
            [ squareStyle |> UpdateSquareStyles |> Internal |> Cmd.ofMsg
              ParseMove |> Internal |> Cmd.ofMsg ]
            |> Cmd.batch
        
        

    | UpdateSquareStyles squareStyle ->
        let newFEN = createFen model.AllPieces (if model.WhiteToMove then White else Black)
        { model with SquareStyles = Array.append model.SquareStyles [|squareStyle|]
                     FENArray = Array.append model.FENArray [|newFEN|] }, Cmd.none

    | UpdateErrorString string ->
        JS.console.error string
        { model with ErrorString = string }, Cmd.none

    | HandleExn exn ->
        { model with Exn = Some exn }, exn.Message |> UpdateErrorString |> Internal |> Cmd.ofMsg

    | CreateTextFile _ ->
        model, Cmd.none

    | MoveForward ->
        let newIdx =
            max 
                (min (model.GameIndex + 1) (model.ChessGame.MovesList.Length - 1))
                0
        { model with GameIndex = model.GameIndex + 1 }, Cmd.none

    | MoveBackward ->
        let newIdx =
            max 
                (min (model.GameIndex - 1) (model.ChessGame.MovesList.Length - 1))
                0

        { model with GameIndex = newIdx }, Cmd.none
