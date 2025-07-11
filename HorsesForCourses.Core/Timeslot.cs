namespace HorsesForCourses.Core;

public record Timeslot
{
    public DateOnly Day { get; set; }

    public TimeOnly Start { get; set; }

    public TimeOnly End { get; set; }

    private Timeslot(DateOnly day, TimeOnly start, TimeOnly end) { Day = day; Start = start; End = end; }

    public static Timeslot From(DateOnly day, TimeOnly start, TimeOnly end)
    {
        if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday) throw new ArgumentException("Course cannot take place during the weekend.");
        if (start < new TimeOnly(9, 0) || end > new TimeOnly(17, 0)) throw new ArgumentException("Course must be planned during working hours (9:00 - 17:00).");
        if (end - start < new TimeSpan(1, 0, 0)) throw new ArgumentException("Course must be one hour minimum.");

        return new Timeslot(day, start, end);
    }

    //     private bool OverlapEarly(Timeslot slot)
    //     {
    //         return slot.Start < Start && slot.End > Start;
    //     }

    //     private bool OverlapContain(Timeslot slot)
    //     {
    //         return (slot.Start > Start && slot.End < End) || (slot.Start == Start && slot.End == End);
    //     }

    //     private bool OverlapAfter(Timeslot slot)
    //     {
    //         return slot.Start < End && slot.End > End;
    //     }

    //     public bool Overlap(Timeslot slot)
    //     {
    //         return OverlapEarly(slot) || OverlapContain(slot) || OverlapAfter(slot);
    //     }
}
