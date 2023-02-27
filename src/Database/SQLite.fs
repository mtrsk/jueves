namespace Database

open System

type Chat =
    { Id : int64
      TimeZone: string }

[<RequireQualifiedAccess>]
module SQLite =
    open FSharp.Data.Sql
    open Microsoft.Data.Sqlite

    [<Literal>]
    let connectionString = 
        "Data Source=" + __SOURCE_DIRECTORY__ + @"/data.db;"
    let createConnection str =
        new SqliteConnection(str)
    let createTables() =
        use conn = createConnection connectionString
        conn.Open()
        
        let createTableCommand =
            "CREATE TABLE IF NOT EXISTS Chat (Id INTEGER PRIMARY KEY, Timezone TEXT NOT NULL, LastUpdate DATETIME DEFAULT CURRENT_TIMESTAMP);"
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
        
    let update (chat: Chat) =
        use conn = createConnection connectionString
        conn.Open()
        let insertCommand = "UPDATE Chat SET LastUpdate=@timestamp WHERE Id=@id;"
        let last = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        use update = new SqliteCommand(insertCommand, conn)
        update.Parameters.AddWithValue("@id", chat.Id) |> ignore
        update.Parameters.AddWithValue("@timestamp", last) |> ignore
        update.ExecuteNonQuery() |> ignore
