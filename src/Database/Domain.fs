namespace Database

open System

type Updates =
    { Monday: DateTimeOffset
      Tuesday: DateTimeOffset
      Thursday: DateTimeOffset
      Saturday: DateTimeOffset }
    static member init() =
        let now = DateTimeOffset.UtcNow
        { Monday = now; Tuesday = now; Thursday = now; Saturday = now }
type Chat =
    { Id : int64
      TimeZone: int
      LastUpdates: Updates }
