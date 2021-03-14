module Client.App.ChessBoard.RecordGameState

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
open RecordGameTypes
open Elmish
open ChessPieces
open SquareCoverage
open SquareStyles
open FEN
open ParseMove
open System

/// Uses Fable's Emit to call JavaScript directly and play sounds
[<Emit("(new Audio($0)).play();")>]
let playSound (fileName: string) = jsNative

let delayMsg ((count, returnVal): int*'a) =
    async { do! Async.Sleep count
            return returnVal }

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
      PreviousPieceCount = pieces.Length
      ChessGame = ChessGame.defaultGame
      WhitePlayer = ChessPlayer.defaultPlayer
      WhitePlayerImage = ""
      BlackPlayer = ChessPlayer.defaultPlayer
      BlackPlayerImage = ""
      WhiteToMove = true
      MovesList = List.empty
      SquareStyles = squareStyle
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
        let wImage = "/Images/" + ChessPlayer.getPlayerName whitePlayer + ".jpg"
        let bImage = "/Images/" + ChessPlayer.getPlayerName blackPlayer + ".jpg"

        let freshModel, _ = init model.Api
        { freshModel with ChessGame = chessGame
                          WhitePlayer = whitePlayer
                          WhitePlayerImage = wImage
                          BlackPlayer = blackPlayer
                          BlackPlayerImage = bImage
                          MovesList = chessGame.MovesList |> List.ofArray }, StartRecording |> Internal |> Cmd.ofMsg

    | StartGame _ ->
        window.alert "Received invalid parameter data in StartGame. Unable to start the game"
        model, Cmd.none

    | StartRecording ->
    //    let wSquareCoverage, bSquareCoverage = getSquareCoverage model.AllPieces
        
    //    let squareStyle = createSquareStyleObject wSquareCoverage bSquareCoverage
        
        model, [ Cmd.OfAsync.perform delayMsg (moveDuration, ()) ParseMove |> Cmd.map Internal ]
               |> Cmd.batch

    | ParseMove () when model.MovesList.IsEmpty || model.MovesList.Head |> String.IsNullOrWhiteSpace ->
        let squareStyle =
            match model.ChessGame.Result with
            | Draw ->
                model.SquareStyles
            | WhiteWin ->
                model.AllPieces
                |> List.find (fun piece -> piece.Color = Black && piece.PieceType = King)
                |> createGameOverStyle
            | BlackWin ->
                model.AllPieces
                |> List.find (fun piece -> piece.Color = White && piece.PieceType = King)
                |> createGameOverStyle

        if model.ChessGame.Result = Draw then
            playSound "/Sounds/draw.wav"
        else
            playSound "/Sounds/victory.wav"

        { model with SquareStyles = squareStyle }, Cmd.OfAsync.either model.Api.CreateTextFile (model.WhitePlayer, model.BlackPlayer, model.ChessGame) CreateTextFile HandleExn |> Cmd.map Internal


    | ParseMove () when model.MovesList.Head.StartsWith("O") ->
        model, Cmd.OfAsync.perform delayMsg (moveDuration/2, ()) ParseCastle |> Cmd.map Internal

    | ParseCastle () ->
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
                     WhiteToMove = model.WhiteToMove |> not
                     SquareStyles = squareStyle },
            [ squareStyle |> UpdateSquareStyles |> Internal |> Cmd.ofMsg
              Cmd.OfAsync.perform delayMsg (moveDuration/2, ()) ParseMove |> Cmd.map Internal ]
            |> Cmd.batch
                

    | ParseMove () ->
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
            [ Cmd.OfAsync.perform delayMsg (transitionDuration, squareStyle) UpdateSquareStyles |> Cmd.map Internal 
              Cmd.OfAsync.perform delayMsg (moveDuration, ()) ParseMove |> Cmd.map Internal ]
            |> Cmd.batch
        
        

    | UpdateSquareStyles squareStyles ->
        if model.AllPieces.Length < model.PreviousPieceCount then
            playSound "/Sounds/capture.mp3"
        else
            playSound "/Sounds/move.mp3"

        { model with SquareStyles = squareStyles
                     PreviousPieceCount = model.AllPieces.Length }, Cmd.none

    | UpdateErrorString string ->
        JS.console.error string
        { model with ErrorString = string }, Cmd.none

    | HandleExn exn ->
        { model with Exn = Some exn }, exn.Message |> UpdateErrorString |> Internal |> Cmd.ofMsg

    | CreateTextFile _ ->
        model, Cmd.none