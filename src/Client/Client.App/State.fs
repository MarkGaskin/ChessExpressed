module Client.App.State

open Types
open Elmish
open Fable.Core
open Browser
open Browser.Types
open Shared



let init (api: ICEApi) (): Model * Cmd<Msg> =
    JS.console.log "State.init"

    let rMdl, rCmd = ChessBoard.RecordGameState.init api
    let pMdl, pCmd = ChessBoard.PrepareGameState.init api
    let cPMdl, cPCmd = ChessPlayers.State.init api
    let cGMdl, cGCmd = ChessGames.State.init api

    { Api = api
      RecordModel = rMdl
      PrepareModel = pMdl
      ChessPlayersModel = cPMdl
      ChessGamesModel = cGMdl
      ActiveTab = TabsType.AddPlayer },
        [ rCmd |> Cmd.map RecordMsg
          pCmd |> Cmd.map PrepareMsg
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

    | RecordMsg (ChessBoard.RecordGameTypes.Internal msg) ->
        let cMdl, cCmd = ChessBoard.RecordGameState.update msg model.RecordModel
        { model with RecordModel = cMdl }, cCmd |> Cmd.map RecordMsg

    | PrepareMsg (ChessBoard.PrepareGameTypes.Internal msg) ->
        let pMdl, pCmd = ChessBoard.PrepareGameState.update msg model.PrepareModel
        { model with PrepareModel = pMdl }, pCmd |> Cmd.map PrepareMsg

    | RecordMsg (ChessBoard.RecordGameTypes.External (ChessBoard.RecordGameTypes.GameRecorded chessGame)) ->
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
        let cMdl, cCmd = ChessBoard.PrepareGameState.update (ChessBoard.PrepareGameTypes.StartGame args) model.PrepareModel
        { model with PrepareModel = cMdl; ActiveTab = PrepareGame }, cCmd |> Cmd.map PrepareMsg

    | ChessPlayersMsg (ChessPlayers.Types.External (ChessPlayers.Types.UpdatedPlayers updatedPlayers)) ->
        let cMdl, cCmd = ChessGames.State.update (ChessGames.Types.GotChessPlayers updatedPlayers) model.ChessGamesModel
        { model with ChessGamesModel = cMdl }, cCmd |> Cmd.map ChessGamesMsg
