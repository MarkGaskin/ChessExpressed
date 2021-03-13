module Client.App.View

open Shared
open Types
open Fable.React
open Fable.React.Props
open Fable.Core
open Fable.Core.JsInterop
open Fulma
open Browser.Blob
open ChessBoard.View
open ChessPlayers.View
open ChessGames.View
open Elmish

let view (model : Model) (dispatch : Msg -> unit) =
        Hero.hero [
            Hero.Color IsPrimary
            Hero.IsFullHeight
            Hero.Props [
                Style [
                    Background """linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), url("https://unsplash.it/1200/900?random") no-repeat center center fixed"""
                    BackgroundSize "cover"
                ]
            ]
        ] [ Hero.head []
                [ Tabs.tabs [ Tabs.IsBoxed
                              Tabs.IsCentered
                              Tabs.IsToggle ]
                      [ Tabs.tab [ Tabs.Tab.IsActive (model.ActiveTab = TabsType.AddPlayer);
                                   Tabs.Tab.Props [ OnClick ( fun _ -> SetTab TabsType.AddPlayer |> dispatch) ] ]
                          [ a [ ]
                              [ str "Add Player" ] ]
                        Tabs.tab [ Tabs.Tab.IsActive (model.ActiveTab = TabsType.AddGame);
                                   Tabs.Tab.Props [ OnClick ( fun _ -> SetTab TabsType.AddGame |> dispatch) ] ]
                          [ a [ ]
                              [ str "Add Game"] ]
                        Tabs.tab [ Tabs.Tab.IsActive (model.ActiveTab = TabsType.RecordGame);
                                   Tabs.Tab.Props [ OnClick ( fun _ -> SetTab TabsType.RecordGame |> dispatch) ]  ]
                          [ a [ ]
                              [ str "Record Game" ] ] ] ]
            Hero.body [ ] [ match model.ActiveTab with
                            | RecordGame -> recordGameView model.ChessBoardModel (ChessBoardMsg >> dispatch)
                            | AddGame -> addGameView model.ChessGamesModel (ChessGamesMsg >> dispatch)
                            | AddPlayer -> addPlayerView model.ChessPlayersModel (ChessPlayersMsg >> dispatch)
                          ]

        
                   
        ]