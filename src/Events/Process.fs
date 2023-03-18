namespace Events

open System
open System.Threading.Tasks

open Funogram.Telegram.Bot

open Bot.Configuration

[<RequireQualifiedAccess>]
module Runner =
    let log = Settings.log
    let mailbox = Mailbox.start()
    
    let updateArrived (ctx: UpdateContext) =
        let result =
            processCommands ctx [|
                cmd "/register" (fun c -> mailbox.Post(FromTelegram { Context = c; Command = Command.Register }))
            |]
        if result then
            mailbox.Post(FromTelegram { Context = ctx; Command = Command.Message })

    let checkDates (d: DateTimeOffset) (t: int) (f: DateTimeOffset -> bool) =
        let date =
            d.ToUniversalTime().Date
            |> (fun x -> Time.toDateTimeWithOffset x t)
        let next = Time.nextWeekday date
        let now = Time.toDateTimeWithOffset DateTimeOffset.UtcNow.DateTime t
        f date && (now >= next)

    let checkDatabaseChats (config: Funogram.Types.BotConfig) (c: Database.Chat) =
        let updates = c.LastUpdates
        if checkDates updates.Monday c.TimeZone Time.isMonday then
            mailbox.Post(FromBackground { BotConfig = config; Operation = PostMonday; Destination = c } )
        if checkDates updates.Tuesday c.TimeZone Time.isTuesday then
            mailbox.Post(FromBackground { BotConfig = config; Operation = PostTuesday; Destination = c } )
        if checkDates updates.Thursday c.TimeZone Time.isThursday then
            mailbox.Post(FromBackground { BotConfig = config; Operation = PostThursday; Destination = c } )

    let backgroundJob botConfig =
        log.Information("Starting background jobs...")
        let config = botConfig
        let interval = TimeSpan.FromMinutes(1)
        async {
            while true do
                let chats = Database.SQLite.readAll()
                let _ = List.map (checkDatabaseChats config) chats
                do! Task.Delay(interval) |> Async.AwaitTask
        }
        |> Async.Start
