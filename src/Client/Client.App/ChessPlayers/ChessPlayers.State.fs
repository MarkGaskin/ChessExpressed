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
      SelectedChessPlayer = None
      ChessPlayerInput = ChessPlayer.defaultPlayer
      ErrorString = ""
      Exn = None }, Cmd.OfAsync.perform api.getChessPlayers () GotChessPlayers |> Cmd.map Internal

let update msg (model:Model) =
    match msg with
    | GotChessPlayers chessPlayers ->
        { model with ChessPlayers = chessPlayers },
            [ UpdateDisplayedChessPlayers |> Internal |> Cmd.ofMsg
              chessPlayers |> UpdatedPlayers |> External |> Cmd.ofMsg ]
            |> Cmd.batch

    | AddPlayer newPlayer when ChessPlayer.isValid newPlayer ->
        model.ChessPlayers
        |> List.exists (fun chessPlayer -> ChessPlayer.isEqual chessPlayer newPlayer)
        |> function
           | true -> model, "Player with the provided name already exists in database" |> UpdateErrorString |> Internal |> Cmd.ofMsg
           | false -> model, Cmd.OfAsync.either model.Api.addChessPlayer model.ChessPlayerInput AddedPlayer HandleExn |> Cmd.map Internal

    | AddPlayer _ ->
        model, "Player to be added was not valid" |> UpdateErrorString |> Internal |> Cmd.ofMsg

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
        { model with ChessPlayers = updatedPlayers
                     SelectedChessPlayer = if Option.contains deletedPlayer model.SelectedChessPlayer then None else model.SelectedChessPlayer },
            [ UpdateDisplayedChessPlayers |> Internal |> Cmd.ofMsg
              updatedPlayers |> UpdatedPlayers |> External |> Cmd.ofMsg ]
            |> Cmd.batch

    | SelectPlayer selectedPlayer ->
        { model with SelectedChessPlayer = selectedPlayer |> Some }, Cmd.none

    | EditFirstName (EditPlayerType.New, string) ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with FirstName = string }}, UpdateDisplayedChessPlayers |> Internal |> Cmd.ofMsg

    | EditLastName (EditPlayerType.New, string) ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with LastName = string }}, UpdateDisplayedChessPlayers |> Internal |> Cmd.ofMsg

    | EditNickName (EditPlayerType.New, string) ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with NickName = string }}, Cmd.none

    | EditTwitchChannel (EditPlayerType.New, string) ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with TwitchChannel = string }}, Cmd.none

    | EditYouTubeChannel (EditPlayerType.New, string) ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with YouTubeChannel = string }}, Cmd.none

    | EditTwitterHandle (EditPlayerType.New, string) ->
        { model with ChessPlayerInput = { model.ChessPlayerInput with TwitterHandle = string }}, Cmd.none
        
    | EditFirstName (EditPlayerType.Selected, string) ->
        { model with SelectedChessPlayer = Option.map (fun player -> { player with FirstName = string }) model.SelectedChessPlayer }, Cmd.none
        
    | EditLastName (EditPlayerType.Selected, string) ->
        { model with SelectedChessPlayer = Option.map (fun player -> { player with LastName = string }) model.SelectedChessPlayer }, Cmd.none
        
    | EditNickName (EditPlayerType.Selected, string) ->
        { model with SelectedChessPlayer = Option.map (fun player -> { player with NickName = string }) model.SelectedChessPlayer }, Cmd.none
        
    | EditTwitchChannel (EditPlayerType.Selected, string) ->
        { model with SelectedChessPlayer = Option.map (fun player -> { player with TwitchChannel = string }) model.SelectedChessPlayer }, Cmd.none
        
    | EditYouTubeChannel (EditPlayerType.Selected, string) ->
        { model with SelectedChessPlayer = Option.map (fun player -> { player with YouTubeChannel = string }) model.SelectedChessPlayer }, Cmd.none
        
    | EditTwitterHandle (EditPlayerType.Selected, string) ->
        { model with SelectedChessPlayer = Option.map (fun player -> { player with TwitterHandle = string }) model.SelectedChessPlayer }, Cmd.none

    | UpdatePlayer (Some player) when ChessPlayer.isValid player ->
        model, Cmd.OfAsync.either model.Api.updateChessPlayer player UpdatedPlayer HandleExn |> Cmd.map Internal

    | UpdatePlayer (Some player) ->
        model, sprintf "Player to be updated was not valid: %A" player |> UpdateErrorString |> Internal |> Cmd.ofMsg

    | UpdatePlayer None ->
        model, "ERROR: Tried to update player with None selected" |> UpdateErrorString |> Internal |> Cmd.ofMsg

    | UpdatedPlayer updatedChessPlayer ->
        let updatedPlayers =
            model.ChessPlayers
            |> List.map( fun chessPlayer -> if chessPlayer.Id = updatedChessPlayer.Id then updatedChessPlayer else chessPlayer)
        { model with ChessPlayers = updatedPlayers }, [ UpdateDisplayedChessPlayers |> Internal |> Cmd.ofMsg
                                                        updatedPlayers |> UpdatedPlayers |> External |> Cmd.ofMsg ]
                                                      |> Cmd.batch

    | UpdateErrorString string ->
        JS.console.error string
        { model with ErrorString = string }, Cmd.none

    | HandleExn exn ->
        { model with Exn = Some exn }, Cmd.none

    | UpdateDisplayedChessPlayers ->
        model.ChessPlayers
        |> List.filter(fun chessPlayer -> chessPlayer.FirstName.StartsWith(model.ChessPlayerInput.FirstName) &&
                                          chessPlayer.LastName.StartsWith(model.ChessPlayerInput.LastName))
        |> fun matchingPlayers -> { model with DisplayedChessPlayers = matchingPlayers }, Cmd.none