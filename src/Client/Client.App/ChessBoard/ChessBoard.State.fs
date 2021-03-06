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

let init api =
    JS.console.warn "Updating ecos"
    { Api = api
      FENPosition = ""
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

    | StartGame _ -> model, Cmd.none

    
    | UpdateErrorString string ->
        JS.console.error string
        { model with ErrorString = string }, Cmd.none

    | HandleExn exn ->
        { model with Exn = Some exn }, exn.Message |> UpdateErrorString |> Cmd.ofMsg