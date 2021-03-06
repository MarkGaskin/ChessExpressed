module Client.App.ChessGames.Types

open Shared
open System

type Model =
    { Api: ICEApi
      ChessPlayers: ChessPlayer list
      ChessGames: ChessGame list
      DisplayedChessGames: ChessGame list
      SelectedChessGame: ChessGame option
      SelectedChessGameOriginal: ChessGame option
      InputPlayer1Name: string
      InputPlayer2Name: string
      SelectedPlayer1Name: string
      SelectedPlayer2Name: string
      ChessGameInput: ChessGame
      ImportDirectory: string
      ErrorString: string
      Exn : exn option }

type EditGameType =
    | New
    | Selected

type InternalMsg =
    | AddBatchGames of string
    | EditImportDirectory of string
    | AddGame of ChessGame
    | StartGamePressed
    | DeleteGame of ChessGame
    | AddedGame of ChessGame
    | DeletedGame of ChessGame
    | SelectGame of ChessGame
    | UpdateGame of ChessGame option
    | UpdatedGame of ChessGame
    | CancelUpdate
    | EditPlayerName of (EditGameType * string * int)
    | EditEloWhite of (EditGameType * string)
    | EditEloBlack of (EditGameType * string)
    | EditYear of (EditGameType * string)
    | EditEvent of (EditGameType * string)
    | EditResult of (EditGameType * ChessGameResult)
    | EditGameNotation of (EditGameType * string)
    | EditHasRecorded of (EditGameType * bool)
    | HandleExn of exn
    | GotChessPlayers of ChessPlayer list
    | GotChessGames of ChessGame list
    | FilterChessGames
    | UpdateErrorString of string
    | UpdateDisplayedChessGames

type ExternalMsg =
    | UpdatedGames of ChessGame list
    | StartGame of ChessGame

type Msg =
    | Internal of InternalMsg
    | External of ExternalMsg

