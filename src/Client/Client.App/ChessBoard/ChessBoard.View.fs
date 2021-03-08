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

type ChessBoardProps =
    | Position of obj
    | TransitionDuration of int
    | Width of int
    | DarkSquareStyle of obj
    | LightSquareStyle of obj
    | SquareStyles of obj
    | ShowNotation of bool

type RenderProps =
    | Status
    | StartRecording of (unit -> unit)
    | StopRecording of (unit -> unit)
    | MediaBlobUrl of Blob

let inline chessBoard (props : ChessBoardProps list) (elems : ReactElement list) : ReactElement =
    ofImport "default" "chessboardjsx" (keyValueList CaseRules.LowerFirst props) elems

type useMediaRecorderType = Blob -> (string * (unit -> unit) * (unit -> unit) * (unit -> Blob))

let useMediaRecorder : useMediaRecorderType = import "useReactMediaRecorder" "react-media-recorder"


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
            button [OnClick (fun _ -> () )] [];
            chessBoard [ ChessBoardProps.Position (model.AllPieces |> createPositionObject)
                         ChessBoardProps.Width 700;
                         ChessBoardProps.TransitionDuration transitionDuration;
                         ChessBoardProps.DarkSquareStyle darkSquareStyle;
                         ChessBoardProps.LightSquareStyle lightSquareStyle;
                         ChessBoardProps.SquareStyles (model.SquareStyles);
                         ChessBoardProps.ShowNotation false ] []
            ]
    ]


type MediaRecorderProps =
    | Audio of bool
    | Video of bool
    | Render of useMediaRecorderType


let inline reactMediaRecorder (props : MediaRecorderProps list) (elems : ReactElement list) : ReactElement =
    ofImport "ReactMediaRecorder" "react-media-recorder" (keyValueList CaseRules.LowerFirst props) elems

let recordGameView (model : Model) (dispatch : Msg -> unit) =
    //canvas [] [
    //FunctionComponent.Of (fun props -> 
    //    let (status, startRec, stopRec, mediaBlob ) = useMediaRecorder(Blob.Create props)
    //reactMediaRecorder [MediaRecorderProps.Audio true; MediaRecorderProps.Video true; MediaRecorderProps.Render ] [
        chessBoardView model
    //]


            //) [|createObj ["Video" ==> "true"; "Audio" ==> "true" ]|]