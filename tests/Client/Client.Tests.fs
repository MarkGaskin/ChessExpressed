module Client.Tests

open Fable.Mocha

open Client.App.State
open Client.App.Types
open Shared
open Fable.Remoting.Client


let api =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ICEApi>

let client = testList "Client" [
    testCase "Added todo" <| fun _ ->
        let newTodo = ChessGame.defaultGame
        let model, _ = init api ()

        let model, _ = update (AddedChessGame newTodo) model

        Expect.equal 1 model.ChessGames.Length "There should be 1 todo"
        Expect.equal newTodo model.ChessGames.[0] "Todo should equal new todo"
]

let all =
    testList "All"
        [
#if FABLE_COMPILER // This preprocessor directive makes editor happy
            Shared.Tests.shared
#endif
            client
        ]

[<EntryPoint>]
let main _ = Mocha.runTests all