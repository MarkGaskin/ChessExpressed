module Client.App.ChessBoard.PrepareGameView

open Shared
open Fable.Remoting.Client
open Fable.React
open Fable.React.Props
open Fulma
open Fable.Core.JsInterop
open Fable.React.HookBindings
open Browser
open Browser.Types
open Browser.Blob
open Fable.Core
open PrepareGameTypes
open ChessPieces
open SquareStyles

type ChessBoardProps =
    | Position of obj
    | TransitionDuration of int
    | Width of int
    | DarkSquareStyle of obj
    | LightSquareStyle of obj
    | SquareStyles of obj
    | ShowNotation of bool
    | OnDrop of (unit -> unit)


let inline chessBoard (props : ChessBoardProps list) (elems : ReactElement list) : ReactElement =
    ofImport "default" "chessboardjsx" (keyValueList CaseRules.LowerFirst props) elems


let darkSquareStyle = createObj ["backgroundColor" ==> "rgba(220, 220, 220)"]
let lightSquareStyle = createObj ["backgroundColor" ==> "rgb(255, 255, 255)"]


let chessBoardView (model : Model) =
    Container.container [ ] [
        Column.column [
            Column.Width (Screen.All, Column.Is6)
            Column.Offset (Screen.All, Column.Is3) ]
            [
            chessBoard [ ChessBoardProps.Position (Array.item model.GameIndex model.FENArray) //model.AllPieces |> createPositionObject //
                         ChessBoardProps.Width 700;
                         ChessBoardProps.TransitionDuration transitionDuration;
                         ChessBoardProps.DarkSquareStyle darkSquareStyle;
                         ChessBoardProps.LightSquareStyle lightSquareStyle;
                         ChessBoardProps.SquareStyles (Array.item model.GameIndex model.SquareStyles);
                         ChessBoardProps.ShowNotation false
                         ChessBoardProps.OnDrop (fun _ -> JS.console.error("I'm alive"))] []
            ]
    ]

let containerFieldProps : IHTMLProp list =
    [ Style [CSSProp.Padding 40; CSSProp.Width "100%"; CSSProp.Height "100%"; CSSProp.Margin "auto"; CSSProp.Display DisplayOptions.Flex; CSSProp.Border "5px solid blue" ] ]


let prepareGameView (model : Model) (dispatch : Msg -> unit) =
    div containerFieldProps [
        Box.box' [GenericOption.Props [Style [CSSProp.Width 400;
                                              CSSProp.FontWeight "Bold";
                                              CSSProp.FontFamily "Verdana"
                                              CSSProp.FontSize 30;
                                              CSSProp.MarginRight -200;
                                              CSSProp.TextAlign TextAlignOptions.Center;
                                              CSSProp.MarginTop -10;
                                              CSSProp.MarginBottom -10;
                                              CSSProp.BackgroundColor "Transparent"
                                              CSSProp.Display DisplayOptions.Flex
                                              CSSProp.FlexDirection "column"
                                              CSSProp.Color "White"] ] ] [
            Field.div [] [
                Control.p [ ] [
                    Button.a [
                        Button.Color IsPrimary
                        Button.OnClick (fun _ -> dispatch (MoveForward |> Internal))
                    ] [ str "Forward" ] ] ]
            Field.div [] [
                Control.p [ ] [
                    Button.a [
                        Button.Color IsPrimary
                        Button.OnClick (fun _ -> dispatch (MoveBackward |> Internal))
                    ] [ str "Backward" ] ] ]

            div [Style [CSSProp.Height 400]] [
                div [ Style [CSSProp.MarginTop "auto"; CSSProp.MarginBottom -20 ] ] [str (ChessPlayer.getPlayerName model.WhitePlayer)]
            ]
        ]
        chessBoardView model
    ]
