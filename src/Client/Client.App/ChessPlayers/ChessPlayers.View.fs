module Client.App.ChessPlayers.View


open Shared
open Types
open Fable.React
open Fable.React.Props
open Fable.Core
open Fable.Core.JsInterop
open Fulma

let (+/) string1 string2 =
    string1 + ", " + string2

let addPlayerView (model : Model) (dispatch : Msg -> unit) =
    Box.box' [ ] [
        Field.div [ Field.IsGrouped; Field.IsGroupedCentered ] [
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.ChessPlayerInput.FirstName
                  Input.Placeholder "First Name"
                  Input.OnChange (fun x -> EditFirstName x.Value |> Internal |> dispatch) ]
            ] ]
        Field.div [] [
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.ChessPlayerInput.LastName
                  Input.Placeholder "Last Name"
                  Input.OnChange (fun x -> EditLastName x.Value |> Internal |> dispatch) ]
        ] ]
        Field.div [] [
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.ChessPlayerInput.NickName
                  Input.Placeholder "Nickname"
                  Input.OnChange (fun x -> EditNickName x.Value |> Internal |> dispatch) ]
        ] ]
        Field.div [] [
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.ChessPlayerInput.TwitchChannel
                  Input.Placeholder "Twitch Channel"
                  Input.OnChange (fun x -> EditTwitchChannel x.Value |> Internal |> dispatch) ]
        ] ]
        Field.div [] [
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.ChessPlayerInput.YouTubeChannel
                  Input.Placeholder "YouTube Channel"
                  Input.OnChange (fun x -> EditYouTubeChannel x.Value |> Internal |> dispatch) ]
        ] ]
        Field.div [] [
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.ChessPlayerInput.TwitterHandle
                  Input.Placeholder "Twitter Handle"
                  Input.OnChange (fun x -> EditTwitterHandle x.Value |> Internal |> dispatch) ]
        ] ]
        Field.div [] [
            Control.p [ ] [
                Button.a [
                    Button.Color IsPrimary
                    Button.OnClick (fun _ -> dispatch (AddPlayer model.ChessPlayerInput |> Internal))
                ] [ str "Add" ] ] ]
        Content.content [] [
            Content.Ol.ol [ ] [
                for chessPlayer in model.DisplayedChessPlayers do
                    li [ ] [ Field.div [ Field.IsHorizontal; Field.IsGroupedRight ] [
                                 str (chessPlayer.FirstName +/ chessPlayer.LastName +/ chessPlayer.NickName +/ chessPlayer.TwitchChannel +/ chessPlayer.YouTubeChannel +/ chessPlayer.TwitterHandle )
                                 Control.p [ ] [
                                     Button.a [
                                         Button.Color IsPrimary
                                         Button.OnClick (fun _ -> DeletePlayer chessPlayer |> Internal |> dispatch )
                                     ] [ str "Delete" ] ] ] ]
            ]
        ]
    ]