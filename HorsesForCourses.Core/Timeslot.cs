namespace HorsesForCourses.Core;

public record Timeslot
{
    public DayOfWeek Day { get; set; }

    public TimeOnly Start { get; set; }

    public TimeOnly End { get; set; }

    public Timeslot(DayOfWeek day, TimeOnly start, TimeOnly end) { Day = day; Start = start; End = end; }

    public static Timeslot From(DayOfWeek day, TimeOnly start, TimeOnly end)
    {
        if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday) throw new ArgumentException("Course cannot take place during the weekend.");
        if (start < new TimeOnly(9, 0) || end > new TimeOnly(17, 0)) throw new ArgumentException("Course must be planned during working hours (9:00 - 17:00).");
        if (end - start < new TimeSpan(1, 0, 0)) throw new ArgumentException("Course must be one hour minimum.");

        return new Timeslot(day, start, end);
    }

    private bool OverlapEarly(Timeslot slot)
    {
        return slot.Day == Day && slot.Start < Start && slot.End > Start;
    }

    private bool OverlapContain(Timeslot slot)
    {
        return (slot.Day == Day && slot.Start > Start && slot.End < End) || (slot.Day == Day && slot.Start == Start && slot.End == End);
    }

    private bool OverlapAfter(Timeslot slot)
    {
        return slot.Day == Day && slot.Start < End && slot.End > End;
    }

    private bool OverlapEqual(Timeslot slot)
    {
        return slot.Day == Day && slot.Start == Start && slot.End == End;
    }

    public bool Overlap(Timeslot slot)
    {
        return OverlapEarly(slot) || OverlapContain(slot) || OverlapAfter(slot) || OverlapEqual(slot);
    }
}
