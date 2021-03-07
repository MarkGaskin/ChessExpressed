namespace Shared.CEError


type ServerError =
    | Generic
    | FailedToAddPlayerToDatabase
    | FailedToUpdateEcosInDB
    | FailedToUpdateEcos
    | FailedToCreateEcosForUpsert
    | FailedToMatchEcoId
    | FailedToMatchEcoMoves
    | FailedToImportGames of exn
    | FailedToParseGame
    | FileForParsingNotFound

    static member describe =
        function
        | Generic -> "Generic Error"
        | FailedToAddPlayerToDatabase -> "Failed to add player to database"
        | FailedToUpdateEcosInDB -> "Failed to update the ECOs in the database"
        | FailedToUpdateEcos -> "Server failed to update the ECOs"
        | FailedToCreateEcosForUpsert -> "Server failed to create ECOs for upsert"
        | FailedToMatchEcoId -> "Failed to find an ECO with the provided id in the db"
        | FailedToMatchEcoMoves -> "Failed to match an ECO with the provided moves in the db"
        | FailedToImportGames _ -> "Failed to import games"
        | FailedToParseGame -> "Failed to parse game"
        | FileForParsingNotFound -> "Failed to find file for parsing"

type CEError =
    | Generic
    | Server of ServerError

    static member describe =
        function
        | CEError.Generic -> "Generic CEError"
        | Server appError -> "Server Error: " + (appError |> ServerError.describe)

