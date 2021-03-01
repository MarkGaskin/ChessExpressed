module Client.App.State

open Types
open Elmish

let init(): Model * Cmd<Msg> =
    let model =
        { ChessGames = []
          Input = "" }
    let cmd = Cmd.OfAsync.perform todosApi.getChessGames () GotChessGames
    model, cmd

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with
    | GotChessGames todos ->
        { model with ChessGames = todos }, Cmd.none
    | SetInput value ->
        { model with Input = value }, Cmd.none
    | AddChessGame ->
        //let todo = ChessGame.create model.Input
        //let cmd = Cmd.OfAsync.perform todosApi.addChessGame todo AddedTodo
        { model with Input = "" }, Cmd.none // cmd
    | AddedChessGame todo ->
        { model with ChessGames = model.ChessGames @ [ todo ] }, Cmd.none