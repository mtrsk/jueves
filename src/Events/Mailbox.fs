namespace Events

open System

open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot
open Funogram.Telegram.Types

open Database
open Funogram.Types

module Mailbox =
    let sendMessage (config: Funogram.Types.BotConfig) chatId message =
        Api.sendMessage chatId message
        |> api config
        |> Async.Ignore
        |> Async.Start

    let handleCommand (ctx: UpdateContext) (cmd: Command) (chat: Funogram.Telegram.Types.Chat) =
        match cmd with
        | Command.Register ->
            let timezone = -3
            let newChat = { Id = chat.Id; TimeZone = timezone; LastUpdate = DateTimeOffset.UtcNow }
            SQLite.insert newChat
            sendMessage ctx.Config chat.Id $"Chat '{chat.Title}' successfully registered!"
        | Command.Message -> sendMessage ctx.Config chat.Id "Got a message"

    let pickRandomItem (data: 'T list) =
        // TODO: Find a better way to do this in F#
        let rnd = System.Random()
        data
        |> List.sortBy (fun _ -> rnd.Next())
        |> List.head

    let handleBackgroundTasks config (operation: Background) (destination: Database.Chat) =
        match operation with
        | PostMonday ->
            let videos =
                [ "https://www.youtube.com/watch?v=1_oHK2fzGe8"
                  "https://www.youtube.com/watch?v=WrulZzDYM6s" ]
            let msg = pickRandomItem videos
            sendMessage config destination.Id msg
        | PostTuesday -> ()
        | PostThursday ->
            let videos =
                [ "https://www.youtube.com/watch?v=R5bQrotNfok"
                  "https://www.youtube.com/watch?v=YE4IvymZQPY"
                  "https://www.youtube.com/watch?v=LIOFdT6TC_w" ]
            let msg = pickRandomItem videos
            sendMessage config destination.Id msg

    let parseMailboxMessage (envelop: Envelop) =
        match envelop with
        | FromTelegram { Context = ctx; Command = cmd } ->
            match ctx.Update.Message with
            | Some { Chat = chat; MessageId = messageId } ->
                handleCommand ctx cmd chat 
            | _ -> ()
        | FromBackground { BotConfig = config; Operation = operation; Destination = chatId } ->
            printfn "Running background process..."
            handleBackgroundTasks config operation chatId

    let start () = 
        let rec loop (mailbox: MailboxProcessor<Envelop>) =
            async {
                let! message = mailbox.Receive()
                parseMailboxMessage message
                return! loop mailbox
            }
        MailboxProcessor.Start loop
