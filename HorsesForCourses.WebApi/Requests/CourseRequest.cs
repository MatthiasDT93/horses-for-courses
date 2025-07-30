using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;


public class CourseRequest
{
    public string Name { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }

    public CourseRequest(string name, DateOnly start, DateOnly end)
    {
        Name = name;
        Start = start;
        End = end;
    }

    public static CourseDTO Request_To_DTO(CourseRequest request, int id)
    {
        return new CourseDTO(id, request.Name, request.Start, request.End, [], []);
    }
}