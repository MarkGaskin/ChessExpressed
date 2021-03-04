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
      ChessPlayerInput = ChessPlayer.defaultPlayer
      ErrorString = "" }, Cmd.none

let update msg (model:Model) =
    match msg with
    | AddPlayer chessPlayer ->
        model.ChessPlayers
        |> List.exists (fun chessPlayer -> ChessPlayer.isEqual chessPlayer chessPlayer)
        |> function
           | true -> model, "Player with the provided name already exists in database" |> UpdateErrorString |> Cmd.ofMsg
           | false -> model, Cmd.OfAsync.perform model.Api.addChessPlayer model.ChessPlayerInput AddedPlayer

    | AddedPlayer chessPlayer ->
        { model with ChessPlayers = chessPlayer :: model.ChessPlayers}, Cmd.none

    | EditFirstName string ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with FirstName = string }}, Cmd.none

    | EditLastName string ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with LastName = string }}, Cmd.none

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