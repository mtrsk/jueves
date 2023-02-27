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

    let compute