namespace Events

open System
open System.Threading.Tasks

open Funogram.Telegram.Bot

[<RequireQualifiedAccess>]
module Runner =
    let mailbox = Mailbox.start()

    let updateArrived (ctx: UpdateContext) =
        let result =
            processCommands ctx [|
                cmd "/register" (fun c -> mailbox.Post(FromTelegram { Context = c; Command = Command.Register }))
            |]
        if result then
            mailbox.Post(FromTelegram { Context = ctx; Command = Command.Message })

    let backgroundJob botConfig =
        let config = botConfig
        async {
            while true do
                let now = DateTimeOffset.UtcNow
                let destination = 1L
                if Time.isMonday(now) then
                    mailbox.Post(FromBackground { BotConfig = config; Operation = PostMonday; Destination = destination } )
                if Time.isTuesday(now) then
                    mailbox.Post(FromBackground { BotConfig = config; Operation = PostTuesday; Destination = destination } )
                if Time.isThursday(now) then
                    mailbox.Post(FromBackground { BotConfig = config; Operation = PostThursday; Destination = destination } )
                do! Task.Delay(TimeSpan.FromSeconds(60.0)) |> Async.AwaitTask
        }
        |> Async.Start