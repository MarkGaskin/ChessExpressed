module Client.App.ChessBoard.State

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
open Types
open Elmish
open ChessPieces

let delayMsg ((count, returnVal): int*'a) =
    async { Async.Sleep count |> ignore; return returnVal }

let init api =
    JS.console.warn "Updating ecos"
    { Api = api
      FENPosition = ""
      AllPieces = Piece.initPieces ()
      ChessGame = ChessGame.defaultGame
      WhitePlayer = ChessPlayer.defaultPlayer
      BlackPlayer = ChessPlayer.defaultPlayer
      WhiteToMove = true
      MovesList = List.empty
      SquareStyles = createEmpty
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
        { model with ChessGame = chessGame
                     WhitePlayer = whitePlayer
                     BlackPlayer = blackPlayer
                     MovesList = chessGame.MovesList |> List.ofArray }, StartRecording |> Internal |> Cmd.ofMsg

    | StartGame _ ->
        window.alert "Received invalid parameter data in StartGame. Unable to start the game"
        model, Cmd.none

    | StartRecording ->
        let wSquareCoverage, bSquareCoverage = getSquareCoverage model.AllPieces
        
        let squareStyle = createSquareStyleObject wSquareCoverage bSquareCoverage
        
        model, [ squareStyle |> UpdateSquareStyles |> Internal |> Cmd.ofMsg
                 Cmd.OfAsync.perform delayMsg (moveDuration, ()) ParseMove |> Cmd.map Internal ]
               |> Cmd.batch

    | ParseMove () when model.MovesList.IsEmpty |> not ->
        let move = model.MovesList.Head

        let newPieceList =
            if model.WhiteToMove then
                parseMove move (model.AllPieces |> Piece.filterPiecesByColor White)
                |> List.append (model.AllPieces |> Piece.filterPiecesByColor Black)
            else
                parseMove move (model.AllPieces |> Piece.filterPiecesByColor Black)
                |> List.append (model.AllPieces |> Piece.filterPiecesByColor White)

        let wSquareCoverage, bSquareCoverage = getSquareCoverage newPieceList

        let squareStyle = createSquareStyleObject wSquareCoverage bSquareCoverage

        { model with MovesList = model.MovesList.Tail
                     AllPieces = newPieceList
                     WhiteToMove = model.WhiteToMove |> not },
            [ Cmd.OfAsync.perform delayMsg (transitionDuration, squareStyle) UpdateSquareStyles |> Cmd.map Internal 
              Cmd.OfAsync.perform delayMsg (moveDuration, ()) ParseMove |> Cmd.map Internal ]
            |> Cmd.batch

    | ParseMove () ->
        model, Cmd.none
        

    | UpdateSquareStyles squareStyles ->
        { model with SquareStyles = squareStyles }, Cmd.none

    | UpdateErrorString string ->
        JS.console.error string
        { model with ErrorString = string }, Cmd.none

    | HandleExn exn ->
        { model with Exn = Some exn }, exn.Message |> UpdateErrorString |> Internal |> Cmd.ofMsg