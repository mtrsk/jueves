open System
open System.Threading.Tasks

open FsConfig
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot

type TelegramToken = string
type ChatId = string

type Settings =
    { BotToken: TelegramToken }

let isJueves() =
    DateTime.Today.DayOfWeek = DayOfWeek.Thursday

let sendMessage (ctx: UpdateContext) chatId messageId =
    Api.sendMessageReply chatId "Hello, world!" messageId
    |> api ctx.Config

let updateArrived (ctx: UpdateContext) =
  match ctx.Update.Message with
  | Some { MessageId = messageId; Chat = chat } ->
    Api.sendMessageReply chat.Id "Hello, world!" messageId |> api ctx.Config
    |> Async.Ignore
    |> Async.Start
  | _ -> ()

[<EntryPoint>]
let main argv =
    let settings = 
        Configuration.load<Settings>()
        |> Configuration.fromEnvironmentVariables()

    async {
        let config = { Config.defaultConfig with Token = settings.BotToken }
        let! _ = Api.deleteWebhookBase () |> api config
        return! startBot config updateArrived None
    }
    |> Async.RunSynchronously
    0
