namespace Shared.CEError


type ServerError =
    | Generic
    | FailedToAddPlayerToDatabase

    static member describe =
        function
        | Generic -> "Generic Error"
        | FailedToAddPlayerToDatabase -> "Failed to add player to database"

type CEError =
    | Generic
    | Server of ServerError

    static member describe =
        function
        | CEError.Generic -> "Generic CEError"
        | Server appError -> "Server Error: " + (appError |> ServerError.describe)

