namespace Database

open System

type Updates =
    { Monday: DateTimeOffset
      Tuesday: DateTimeOffset
      Thursday: DateTimeOffset }
    static member init() =
        { Monday = DateTimeOffset.UtcNow; Tuesday = DateTimeOffset.UtcNow; Thursday = DateTimeOffset.UtcNow }
type Chat =
    { Id : int64
      TimeZone: int
      LastUpdates: Updates }
