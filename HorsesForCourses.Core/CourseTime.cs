namespace HorsesForCourses.Core;

public record CourseTime
{
    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    private CourseTime(DateTime start, DateTime end) { StartTime = start; EndTime = end; }

    public static CourseTime From(DateTime start, DateTime end)
    {
        if (start.DayOfWeek == DayOfWeek.Saturday || start.DayOfWeek == DayOfWeek.Sunday) throw new ArgumentException("Course cannot start during the weekend.");
        if (end.DayOfWeek == DayOfWeek.Saturday || end.DayOfWeek == DayOfWeek.Sunday) throw new ArgumentException("Course cannot end during the weekend.");
        if (start.TimeOfDay < new TimeSpan(9, 0, 0) && end.TimeOfDay > new TimeSpan(17, 0, 0)) throw new ArgumentException("Course must be planned during working hours (9:00 - 17:00).");
        if (end - start < new TimeSpan(1, 0, 0)) throw new ArgumentException("Course must be one hour minimum.");

        return new CourseTime(start, end);
    }

    public bool OverlapEarly(CourseTime ct)
    {
        return ct.StartTime < StartTime && ct.EndTime > StartTime;
    }

    public bool OverlapContain(CourseTime ct)
    {
        return (ct.StartTime > StartTime && ct.EndTime < EndTime) || (ct.StartTime == StartTime && ct.EndTime == EndTime);
    }

    public bool OverlapAfter(CourseTime ct)
    {
        return ct.StartTime < EndTime && ct.EndTime > EndTime;
    }
}
