namespace Events

open System

[<RequireQualifiedAccess>]
module Time =
    let isMonday(date: DateTimeOffset) =
        date.DayOfWeek = DayOfWeek.Monday

    let isTuesday(date: DateTimeOffset) =
        date.DayOfWeek = DayOfWeek.Tuesday

    let isThursday(date: DateTimeOffset) =
        date.DayOfWeek = DayOfWeek.Thursday

    let toTimespan (hours: int) = TimeSpan(hours, 0, 0)
 
    let toDateTimeWithOffset (date: DateTime) (offset: int) =
        let timespan = toTimespan offset
        DateTimeOffset(date, timespan)
 
    let nextWeekday (date: DateTimeOffset) =
        let next = date.AddDays(7)
        DateTimeOffset(next.Year, next.Month, next.Day, 0, 0, 0, 0, next.Offset)
