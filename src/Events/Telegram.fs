namespace Events

open System
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot

    
[<RequireQualifiedAccess>]
module Chats =
    let mailbox = Mailbox.create
    let updateArrived (ctx: UpdateContext) =
        let result =
            processCommands ctx [|
                cmd "/register" (fun c -> mailbox.Post({ Context = c; Command = Command.Register }))
            |]
        if result then
            mailbox.Post( { Context = ctx; Command = Command.Message })