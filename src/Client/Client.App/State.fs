module Client.App.State

open Types
open Elmish
open Fable.Core
open Browser
open Browser.Types
open Shared



let init (api: ICEApi) (): Model * Cmd<Msg> =
    JS.console.log "State.init"

    let cBMdl, cBCmd = ChessBoard.State.init api
    let cPMdl, cPCmd = ChessPlayers.State.init api
    let cGMdl, cGCmd = ChessGames.State.init api

    { Api = api
      ChessBoardModel = cBMdl
      ChessPlayersModel = cPMdl
      ChessGamesModel = cGMdl
      ActiveTab = TabsType.AddPlayer },
        [ cBCmd |> Cmd.map ChessBoardMsg
          cPCmd |> Cmd.map ChessPlayersMsg
          cGCmd |> Cmd.map ChessGamesMsg ]
        |> Cmd.batch

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with
    | StartRecording startRecording->
        let canvas = document.getElementsByName("RecordingCanvas").[0] :?> HTMLCanvasElement

        JS.console.log "Start Recording"

        startRecording()

        model, Cmd.none

    | SetTab tabType ->
        { model with ActiveTab = tabType }, Cmd.none

    | ChessBoardMsg (ChessBoard.Types.Internal msg) ->
        let cMdl, cCmd = ChessBoard.State.update msg model.ChessBoardModel
        { model with ChessBoardModel = cMdl }, cCmd |> Cmd.map ChessBoardMsg

    | ChessBoardMsg (ChessBoard.Types.External (ChessBoard.Types.GameRecorded chessGame)) ->
        // Update hasRecorded in db
        model, Cmd.none

    | ChessPlayersMsg (ChessPlayers.Types.Internal msg) ->
        let cMdl, cCmd = ChessPlayers.State.update msg model.ChessPlayersModel
        { model with ChessPlayersModel = cMdl }, cCmd |> Cmd.map ChessPlayersMsg

    | ChessGamesMsg (ChessGames.Types.Internal msg) ->
        let cMdl, cCmd = ChessGames.State.update msg model.ChessGamesModel
        { model with ChessGamesModel = cMdl }, cCmd |> Cmd.map ChessGamesMsg

    | ChessGamesMsg (ChessGames.Types.External (ChessGames.Types.ImportedGames)) ->
        let cMdl, cCmd = ChessPlayers.State.update ChessPlayers.Types.RefreshPlayers model.ChessPlayersModel
        { model with ChessPlayersModel = cMdl }, cCmd |> Cmd.map ChessPlayersMsg

    | ChessGamesMsg (ChessGames.Types.External (ChessGames.Types.ExternalMsg.StartGame args)) ->
        let cMdl, cCmd = ChessBoard.State.update (ChessBoard.Types.StartGame args) model.ChessBoardModel
        { model with ChessBoardModel = cMdl; ActiveTab = RecordGame }, cCmd |> Cmd.map ChessBoardMsg

    | ChessPlayersMsg (ChessPlayers.Types.External (ChessPlayers.Types.UpdatedPlayers updatedPlayers)) ->
        let cMdl, cCmd = ChessGames.State.update (ChessGames.Types.GotChessPlayers updatedPlayers) model.ChessGamesModel
        { model with ChessGamesModel = cMdl }, cCmd |> Cmd.map ChessGamesMsg
