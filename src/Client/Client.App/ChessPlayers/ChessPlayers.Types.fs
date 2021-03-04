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
      SelectedChessPlayer: ChessPlayer option
      ChessPlayerInput: ChessPlayer
      ErrorString: string
      Exn : exn option }

type EditPlayerType =
    | New
    | Selected

type InternalMsg =
    | AddPlayer of ChessPlayer
    | DeletePlayer of ChessPlayer
    | AddedPlayer of ChessPlayer
    | DeletedPlayer of ChessPlayer
    | SelectPlayer of ChessPlayer
    | UpdatePlayer of ChessPlayer option
    | UpdatedPlayer of ChessPlayer
    | EditFirstName of (EditPlayerType * string)
    | EditLastName of (EditPlayerType * string)
    | EditNickName of (EditPlayerType * string)
    | EditTwitchChannel of (EditPlayerType * string)
    | EditYouTubeChannel of (EditPlayerType * string)
    | EditTwitterHandle of (EditPlayerType * string)
    | UpdateErrorString of string
    | HandleExn of exn
    | UpdateDisplayedChessPlayers
    | GotChessPlayers of ChessPlayer list

type ExternalMsg =
    | UpdatedPlayers of ChessPlayer list

type Msg =
    | Internal of InternalMsg
    | External of ExternalMsg

