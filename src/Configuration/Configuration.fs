namespace Bot.Configuration

open FsConfig
open Serilog
open Serilog.Core
open Serilog.Sinks.SystemConsole.Themes

type TelegramUsername = string
type TelegramToken = string

[<Convention("TELEGRAM")>]
type TelegramSettings =
    { BotToken: TelegramToken
      [<ListSeparator(',')>]
      AdminUsernames: TelegramUsername list }

type ApplicationSettings =
    { Telegram: TelegramSettings; Logger: Logger }
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
        let logger =
            LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    theme = AnsiConsoleTheme.Code,
                    outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [Thread <{ThreadId}>][{ThreadName}] {Message:lj}{NewLine}{Exception}")
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .CreateLogger()

        { Telegram = telegram; Logger = logger }

[<RequireQualifiedAccess>]
module Settings =
    let settings = ApplicationSettings.getSettings()
    let log = settings.Logger
