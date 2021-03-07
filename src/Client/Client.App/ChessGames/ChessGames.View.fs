module Client.App.ChessGames.View


open Shared
open System
open Types
open Fable.React
open Fable.React.Props
open Fable.Core
open Fable.Core.JsInterop
open Fulma
open System.Globalization

let (+/) string1 string2 =
    string1 + ", " + string2

let containerFieldProps : IHTMLProp list =
    [ Style [CSSProp.Padding 40; CSSProp.Margin "auto"; CSSProp.Display DisplayOptions.Flex] ]

let addGameView (model : Model) (dispatch : Msg -> unit) =
    div containerFieldProps [
        div [] [
            Box.box' [GenericOption.Props [Style [CSSProp.Width 400; CSSProp.Height 850; CSSProp.MarginRight 100] ] ] [
                Field.div [] [
                    Dropdown.dropdown[ Dropdown.IsHoverable ] [
                        Dropdown.trigger [] [
                            Input.text [
                                Input.Value (model.InputPlayer1Name)
                                Input.Placeholder "Player 1 Name"
                                Input.OnChange (fun x -> EditPlayerName (EditGameType.New, x.Value, 0) |> Internal |> dispatch) 
                            ]
                        ]
                        Dropdown.menu [] [
                            Dropdown.content [] [
                                for chessPlayer in model.ChessPlayers do
                                    if ChessPlayer.searchNameStart chessPlayer model.InputPlayer1Name then
                                        Dropdown.Item.div [
                                            Dropdown.Item.Option.Props [
                                                OnClick (fun _ -> EditPlayerName (EditGameType.New, ChessPlayer.getPlayerName chessPlayer,0) |> Internal |> dispatch )
                                            ]
                                        ] [str (ChessPlayer.getPlayerName chessPlayer)]
                            ]
                        ]
                    ]
                ]

                Field.div [] [
                    Dropdown.dropdown[ Dropdown.IsHoverable ] [
                        Dropdown.trigger [] [
                            Input.text [
                                Input.Value (model.InputPlayer2Name)
                                Input.Placeholder "Player 2 Name"
                                Input.OnChange (fun x -> EditPlayerName (EditGameType.New, x.Value, 1) |> Internal |> dispatch) 
                            ]
                        ]
                        Dropdown.menu [] [
                            Dropdown.content [] [
                                for chessPlayer in model.ChessPlayers do
                                    if ChessPlayer.searchNameStart chessPlayer model.InputPlayer2Name then
                                        Dropdown.Item.div [
                                            Dropdown.Item.Option.Props [
                                                OnClick (fun _ -> EditPlayerName (EditGameType.New, ChessPlayer.getPlayerName chessPlayer,1) |> Internal |> dispatch )
                                            ]
                                        ] [str (ChessPlayer.getPlayerName chessPlayer)]
                            ]
                        ]
                    ]
                ]

                Field.div [] [
                    Control.p [ Control.IsExpanded ] [
                        Input.number [
                          Input.Value (model.ChessGameInput.EloWhite |> Option.defaultValue "")
                          Input.Placeholder "ELO White"
                          Input.OnChange (fun x -> EditEloWhite (EditGameType.New, x.Value) |> Internal |> dispatch) ]
                    ]
                ]

                Field.div [] [
                    Control.p [ Control.IsExpanded ] [
                        Input.number [
                          Input.Value (model.ChessGameInput.EloBlack|> Option.defaultValue "")
                          Input.Placeholder "ELO Black"
                          Input.OnChange (fun x -> EditEloBlack (EditGameType.New, x.Value) |> Internal |> dispatch) ]
                    ]
                ]

                Field.div [] [
                    Control.p [ Control.IsExpanded ] [
                        Input.text [
                          Input.Value (model.ChessGameInput.Year|> Option.defaultValue "")
                          Input.Placeholder "Year"
                          Input.OnChange (fun x -> EditYear (EditGameType.New, x.Value) |> Internal |> dispatch) ]
                    ]
                ]
            
                Field.div [] [
                    Control.p [ Control.IsExpanded ] [
                        Input.text [
                          Input.Value (model.ChessGameInput.Event|> Option.defaultValue "")
                          Input.Placeholder "Event"
                          Input.OnChange (fun x -> EditEvent (EditGameType.New, x.Value) |> Internal |> dispatch) ]
                    ]
                ]

            
            
                Field.div [] [
                    Dropdown.dropdown[ Dropdown.IsHoverable ] [
                        Dropdown.trigger [] [
                            Input.text [
                                Input.Value (model.ChessGameInput.Result  |> ChessGameResult.describe)
                                Input.Placeholder "Result"
                                Input.OnChange (fun x -> EditResult (EditGameType.New, x.Value |> ChessGameResult.parse) |> Internal |> dispatch) 
                            ]
                        ]
                        Dropdown.menu [] [
                            Dropdown.content [] [
                                for result in ChessGameResult.all do
                                    Dropdown.Item.div [
                                        Dropdown.Item.Option.Props [
                                            OnClick (fun _ -> EditResult (EditGameType.New, result) |> Internal |> dispatch )
                                        ]
                                    ] [str (result |> ChessGameResult.describe)]
                            ]
                        ]
                    ]
                ]
       
                Field.div [] [
                    Control.p [ Control.IsExpanded ] [
                        Input.text [
                            Input.Props [Style [CSSProp.Height 400; CSSProp.MarginRight 150]] 
                            Input.Value (model.ChessGameInput.GameNotation)
                            Input.Placeholder "Game Notation"
                            Input.OnChange (fun x -> EditGameNotation (EditGameType.New, x.Value) |> Internal |> dispatch) ]
                    ]
                ]
            
                Field.div [ Field.IsHorizontal ] [
                    Control.p [ Control.Props [Style [ CSSProp.MarginRight 200 ] ] ] [
                        Button.a [
                            Button.Color IsPrimary
                            Button.OnClick (fun _ -> dispatch (AddGame model.ChessGameInput |> Internal))
                        ] [ str "Add" ]
                    ]     
                ]
        
                Box.box' [GenericOption.Props [Style [CSSProp.Width 400; CSSProp.MarginTop 100] ] ] [
                    Field.div [ Field.IsHorizontal; Field.Props [Style [ ] ] ] [
                        Control.p [ Control.IsExpanded ] [
                            Input.text [
                                Input.Value (model.ImportDirectory)
                                Input.Placeholder "Import Directory"
                                Input.OnChange (fun x -> EditImportDirectory x.Value |> Internal |> dispatch)
                            ]
                        ]
                        Control.p [ Control.Props [Style [ CSSProp.MarginLeft 40 ] ]  ] [
                            Button.a [
                                Button.Color IsPrimary
                                Button.OnClick (fun _ -> dispatch (AddBatchGames model.ImportDirectory |> Internal))
                            ] [ str "Import" ]
                        ]
                    ]
                ]
            ]
        ]

        
        Box.box' [GenericOption.Props [Style [CSSProp.Width 700; CSSProp.MarginRight 100 ] ] ] [
            div [] [
                Content.content [] [
                    Content.Ol.ol [ ] [
                        for chessGame in model.DisplayedChessGames do
                            li [ ] [ Field.div [ Field.IsHorizontal; Field.IsGroupedRight ] [
                                Control.p [ ] [
                                    Button.a [
                                        Button.Color IColor.IsWhite
                                        Button.OnClick (fun _ -> SelectGame chessGame |> Internal |> dispatch )
                                    ] [ str ( chessGame.DisplayName ) ]
                                ]
                                Control.p [ ] [
                                    Button.a [
                                        Button.Color IsPrimary
                                        Button.OnClick (fun _ -> DeleteGame chessGame |> Internal |> dispatch )
                                    ] [ str "Delete" ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
        Box.box' [ GenericOption.Props [Style [CSSProp.Width 400] ] ] [
           
            Field.div [] [
                Dropdown.dropdown[ Dropdown.IsHoverable ] [
                    Dropdown.trigger [] [
                        Input.text [
                            Input.Value (model.SelectedPlayer1Name)
                            Input.Placeholder "Player 1 Name"
                            Input.OnChange (fun x -> EditPlayerName (EditGameType.Selected, x.Value, 0) |> Internal |> dispatch) 
                        ]
                    ]
                    Dropdown.menu [] [
                        Dropdown.content [] [
                            for chessPlayer in model.ChessPlayers do
                                if ChessPlayer.searchNameStart chessPlayer model.SelectedPlayer2Name then
                                    Dropdown.Item.div [
                                        Dropdown.Item.Option.Props [
                                            OnClick (fun _ -> EditPlayerName (EditGameType.Selected, ChessPlayer.getPlayerName chessPlayer,0) |> Internal |> dispatch )
                                        ]
                                    ] [str (ChessPlayer.getPlayerName chessPlayer)]
                        ]
                    ]
                ]
            ]

            Field.div [] [
                Dropdown.dropdown[ Dropdown.IsHoverable ] [
                    Dropdown.trigger [] [
                        Input.text [
                            Input.Value (model.SelectedPlayer2Name)
                            Input.Placeholder "Player 2 Name"
                            Input.OnChange (fun x -> EditPlayerName (EditGameType.Selected, x.Value, 1) |> Internal |> dispatch) 
                        ]
                    ]
                    Dropdown.menu [] [
                        Dropdown.content [] [
                            for chessPlayer in model.ChessPlayers do
                                if ChessPlayer.searchNameStart chessPlayer model.SelectedPlayer2Name then
                                    Dropdown.Item.div [
                                        Dropdown.Item.Option.Props [
                                            OnClick (fun _ -> EditPlayerName (EditGameType.Selected, ChessPlayer.getPlayerName chessPlayer,1) |> Internal |> dispatch )
                                        ]
                                    ] [str (ChessPlayer.getPlayerName chessPlayer)]
                        ]
                    ]
                ]
            ]

            
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.number [
                      Input.Value (Option.map (fun (chessGame:ChessGame) -> chessGame.EloWhite |> string) model.SelectedChessGame |> Option.defaultValue "")
                      Input.Placeholder "ELO White"
                      Input.OnChange (fun x -> EditEloWhite (EditGameType.Selected, x.Value) |> Internal |> dispatch) ]
                ]
            ]

            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.number [
                      Input.Value (Option.map (fun (chessGame:ChessGame) -> chessGame.EloBlack |> string) model.SelectedChessGame |> Option.defaultValue "")
                      Input.Placeholder "ELO Black"
                      Input.OnChange (fun x -> EditEloBlack (EditGameType.Selected, x.Value) |> Internal |> dispatch) ]
                ]
            ]

            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value (Option.bind (fun (chessGame:ChessGame) -> chessGame.Year) model.SelectedChessGame |> Option.defaultValue "")
                      Input.Placeholder "Year"
                      Input.OnChange (fun x -> EditYear (EditGameType.Selected, x.Value) |> Internal |> dispatch) ]
                ]
            ]
            
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value (Option.bind (fun (chessGame:ChessGame) -> chessGame.Event) model.SelectedChessGame |> Option.defaultValue "")
                      Input.Placeholder "Event"
                      Input.OnChange (fun x -> EditEvent (EditGameType.Selected, x.Value) |> Internal |> dispatch) ]
                ]
            ]

            
            
            Field.div [] [
                Dropdown.dropdown[ Dropdown.IsHoverable ] [
                    Dropdown.trigger [] [
                        Input.text [
                            Input.Value (Option.map (fun (chessGame:ChessGame) -> chessGame.Result |> ChessGameResult.describe) model.SelectedChessGame |> Option.defaultValue "")
                            Input.Placeholder "Result"
                            Input.OnChange (fun x -> EditResult (EditGameType.Selected, x.Value |> ChessGameResult.parse) |> Internal |> dispatch) 
                        ]
                    ]
                    Dropdown.menu [] [
                        Dropdown.content [] [
                            for result in ChessGameResult.all do
                                Dropdown.Item.div [
                                    Dropdown.Item.Option.Props [
                                        OnClick (fun _ -> EditResult (EditGameType.Selected, result) |> Internal |> dispatch )
                                    ]
                                ] [str (result |> ChessGameResult.describe)]
                        ]
                    ]
                ]
            ]
       
            Field.div [] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                        Input.Props [Style [CSSProp.Height 400; CSSProp.MarginRight 150]] 
                        Input.Value (Option.map (fun (chessGame:ChessGame) -> chessGame.GameNotation) model.SelectedChessGame |> Option.defaultValue "")
                        Input.Placeholder "Game Notation"
                        Input.OnChange (fun x -> EditEvent (EditGameType.Selected, x.Value) |> Internal |> dispatch) ]
                ]
            ]
            
            Field.div [ Field.IsHorizontal ] [
                Control.p [ Control.Props [Style [ CSSProp.MarginRight 172 ] ] ] [
                    Button.a [
                        Button.Color IsPrimary
                        Button.OnClick (fun _ -> dispatch (CancelUpdate |> Internal))
                    ] [ str "Cancel" ]
                ]
                Control.p [ Control.Props [Style [ CSSProp.MarginRight 128 ] ] ] [
                    Button.a [
                        Button.Color IsPrimary
                        Button.OnClick (fun _ -> dispatch (UpdateGame model.SelectedChessGame |> Internal))
                    ] [ str "Update" ]
                ]
            ]

            Field.div [ Field.IsGroupedCentered; Field.Props [Style [ CSSProp.MarginTop 48 ] ] ] [
                Control.p [ ] [
                    Button.a [
                        Button.Props [Style [ CSSProp.Height 80 ]]
                        Button.Color IsPrimary
                        Button.Disabled (model.SelectedChessGame <> model.SelectedChessGameOriginal || model.SelectedChessGame.IsNone)
                        Button.OnClick (fun _ -> dispatch (StartGame model.SelectedChessGame.Value |> External))
                    ] [ str "Start Recording" ]
                ]
            ]
        ]
    ]

