namespace Domain

open FsConfig

type TelegramToken = string

[<Convention("TELEGRAM")>]
type TelegramSettings =
    { BotToken: TelegramToken }
    
type ApplicationSettings =
    { Telegram: TelegramSettings }
    static member getSettings () =
        let telegram =
            match EnvConfig.Get<TelegramSettings>() with
            | Ok config -> config
            | Error error -> 
                match error with
                | NotFound envVarName ->
                    failwithf $"Environment variable {envVarName} not found"
                | BadValue (envVarName, value) ->
                    failwithf $"Environment variable {envVarName} has invalid value {value}"
                | NotSupported msg -> failwith msg
        { Telegram = telegram }
