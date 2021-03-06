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

let inline chessBoard (props : ChessBoardProps list) (elems : ReactElement list) : ReactElement =
    ofImport "default" "chessboardjsx" (keyValueList CaseRules.LowerFirst props) elems

//let inline reactMediaRecorder (props : MediaRecorderProps list) (elems : ReactElement list) : ReactElement =
//    ofImport "default" "react-media-recorder" (keyValueList CaseRules.LowerFirst props) elems

let recordGameView (model : Model) (dispatch : Msg -> unit) =
    Container.container [ ] [
        Column.column [
            Column.Width (Screen.All, Column.Is6)
            Column.Offset (Screen.All, Column.Is3)
        ] [
            button [OnClick (fun _ -> ())] [];
                    chessBoard [ ChessBoardProps.Position "start";
                                 ChessBoardProps.Width 560;
                                 ChessBoardProps.TransitionDuration 500;
                                 ChessBoardProps.DarkSquareStyle darkSquareStyle;
                                 ChessBoardProps.LightSquareStyle lightSquareStyle;
                                 ChessBoardProps.SquareStyles squareStyle;
                                 ChessBoardProps.ShowNotation false ] [] ]
        ]


let view (model : Model) (dispatch : Msg -> unit) =
    //FunctionComponent.Of(fun props ->
        //let (status, startRec, stopRec, mediaBlob ) = useMediaRecorder(Blob.Create props)
        Hero.hero [
            Hero.Color IsPrimary
            Hero.IsFullHeight
            Hero.Props [
                Style [
                    Background """linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), url("https://unsplash.it/1200/900?random") no-repeat center center fixed"""
                    BackgroundSize "cover"
                ]
            ]
        ] [ Hero.head [ ]
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
                            | RecordGame -> recordGameView model dispatch
                            | AddGame -> addGameView model.ChessGamesModel (ChessGamesMsg >> dispatch)
                            | AddPlayer -> addPlayerView model.ChessPlayersModel (ChessPlayersMsg >> dispatch)
                          ]

        
                   
        ]
        //) ([|createObj ["Video" ==> "true"; "Audio" ==> "true" ]|])