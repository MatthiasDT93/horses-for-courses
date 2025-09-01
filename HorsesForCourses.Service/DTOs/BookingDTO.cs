using HorsesForCourses.Core;

namespace HorsesForCourses.Service;

public class BookingDTO
{
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public List<TimeSlotDTO> Planning { get; set; } = new();

    public BookingDTO() { }

    public BookingDTO(List<TimeSlotDTO> planning, DateOnly start, DateOnly end)
    {
        Planning = planning;
        Start = start;
        End = end;
    }
}