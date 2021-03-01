module Server.Tests

open Expecto

open Shared
open Server

let server = testList "Server" [
    testCase "Adding valid Chess Game" <| fun _ ->
        let storage = Storage()
        let validTodo = ChessGame.defaultGame
        let expectedResult = Ok ()

        let result = storage.AddChessGame validTodo

        Expect.equal result expectedResult "Result should be ok"
        Expect.contains (storage.GetTodos()) validTodo "Storage should contain new todo"
]

let all =
    testList "All"
        [
            Shared.Tests.shared
            server
        ]

[<EntryPoint>]
let main _ = runTests defaultConfig all