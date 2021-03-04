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
      ChessPlayerInput: ChessPlayer
      ErrorString: string }

type Msg =
    | AddPlayer of ChessPlayer
    | AddedPlayer of ChessPlayer
    | EditFirstName of string
    | EditLastName of string
    | EditNickName of string
    | EditTwitchChannel of string
    | EditYouTubeChannel of string
    | EditTwitterHandle of string
    | UpdateErrorString of string

