module Client.App.State

open Types
open Elmish
open Fable.Core
open Browser
open Browser.Types
open Shared



let init api (): Model * Cmd<Msg> =
    let cmd = Cmd.OfAsync.perform api.getChessGames () GotChessGames
    JS.console.log "State.init"

    let cBMdl, cBCmd = ChessBoard.State.init api
    let cPMdl, cPCmd = ChessPlayers.State.init api

    { Api = api
      ChessBoardModel = cBMdl
      ChessPlayersModel = cPMdl
      ChessGames = []
      ActiveTab = TabsType.AddPlayer },
        [ cBCmd |> Cmd.map ChessBoardMsg
          cPCmd |> Cmd.map ChessPlayersMsg
          cmd ]
        |> Cmd.batch

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with
    | GotChessGames todos ->
        JS.console.log "Canvas recorder starting"
        { model with ChessGames = todos }, Cmd.none
    | AddChessGame ->
        //let todo = ChessGame.create model.Input
        //let cmd = Cmd.OfAsync.perform todosApi.addChessGame todo AddedTodo
        model, Cmd.none // cmd
    | AddedChessGame todo ->
        { model with ChessGames = model.ChessGames @ [ todo ] }, Cmd.none

    | StartRecording startRecording->
        let canvas = document.getElementsByName("RecordingCanvas").[0] :?> HTMLCanvasElement

        JS.console.log "Start Recording"

        startRecording()

        model, Cmd.none

    | SetTab tabType ->
        { model with ActiveTab = tabType }, Cmd.none

    | ChessBoardMsg msg ->
        let cMdl, cCmd = ChessBoard.State.update msg model.ChessBoardModel
        { model with ChessBoardModel = cMdl }, cCmd |> Cmd.map ChessBoardMsg

    | ChessPlayersMsg (ChessPlayers.Types.Internal msg) ->
        let cMdl, cCmd = ChessPlayers.State.update msg model.ChessPlayersModel
        { model with ChessPlayersModel = cMdl }, cCmd |> Cmd.map ChessPlayersMsg

    | ChessPlayersMsg (ChessPlayers.Types.External (ChessPlayers.Types.UpdatedPlayers updatedPlayers)) ->
        model, Cmd.none