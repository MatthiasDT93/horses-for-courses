using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;

public class CourseDTO
{
    public string Name { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public List<string> Requirements { get; set; }
    public List<TimeSlotDTO> Planning { get; set; } = new();

    public CourseDTO(string name, DateOnly start, DateOnly end, List<string> req, List<Timeslot> planning)
    {
        Name = name;
        Start = start;
        End = end;
        Requirements = req;
        foreach (var slot in planning)
        {
            var newslot = new TimeSlotDTO(slot.Day, slot.Start, slot.End);
            Planning.Add(newslot);
        }
    }
}