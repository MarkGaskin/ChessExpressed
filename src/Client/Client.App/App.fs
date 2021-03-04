module Client.App.App

open Elmish
open Elmish.React
open Shared
open Fable.Remoting.Client

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif


let api =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ICEApi>

Program.mkProgram (State.init api) State.update View.view
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
