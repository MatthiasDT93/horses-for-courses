namespace HorsesForCourses.WebApi;

public class TimeSlotDTO
{
    public DayOfWeek Day { get; set; }

    public TimeOnly Start { get; set; }

    public TimeOnly End { get; set; }
}