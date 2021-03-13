module Client.App.ChessBoard.View

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
open Types
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

//type RenderProps =
//    | Status
//    | StartRecording of (MouseEvent -> unit)
//    | StopRecording of (MouseEvent -> unit)
//    | MediaBlobUrl of Blob

//type useMediaRecorderResult =
//    {
//        status : string
//        mediaBlob : Blob
//        stopRecording : unit -> unit
//        startRecording : unit -> unit
//    }

//type OnStopFunction = string -> unit 

//type MediaProps =
//    { audio : bool
//      video : bool
//      screen: bool
//      //blobPropertyBag: obj
//      onStop: OnStopFunction }

//let onStop urlBlob =
//    JS.console.log "onStop"
//    JS.console.log urlBlob

//type MyProps =
//    { model: Model }

//type useMediaRecorderType = MediaProps -> useMediaRecorderResult

//let useMediaRecorder : useMediaRecorderType = import "useReactMediaRecorder" "react-media-recorder"


let darkSquareStyle = createObj ["backgroundColor" ==> "rgba(220, 220, 220)"]
let lightSquareStyle = createObj ["backgroundColor" ==> "rgb(255, 255, 255)"]



//FunctionComponent.Of(fun props ->
//let (status, startRec, stopRec, mediaBlob ) = useMediaRecorder(Blob.Create props)


//) ()


let chessBoardView (model : Model) =
    Container.container [ ] [
        Column.column [
            Column.Width (Screen.All, Column.Is6)
            Column.Offset (Screen.All, Column.Is3) ]
            [
            chessBoard [ ChessBoardProps.Position model.FENPosition //model.AllPieces |> createPositionObject //
                         ChessBoardProps.Width 700;
                         ChessBoardProps.TransitionDuration transitionDuration;
                         ChessBoardProps.DarkSquareStyle darkSquareStyle;
                         ChessBoardProps.LightSquareStyle lightSquareStyle;
                         ChessBoardProps.SquareStyles (model.SquareStyles);
                         ChessBoardProps.ShowNotation false
                         ChessBoardProps.OnDrop (fun _ -> JS.console.error("I'm alive"))] []
            ]
    ]


//type MediaRecorderProps =
//    | Audio of bool
//    | Video of bool
//    | Render of (RenderProps[] -> ReactElement)


//let inline reactMediaRecorder (props : MediaRecorderProps list) (elems : ReactElement list) : ReactElement =
//    ofImport "default" "react-media-recorder" (keyValueList CaseRules.LowerFirst props) elems

let containerFieldProps : IHTMLProp list =
    [ Style [CSSProp.Padding 40; CSSProp.Width "100%"; CSSProp.Height "100%"

    ; CSSProp.Margin "auto"; CSSProp.Display DisplayOptions.Flex; CSSProp.Border "5px solid blue" ] ]




//let reactMediaRecorderHook =
//    FunctionComponent.Of (fun (props: MyProps) ->
//        let result = useMediaRecorder ({video = false; audio = true; screen = true; (*blobPropertyBag = createObj ["type"==>"video/mp4"];*) onStop = onStop })
//        div[] [
//            Button.a [
//                Button.Color IsPrimary
//                Button.OnClick (fun _ -> JS.console.log "Started recording"; result.startRecording ())
//            ] [ str "Record" ]
//            Button.a [
//                Button.Color IsPrimary
//                Button.OnClick (fun _ -> JS.console.log "Stopped recording"; result.stopRecording ())
//            ] [ str "Stop"]
            
//        ]
//    )

let recordGameView (model : Model) (dispatch : Msg -> unit) =
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
            div [Style [CSSProp.Height 400]] [
                div [] [str (ChessPlayer.getPlayerName model.BlackPlayer)]
                div [] [img [ Src model.BlackPlayerImage; HTMLAttr.Height 300; HTMLAttr.Width 300 ] ]
            ]
            div [Style [CSSProp.Height 400]] [
                div [] [img [ Src model.WhitePlayerImage; HTMLAttr.Height 250; HTMLAttr.Width 250; HTMLAttr.MarginHeight 100.0; ] ]
                div [ Style [CSSProp.MarginTop "auto"; CSSProp.MarginBottom -20 ] ] [str (ChessPlayer.getPlayerName model.WhitePlayer)]
            ]
        ]
        chessBoardView model
    ]
