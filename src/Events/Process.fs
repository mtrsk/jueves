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
 
    let checkDatabaseChats (config: Funogram.Types.BotConfig) (c: Database.Chat) =
        let last = c.LastUpdate.ToUniversalTime().Date
        let date = Time.toDateTimeWithOffset last c.TimeZone
        let next = Time.nextWeekday date
        let now = Time.toDateTimeWithOffset DateTimeOffset.UtcNow.DateTime c.TimeZone
        if Time.isMonday(date) && (now >= next) then
            mailbox.Post(FromBackground { BotConfig = config; Operation = PostMonday; Destination = c } )
        if Time.isTuesday(date) && (now >= next) then
            mailbox.Post(FromBackground { BotConfig = config; Operation = PostTuesday; Destination = c } )
        if Time.isThursday(date) && (now >= next) then
            mailbox.Post(FromBackground { BotConfig = config; Operation = PostThursday; Destination = c } )

    let backgroundJob botConfig =
        let config = botConfig
        async {
            while true do
                let chats = Database.SQLite.readAll()
                let _ = List.map (checkDatabaseChats config) chats
                do! Task.Delay(TimeSpan.FromSeconds(10.0)) |> Async.AwaitTask
        }
        |> Async.Start
