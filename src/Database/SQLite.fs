namespace Database

open System

open Bot.Configuration

[<RequireQualifiedAccess>]
module SQLite =
    open FSharp.Data.Sql
    open Microsoft.Data.Sqlite
    
    [<Literal>]
    let connectionString = "Data Source=" + __SOURCE_DIRECTORY__ + @"/data.db;"
    let createConnection str = new SqliteConnection(str)
    let log = Settings.log

    let createTables() =
        log.Information("Bootstrapping Database...")
        try
            use conn = createConnection connectionString
            conn.Open()
            let createTableCommand =
                """
                CREATE TABLE IF NOT EXISTS Chat (
                    Id INTEGER PRIMARY KEY,
                    Timezone Int NOT NULL,
                    LastMonday DATETIME DEFAULT CURRENT_TIMESTAMP,
                    LastTuesday DATETIME DEFAULT CURRENT_TIMESTAMP,
                    LastThursday DATETIME DEFAULT CURRENT_TIMESTAMP
                );
                """
            use createTable = new SqliteCommand(createTableCommand, conn)
            createTable.ExecuteNonQuery() |> ignore
        with
        | e ->
            log.Error(e.Message)
            log.Error(e.StackTrace)
            ()
 
    let insert (chat: Chat) =
        log.Information("Inserting CHAT={@Chat}", chat)
        try
            use conn = createConnection connectionString
            conn.Open()
            let insertCommand = "INSERT INTO Chat (Id, Timezone) VALUES (@id, @timezone);"
            use insert = new SqliteCommand(insertCommand, conn)
            insert.Parameters.AddWithValue("@id", chat.Id) |> ignore
            insert.Parameters.AddWithValue("@timezone", chat.TimeZone) |> ignore
            let result = insert.ExecuteNonQuery()
            if result >= 1 then
                ()
            else
                ()
        with
        | e ->
            log.Error(e.Message)
            log.Error(e.StackTrace)
            log.Error("ChatID = {ChatId}", chat.Id)
            ()
 
    let readAll () =
        try
            use conn = createConnection connectionString
            conn.Open()
            let selectCommand = "SELECT * FROM Chat;"
            use select = new SqliteCommand(selectCommand, conn)
            let reader = select.ExecuteReader()
            let mutable results = []
            while reader.Read() do
                let chatId = reader.GetInt64(0)
                let timezone = reader.GetInt32(1)
                let lastMonday = reader.GetDateTimeOffset(2)
                let lastTuesday = reader.GetDateTimeOffset(3)
                let lastThursday = reader.GetDateTimeOffset(4)
                let last = { Monday = lastMonday; Tuesday = lastTuesday; Thursday = lastThursday }
                results <- { Id = chatId; TimeZone = timezone; LastUpdates = last } :: results
            results
        with
        | e ->
            log.Error(e.Message)
            log.Error(e.StackTrace)
            []
 
    let update (chat: Chat) (field: string) =
        try
            log.Information("Updating CHAT={@Chat}", chat)
            use conn = createConnection connectionString
            conn.Open()
            let insertCommand = $"UPDATE Chat SET Last{field}=@timestamp WHERE Id=@id;"
            let last = DateTimeOffset.UtcNow
            use update = new SqliteCommand(insertCommand, conn)
            update.Parameters.AddWithValue("@id", chat.Id) |> ignore
            update.Parameters.AddWithValue("@timestamp", last) |> ignore
            update.ExecuteNonQuery() |> ignore
        with
        | e ->
            log.Error(e.Message)
            log.Error(e.StackTrace)
            ()
