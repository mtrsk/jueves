namespace Domain

open FsConfig

type TelegramToken = string

[<Convention("TELEGRAM")>]
type TelegramSettings =
    { BotToken: TelegramToken }
