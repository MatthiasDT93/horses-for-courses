using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;

public class CourseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public List<string> Requirements { get; set; }
    public List<TimeSlotDTO> Planning { get; set; } = new();

    public CourseDTO() { }

    public CourseDTO(string name, DateOnly start, DateOnly end, List<string> req, List<TimeSlotDTO> planning)
    {
        Name = name;
        Start = start;
        End = end;
        Requirements = req;
        Planning = planning;
    }
}