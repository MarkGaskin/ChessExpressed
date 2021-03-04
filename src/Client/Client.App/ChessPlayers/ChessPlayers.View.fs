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

let containerFieldProps : IHTMLProp list =
    [ Style [CSSProp.Padding 40; CSSProp.Margin "auto"; CSSProp.Display DisplayOptions.Flex] ]

let addPlayerView (model : Model) (dispatch : Msg -> unit) =
    div containerFieldProps [
        Box.box' [GenericOption.Props [Style [CSSProp.Width 400; CSSProp.MarginRight 150] ] ] [
            Field.div [ Field.IsGrouped; Field.IsGroupedCentered; Field.Props [HTMLAttr.Width "1000px"]; Field.Modifiers [] ] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value model.ChessPlayerInput.FirstName
                      Input.Placeholder "First Name"
                      Input.OnChange (fun x -> EditFirstName (EditPlayerType.New, x.Value) |> Internal |> dispatch) ]
                ] ]
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value model.ChessPlayerInput.LastName
                      Input.Placeholder "Last Name"
                      Input.OnChange (fun x -> EditLastName (EditPlayerType.New, x.Value) |> Internal |> dispatch) ]
            ] ]
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value model.ChessPlayerInput.NickName
                      Input.Placeholder "Nickname"
                      Input.OnChange (fun x -> EditNickName (EditPlayerType.New, x.Value) |> Internal |> dispatch) ]
            ] ]
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value model.ChessPlayerInput.TwitchChannel
                      Input.Placeholder "Twitch Channel"
                      Input.OnChange (fun x -> EditTwitchChannel (EditPlayerType.New, x.Value) |> Internal |> dispatch) ]
            ] ]
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value model.ChessPlayerInput.YouTubeChannel
                      Input.Placeholder "YouTube Channel"
                      Input.OnChange (fun x -> EditYouTubeChannel (EditPlayerType.New, x.Value) |> Internal |> dispatch) ]
            ] ]
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value model.ChessPlayerInput.TwitterHandle
                      Input.Placeholder "Twitter Handle"
                      Input.OnChange (fun x -> EditTwitterHandle (EditPlayerType.New, x.Value) |> Internal |> dispatch) ]
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
                                     Control.p [ ] [
                                         Button.a [
                                             Button.Color IColor.IsWhite
                                             Button.OnClick (fun _ -> SelectPlayer chessPlayer |> Internal |> dispatch )
                                         ] [ str (chessPlayer.FirstName + " " + chessPlayer.LastName) ] ]
                                     Control.p [ ] [
                                         Button.a [
                                             Button.Color IsPrimary
                                             Button.OnClick (fun _ -> DeletePlayer chessPlayer |> Internal |> dispatch )
                                         ] [ str "Delete" ] ] ] ]
                ]
            ]
        ]
        Box.box' [ GenericOption.Props [Style [CSSProp.Width 400] ] ] [
            Field.div [ Field.IsGrouped; Field.IsGroupedCentered; Field.Props []; Field.Modifiers [] ] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value (Option.map (fun player -> player.FirstName) model.SelectedChessPlayer |> Option.defaultValue "")
                      Input.Placeholder "First Name"
                      Input.OnChange (fun x -> EditFirstName (EditPlayerType.Selected, x.Value) |> Internal |> dispatch) ]
                ] ]
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value (Option.map (fun player -> player.LastName) model.SelectedChessPlayer |> Option.defaultValue "")
                      Input.Placeholder "Last Name"
                      Input.OnChange (fun x -> EditLastName (EditPlayerType.Selected, x.Value) |> Internal |> dispatch) ]
            ] ]
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value (Option.map (fun player -> player.NickName) model.SelectedChessPlayer |> Option.defaultValue "")
                      Input.Placeholder "Nickname"
                      Input.OnChange (fun x -> EditNickName (EditPlayerType.Selected, x.Value) |> Internal |> dispatch) ]
            ] ]
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value (Option.map (fun player -> player.TwitchChannel) model.SelectedChessPlayer |> Option.defaultValue "")
                      Input.Placeholder "Twitch Channel"
                      Input.OnChange (fun x -> EditTwitchChannel (EditPlayerType.Selected, x.Value) |> Internal |> dispatch) ]
            ] ]
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value (Option.map (fun player -> player.YouTubeChannel) model.SelectedChessPlayer |> Option.defaultValue "")
                      Input.Placeholder "YouTube Channel"
                      Input.OnChange (fun x -> EditYouTubeChannel (EditPlayerType.Selected, x.Value) |> Internal |> dispatch) ]
            ] ]
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value (Option.map (fun player -> player.TwitterHandle) model.SelectedChessPlayer |> Option.defaultValue "")
                      Input.Placeholder "Twitter Handle"
                      Input.OnChange (fun x -> EditTwitterHandle (EditPlayerType.Selected, x.Value) |> Internal |> dispatch) ]
            ] ]
            Field.div [] [
                Control.p [ ] [
                    Button.a [
                        Button.Color IsPrimary
                        Button.OnClick (fun _ -> dispatch (UpdatePlayer model.SelectedChessPlayer |> Internal))
                    ] [ str "Update" ] ] ]
            ]
        ]