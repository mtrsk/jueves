namespace Events

open Funogram.Telegram.Bot

type Command = Register | Message
type Background = PostMonday | PostTuesday | PostThursday

type TelegramEvent =
    { Command: Command
      Context: UpdateContext }
    
type BackgroundEvent =
    { BotConfig: Funogram.Types.BotConfig
      Operation: Background
      Destination: int64 }
 
type Envelop =
    | FromTelegram of TelegramEvent
    | FromBackground of BackgroundEvent
