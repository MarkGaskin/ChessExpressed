module Client.App.ChessPlayers.Types

open Shared

//type ChessPlayerVM =
//    { FirstName: string
//      LastName: string
//      NickName: string
//      TwitchChannel: string
//      YouTubeChannel: string
//      TwitterHandle: string }
//    static member defaultPlayer =
//        { FirstName = ""
//          LastName = ""
//          NickName = ""
//          TwitchChannel = ""
//          YouTubeChannel = ""
//          TwitterHandle = "" }


type Model =
    { Api: ICEApi
      ChessPlayers: ChessPlayer list
      DisplayedChessPlayers: ChessPlayer list
      ChessPlayerInput: ChessPlayer
      ErrorString: string
      Exn : exn option }

type InternalMsg =
    | AddPlayer of ChessPlayer
    | DeletePlayer of ChessPlayer
    | AddedPlayer of ChessPlayer
    | DeletedPlayer of ChessPlayer
    | EditFirstName of string
    | EditLastName of string
    | EditNickName of string
    | EditTwitchChannel of string
    | EditYouTubeChannel of string
    | EditTwitterHandle of string
    | UpdateErrorString of string
    | HandleExn of exn
    | UpdateDisplayedChessPlayers
    | GotChessPlayers of ChessPlayer list

type ExternalMsg =
    | UpdatedPlayers of ChessPlayer list

type Msg =
    | Internal of InternalMsg
    | External of ExternalMsg

