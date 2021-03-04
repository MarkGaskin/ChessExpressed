module Client.App.ChessPlayers.View


open Shared
open Types
open Fable.React
open Fable.React.Props
open Fable.Core
open Fable.Core.JsInterop
open Fulma


let addPlayerView (model : Model) (dispatch : Msg -> unit) =
    Box.box' [ ] [
        Content.content [ ] [
            Content.Ol.ol [ ] [
                for chessPlayer in model.ChessPlayers do
                    li [ ] [ str chessPlayer.LastName ]
            ]
        ]
        Field.div [ Field.IsGrouped ] [
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.ChessPlayerInput.FirstName
                  Input.Placeholder "First Name"
                  Input.OnChange (fun x -> EditFirstName x.Value |> dispatch) ]
            ]
            Control.p [ ] [
                Button.a [
                    Button.Color IsPrimary
                    Button.OnClick (fun _ -> dispatch (AddPlayer model.ChessPlayerInput))
                ] [
                    str "Add"
                ]
            ]
        ]
    ]