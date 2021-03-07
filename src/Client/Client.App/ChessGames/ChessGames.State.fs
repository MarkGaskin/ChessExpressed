module Client.App.ChessGames.State

open Shared
open System
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
open Shared.CEError

let updateInputField (updateFun: ChessGame -> ChessGame) model editGameType =
    if editGameType = EditGameType.New then
        { model with ChessGameInput = updateFun model.ChessGameInput}, FilterChessGames |> Internal |> Cmd.ofMsg
    else
        { model with SelectedChessGame = Option.map updateFun model.SelectedChessGame }, Cmd.none

let init api =
    { Api = api
      ChessPlayers = []
      InputPlayer1Name = ""
      InputPlayer2Name = ""
      SelectedPlayer1Name = ""
      SelectedPlayer2Name = ""
      ChessGames = []
      DisplayedChessGames = []
      SelectedChessGame = None
      SelectedChessGameOriginal = None
      ChessGameInput = ChessGame.defaultGame
      ErrorString = ""
      ImportDirectory = "C:\\PGN"
      Exn = None }, Cmd.OfAsync.perform api.getChessGames () GotChessGames |> Cmd.map Internal

let update msg (model:Model) =
    let findPlayerNameById id =
        model.ChessPlayers
        |> List.tryFind
            (fun chessPlayer -> chessPlayer.Id = id)
        |> Option.map ChessPlayer.getPlayerName
        |> Option.defaultValue ""

    
    match msg with
    | AddBatchGames fileDirectory ->
        model, Cmd.OfAsync.either model.Api.ImportFromPath fileDirectory AddedBatchGames HandleExn |> Cmd.map Internal

    | AddedBatchGames (Ok ()) ->
        window.alert "Successfully imported games"
        model, Cmd.OfAsync.perform model.Api.getChessGames () GotChessGames |> Cmd.map Internal

    | AddedBatchGames (Error (FailedToImportGames e)) ->
        { model with Exn = e |> Some }, FailedToImportGames e |> ServerError.describe |> UpdateErrorString |> Internal |> Cmd.ofMsg

    | AddedBatchGames (Error e) ->
        model, e |> ServerError.describe |> UpdateErrorString |> Internal |> Cmd.ofMsg

    | EditImportDirectory importDirectory ->
        { model with ImportDirectory = importDirectory }, Cmd.none

    | StartGamePressed ->
        model, Cmd.none

    | GotChessGames chessGames ->
        { model with ChessGames = chessGames }, FilterChessGames |> Internal |> Cmd.ofMsg

    | GotChessPlayers chessPlayers ->
        { model with ChessPlayers = chessPlayers }, Cmd.none

    | AddGame newGame when ChessGame.isValid newGame ->
        model.ChessGames
        |> List.exists (ChessGame.isRoughlyEqual newGame)
        |> function
           | true -> model, "Game already exists in database" |> UpdateErrorString |> Internal |> Cmd.ofMsg
           | false -> model, Cmd.OfAsync.either model.Api.addChessGame model.ChessGameInput AddedGame HandleExn |> Cmd.map Internal

    | AddGame _ ->
        model, "Game to be added was not valid" |> UpdateErrorString |> Internal |> Cmd.ofMsg

    | AddedGame chessGame ->
        let updatedGames = chessGame :: model.ChessGames
        { model with ChessGames = updatedGames
                     ChessGameInput = ChessGame.defaultGame }, [ FilterChessGames |> Internal |> Cmd.ofMsg
                                                                 updatedGames |> UpdatedGames |> External |> Cmd.ofMsg ]
                                                                     |> Cmd.batch

    | DeleteGame chessGame ->
        model, Cmd.OfAsync.either model.Api.deleteChessGame chessGame DeletedGame HandleExn |> Cmd.map Internal

    | DeletedGame deletedGame ->
        let updatedGames = List.filter (fun (chessGame:ChessGame) -> chessGame.Id = deletedGame.Id |> not) model.ChessGames
        { model with ChessGames = updatedGames
                     SelectedChessGame = if Option.contains deletedGame model.SelectedChessGame then None else model.SelectedChessGame },
            [ FilterChessGames |> Internal |> Cmd.ofMsg
              updatedGames |> UpdatedGames |> External |> Cmd.ofMsg ]
            |> Cmd.batch

    | SelectGame selectedGame ->
        { model with SelectedChessGame = selectedGame |> Some
                     SelectedChessGameOriginal = selectedGame |> Some
                     SelectedPlayer1Name = findPlayerNameById selectedGame.PlayerIds.[0]
                     SelectedPlayer2Name = findPlayerNameById selectedGame.PlayerIds.[1] }, Cmd.none

    | EditPlayerName (editGameType, playerName, playerIndex) ->
        model.ChessPlayers
        |> List.tryFind (fun chessPlayer -> ChessPlayer.getPlayerName chessPlayer = playerName)
        |> function
           | Some matchingPlayer ->
                updateInputField
                    (fun game ->
                        { game with PlayerIds = game.PlayerIds
                                              |> List.mapi
                                                  (fun idx playerId ->
                                                       if idx = playerIndex then matchingPlayer.Id
                                                       else playerId ) }) { model with InputPlayer1Name = if playerIndex = 0 then playerName else model.InputPlayer1Name
                                                                                       InputPlayer2Name = if playerIndex = 1 then playerName else model.InputPlayer2Name } editGameType
           | None ->
                if playerIndex = 0 then
                    { model with InputPlayer1Name = playerName }, Cmd.none
                else
                    { model with InputPlayer2Name = playerName }, Cmd.none

    | EditPlayerName (editGameType, playerName, playerIndex) ->
        let matchingPlayerId = 
            model.ChessPlayers
            |> List.tryFind (fun chessPlayer -> ChessPlayer.getPlayerName chessPlayer = playerName)
            |> function
               | Some matchingPlayer -> matchingPlayer.Id
               | None -> Guid.Empty

        updateInputField
            (fun game ->
                { game with
                    PlayerIds = game.PlayerIds
                                |> List.mapi
                                    (fun idx playerId ->
                                        if idx = playerIndex then matchingPlayerId
                                        else playerId ) })
            ( match playerIndex, editGameType with
                | 0, EditGameType.New -> { model with InputPlayer1Name = playerName }
                | 0, EditGameType.Selected -> { model with InputPlayer2Name = playerName }
                | 1, EditGameType.New -> { model with SelectedPlayer1Name = playerName }
                | 1, EditGameType.Selected -> { model with SelectedPlayer2Name = playerName }
                | _ -> model ) editGameType
           

    | EditEloWhite (editGameType, elo) ->
        updateInputField (fun game -> { game with EloWhite = Some elo }) model editGameType

    | EditEloBlack (editGameType, elo) ->
        updateInputField (fun game -> { game with EloBlack = Some elo }) model editGameType

    | EditYear (editGameType, dateTime) ->
        updateInputField (fun game -> { game with Year = Some dateTime }) model editGameType

    | EditEvent (editGameType, event) ->
        updateInputField (fun game -> { game with Event = Some event }) model editGameType

    | EditResult (editGameType, chessGameResult) ->
        updateInputField (fun game -> { game with Result = chessGameResult }) model editGameType 

    | EditGameNotation (editGameType, gameNotation) ->
        updateInputField (fun game -> { game with GameNotation = gameNotation }) model editGameType

    | EditHasRecorded (editGameType, hasRecorded) ->
        updateInputField (fun game -> { game with HasRecorded = hasRecorded }) model editGameType

    | UpdateGame (Some game) when ChessGame.isValid game ->
        model, Cmd.OfAsync.either model.Api.updateChessGame game UpdatedGame HandleExn |> Cmd.map Internal

    | UpdateGame (Some game) ->
        model, sprintf "Game to be updated was not valid: %A" game |> UpdateErrorString |> Internal |> Cmd.ofMsg

    | UpdateGame None ->
        model, "ERROR: Tried to update Game with None selected" |> UpdateErrorString |> Internal |> Cmd.ofMsg

    | CancelUpdate ->
        { model with SelectedChessGame = model.SelectedChessGameOriginal }, Cmd.none

    | UpdatedGame updatedChessGame ->
        let updatedGames =
            model.ChessGames
            |> List.map( fun chessGame -> if chessGame.Id = updatedChessGame.Id then updatedChessGame else chessGame)
        { model with ChessGames = updatedGames }, FilterChessGames |> Internal |> Cmd.ofMsg

    | UpdateErrorString string ->
        JS.console.error string
        { model with ErrorString = string }, Cmd.none

    | HandleExn exn ->
        { model with Exn = Some exn }, Cmd.none

    | FilterChessGames ->
        model.ChessGames
        |> List.filter
                (fun chessGame ->
                    if List.isEmpty model.ChessGameInput.PlayerIds then true
                    else
                        model.ChessGameInput.PlayerIds
                        |> List.forall
                            (fun playerId ->
                                if Guid.Empty = playerId then true else List.contains playerId chessGame.PlayerIds) &&
                                (if model.ChessGameInput.Result = Draw then true
                                 else chessGame.Result = model.ChessGameInput.Result) &&
                                (if Option.forall String.IsNullOrWhiteSpace model.ChessGameInput.Year then true
                                 else chessGame.Year = model.ChessGameInput.Year))
        |> fun list -> List.take (min 20 list.Length) list
        |> fun matchingGames -> { model with DisplayedChessGames = matchingGames }, Cmd.none

    | FilterChessGames ->
        model, Cmd.none