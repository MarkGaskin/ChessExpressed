module Client.App.View

open Shared
open Types
open Fable.React
open Fable.React.Props
open Fable.Core
open Fable.Core.JsInterop
open Fulma

let inline chessBoard (props : ChessBoardProps list) (elems : ReactElement list) : ReactElement =
    ofImport "default" "chessboardjsx" (keyValueList CaseRules.LowerFirst props) elems

let navBrand =
    Navbar.Brand.div [ ] [
        Navbar.Item.a [
            Navbar.Item.Props [ Href "https://safe-stack.github.io/" ]
            Navbar.Item.IsActive true
        ] [
            img [
                Src "/favicon.png"
                Alt "Logo"
            ]
        ]
    ]

let containerBox (model : Model) (dispatch : Msg -> unit) =
    Box.box' [ ] [
        Content.content [ ] [
            Content.Ol.ol [ ] [
                for todo in model.ChessGames do
                    li [ ] [ str todo.Event ]
            ]
        ]
        Field.div [ Field.IsGrouped ] [
            Control.p [ Control.IsExpanded ] [
                Input.text [
                  Input.Value model.Input
                  Input.Placeholder "What needs to be done?"
                  Input.OnChange (fun x -> SetInput x.Value |> dispatch) ]
            ]
            Control.p [ ] [
                Button.a [
                    Button.Color IsPrimary
                    Button.Disabled (ChessGame.isValid model.Input |> not)
                    Button.OnClick (fun _ -> dispatch AddChessGame)
                ] [
                    str "Add"
                ]
            ]
        ]
    ]

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
    ] [
        Hero.head [ ] [
            Navbar.navbar [ ] [
                Container.container [ ] [ navBrand ]
            ]
        ]

        Hero.body [ ] [
            Container.container [ ] [
                Column.column [
                    Column.Width (Screen.All, Column.Is6)
                    Column.Offset (Screen.All, Column.Is3)
                ] [
                    chessBoard [ ChessBoardProps.Position "start";
                                 ChessBoardProps.Width 560;
                                 ChessBoardProps.TransitionDuration 500;
                                 ChessBoardProps.DarkSquareStyle darkSquareStyle
                                 ChessBoardProps.LightSquareStyle lightSquareStyle
                                 ChessBoardProps.SquareStyles squareStyle ] []
                ]
            ]

        ]
    ]