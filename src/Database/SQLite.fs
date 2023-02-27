namespace Database

open System

[<RequireQualifiedAccess>]
module SQLite =
    open FSharp.Data.Sql
    open Microsoft.Data.Sqlite

    [<Literal>]
    let connectionString = "Data Source=" + __SOURCE_DIRECTORY__ + @"/data.db;"
    let createConnection str = new SqliteConnection(str)

    let createTables() =
        use conn = createConnection connectionString
        conn.Open()
        let createTableCommand =
            "CREATE TABLE IF NOT EXISTS Chat (Id INTEGER PRIMARY KEY, Timezone Int NOT NULL, LastUpdate DATETIME DEFAULT CURRENT_TIMESTAMP);"
        use createTable = new SqliteCommand(createTableCommand, conn)
        createTable.ExecuteNonQuery() |> ignore
 
    let insert (chat: Chat) =
        use conn = createConnection connectionString
        conn.Open()
        let insertCommand = "INSERT INTO Chat (Id, Timezone) VALUES (@id, @timezone);"
        use insert = new SqliteCommand(insertCommand, conn)
        insert.Parameters.AddWithValue("@id", chat.Id) |> ignore
        insert.Parameters.AddWithValue("@timezone", chat.TimeZone) |> ignore
        insert.ExecuteNonQuery() |> ignore
 
    let readAll () =
        use conn = createConnection connectionString
        conn.Open()
        let selectCommand = "SELECT Id, Timezone, LastUpdate FROM Chat;"
        use select = new SqliteCommand(selectCommand, conn)
        let reader = select.ExecuteReader()
        let mutable results = []
        while reader.Read() do
            let chatId = reader.GetInt64(0)
            let timezone = reader.GetInt32(1)
            let last = reader.GetDateTimeOffset(2)
            results <- { Id = chatId; TimeZone = timezone; LastUpdate = last } :: results
        results
 
    let update (chat: Chat) =
        use conn = createConnection connectionString
        conn.Open()
        let insertCommand = "UPDATE Chat SET LastUpdate=@timestamp WHERE Id=@id;"
        let last = DateTimeOffset.UtcNow
        use update = new SqliteCommand(insertCommand, conn)
        update.Parameters.AddWithValue("@id", chat.Id) |> ignore
        update.Parameters.AddWithValue("@timestamp", last) |> ignore
        update.ExecuteNonQuery() |> ignore
