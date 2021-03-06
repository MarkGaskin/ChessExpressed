module Client.App.Types

open Shared
open Fable.Remoting.Client
open Fable.React
open Fable.Core.JsInterop
open Browser
open Browser.Types
open Browser.Blob
open Fable.Core
open Fable.React.Props

//type IMediaRecord =
//    abstract startRecording : unit -> unit
//    abstract stopRecording : unit -> unit
//    abstract mediaBlobUrl : HTMLCanvasElement -> unit

//[<Import("default", from="react-media-recorder")>]
//let mediaRecorder : IMediaRecord = jsNative

type IMediaProps =
    | Audio of bool
    | Video of bool

type useMediaRecorderType = Blob -> (string * (unit -> unit) * (unit -> unit) * (unit -> Blob))

let useMediaRecorder : useMediaRecorderType = import "useReactMediaRecorder" "react-media-recorder"
   

type TabsType =
    | AddPlayer
    | AddGame
    | RecordGame

type Model =
    { Api: ICEApi
      ChessGames: ChessGame list
      ChessBoardModel: ChessBoard.Types.Model
      ChessPlayersModel: ChessPlayers.Types.Model
      ChessGamesModel: ChessGames.Types.Model
      ActiveTab: TabsType
      }

type Msg =
    | GotChessGames of ChessGame list
    | AddChessGame
    | AddedChessGame of ChessGame
    | StartRecording of (unit -> unit)
    | SetTab of TabsType
    | ChessBoardMsg of ChessBoard.Types.Msg
    | ChessPlayersMsg of ChessPlayers.Types.Msg
    | ChessGamesMsg of ChessGames.Types.Msg