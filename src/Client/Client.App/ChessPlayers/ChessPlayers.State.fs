module Client.App.ChessPlayers.State

open Shared
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

let init api =
    { Api = api
      ChessPlayers = []
      DisplayedChessPlayers = []
      ChessPlayerInput = ChessPlayer.defaultPlayer
      ErrorString = ""
      Exn = None }, Cmd.OfAsync.perform api.getChessPlayers () GotChessPlayers |> Cmd.map Internal

let update msg (model:Model) =
    match msg with
    | GotChessPlayers chessPlayers ->
        { model with ChessPlayers = chessPlayers }, UpdateDisplayedChessPlayers |> Internal |> Cmd.ofMsg

    | AddPlayer addedPlayer ->
        model.ChessPlayers
        |> List.exists (fun chessPlayer -> ChessPlayer.isEqual chessPlayer addedPlayer)
        |> function
           | true -> model, "Player with the provided name already exists in database" |> UpdateErrorString |> Internal |> Cmd.ofMsg
           | false -> model, Cmd.OfAsync.either model.Api.addChessPlayer model.ChessPlayerInput AddedPlayer HandleExn |> Cmd.map Internal

    | AddedPlayer chessPlayer ->
        let updatedPlayers = chessPlayer :: model.ChessPlayers
        { model with ChessPlayers = updatedPlayers
                     ChessPlayerInput = ChessPlayer.defaultPlayer }, [ UpdateDisplayedChessPlayers |> Internal |> Cmd.ofMsg
                                                                       updatedPlayers |> UpdatedPlayers |> External |> Cmd.ofMsg ]
                                                                     |> Cmd.batch

    | DeletePlayer chessPlayer ->
        model, Cmd.OfAsync.either model.Api.deleteChessPlayer chessPlayer DeletedPlayer HandleExn |> Cmd.map Internal

    | DeletedPlayer deletedPlayer ->
        let updatedPlayers = List.filter (fun (chessPlayer:ChessPlayer) -> chessPlayer.Id = deletedPlayer.Id |> not) model.ChessPlayers
        { model with ChessPlayers = updatedPlayers}, [ UpdateDisplayedChessPlayers |> Internal |> Cmd.ofMsg
                                                       updatedPlayers |> UpdatedPlayers |> External |> Cmd.ofMsg ]
                                                     |> Cmd.batch

    | EditFirstName string ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with FirstName = string }}, UpdateDisplayedChessPlayers |> Internal |> Cmd.ofMsg

    | EditLastName string ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with LastName = string }}, UpdateDisplayedChessPlayers |> Internal |> Cmd.ofMsg

    | EditNickName string ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with NickName = string }}, Cmd.none

    | EditTwitchChannel string ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with TwitchChannel = string }}, Cmd.none

    | EditYouTubeChannel string ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with YouTubeChannel = string }}, Cmd.none

    | EditTwitterHandle string ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with TwitterHandle = string }}, Cmd.none

    | UpdateErrorString string ->
        { model with ErrorString = string }, Cmd.none

    | HandleExn exn ->
        { model with Exn = Some exn }, Cmd.none

    | UpdateDisplayedChessPlayers ->
        model.ChessPlayers
        |> List.filter(fun chessPlayer -> chessPlayer.FirstName.StartsWith(model.ChessPlayerInput.FirstName) &&
                                          chessPlayer.LastName.StartsWith(model.ChessPlayerInput.LastName))
        |> fun matchingPlayers -> { model with DisplayedChessPlayers = matchingPlayers }, Cmd.none