module Client.App.ChessGames.Types

open Shared
open Shared.CEError
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
      ImportSingleDirectory: string
      ErrorString: string
      Exn : exn option }

type EditGameType =
    | New
    | Selected

type InternalMsg =
    | AddBatchGames of string
    | AddedBatchGames of Result<unit, ServerError>
    | ImportGame of string
    | ImportedGame of Result<unit, ServerError>
    | EditImportDirectory of string
    | EditImportSingleDirectory of string
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

type ExternalMsg =
    | ImportedGames
    | StartGame of (ChessGame option * ChessPlayer option * ChessPlayer option)

type Msg =
    | Internal of InternalMsg
    | External of ExternalMsg

