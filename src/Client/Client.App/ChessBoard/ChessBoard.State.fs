module Client.App.ChessBoard.State

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
      FENPosition = "" }, Cmd.none

let update msg (model:Model) =
    match msg with
    | StartGame _ -> model, Cmd.none