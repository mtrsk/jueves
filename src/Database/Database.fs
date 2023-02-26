namespace Database

module Settings =
    open FSharp.Data.Sql
    open Microsoft.Data.Sqlite

    [<Literal>]
    let connectionString = 
        "Data Source=" + __SOURCE_DIRECTORY__ + @"/../../database/data.db;" + "Version=3;foreign keys=true"
    [<Literal>]
    let resolutionPath = __SOURCE_DIRECTORY__ + @"/../../packages/System.Data.SQLite.Core"
    
    let createConnection str =
        new SqliteConnection(str)
