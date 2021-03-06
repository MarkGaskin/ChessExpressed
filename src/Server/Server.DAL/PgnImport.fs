module Server.DAL.PgnImport

open Shared

open System.IO
open BatchAnalysis

let importGames (directoryPath: string) =
    async {
        return result{
            Directory.GetFiles(directoryPath) 
            |> Array.map Path.GetFileName 
            |> Array.iter (printfn "%s")

            |> ignore
            return ()
        }
    }

let pgnApi : PGNApi =
    { ImportFromPath = importGames }