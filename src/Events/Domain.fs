namespace Events

open Funogram.Telegram.Bot

type Command = Register | Message
type Background = PostMonday | PostTuesday | PostThursday | PostSaturday

type TelegramEvent =
    { Command: Command
      Context: UpdateContext }
    
type BackgroundEvent =
    { BotConfig: Funogram.Types.BotConfig
      Operation: Background
      Destination: Database.Chat }
 
type Envelop =
    | FromTelegram of TelegramEvent
    | FromBackground of BackgroundEvent

type RandomEvent = PostVideo | PostAnimation