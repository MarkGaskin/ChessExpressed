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



type TabsType =
    | AddPlayer
    | AddGame
    | PrepareGame
    | RecordGame

type Model =
    { Api: ICEApi
      RecordModel: ChessBoard.RecordGameTypes.Model
      PrepareModel: ChessBoard.PrepareGameTypes.Model
      ChessPlayersModel: ChessPlayers.Types.Model
      ChessGamesModel: ChessGames.Types.Model
      ActiveTab: TabsType
      }

type Msg =
    | StartRecording of (unit -> unit)
    | SetTab of TabsType
    | RecordMsg of ChessBoard.RecordGameTypes.Msg
    | PrepareMsg of ChessBoard.PrepareGameTypes.Msg
    | ChessPlayersMsg of ChessPlayers.Types.Msg
    | ChessGamesMsg of ChessGames.Types.Msg