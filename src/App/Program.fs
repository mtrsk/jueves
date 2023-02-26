open System
open System.Threading.Tasks

open FsConfig
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot

open Domain
open Events

[<EntryPoint>]
let main argv =
    let settings = ApplicationSettings.getSettings()
    async {
        let config = { Config.defaultConfig with Token = settings.Telegram.BotToken }
        let! _ = Api.deleteWebhookBase () |> api config
        return! startBot config Chats.updateArrived None
    }
    |> Async.RunSynchronously
    0
