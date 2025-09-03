namespace HorsesForCourses.Service;

public class TimeSlotDTO
{
    public DayOfWeek Day { get; set; }

    public TimeOnly Start { get; set; }

    public TimeOnly End { get; set; }

    public TimeSlotDTO() { }
    public TimeSlotDTO(DayOfWeek day, TimeOnly start, TimeOnly end)
    {
        Day = day;
        Start = start;
        End = end;
    }
}