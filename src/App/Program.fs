open FsConfig
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot

open Configuration
open Database
open Events

[<EntryPoint>]
let main argv =
    let settings = ApplicationSettings.getSettings()
    async {
        let config = { Config.defaultConfig with Token = settings.Telegram.BotToken }
        SQLite.createTables()
        let! _ = Api.deleteWebhookBase () |> api config
        let _ = Runner.backgroundJob config
        return! startBot config Runner.updateArrived None
    }
    |> Async.RunSynchronously
    0
