namespace Events

open System

open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot
open Funogram.Telegram.Types

type Command =
    | Register
    | Message

module Mailbox =
    type ChatId = int64
    // Define the mailbox message type
    type Envelop = {
        Command: Command
        Context: UpdateContext
    }
    
    let isThursday(date: DateTimeOffset) =
        date.DayOfWeek = DayOfWeek.Thursday

    let sendMessage (ctx: UpdateContext) (chatId: ChatId) message =
        Api.sendMessage chatId message
        |> api ctx.Config
        |> Async.Ignore
        |> Async.Start

    let processEvent (ctx: UpdateContext) (cmd: Command) (chat: Funogram.Telegram.Types.Chat) date messageId =
        match cmd with
        | Command.Register -> sendMessage ctx chat.Id "Chat successfully registered"
        | Command.Message ->
            let msg = "https://www.youtube.com/watch?v=LIOFdT6TC_w"
            sendMessage ctx chat.Id msg

    let create = 
        let rec loop (mailbox: MailboxProcessor<Envelop>) = async {
            let! message = mailbox.Receive()
            let ctx = message.Context
            let command = message.Command
            match ctx.Update.Message with
            | Some { Chat = chat; Date = date; MessageId = messageId } ->
                processEvent ctx command chat date messageId
            | _ -> ()                
            return! loop mailbox
        }
        MailboxProcessor.Start loop