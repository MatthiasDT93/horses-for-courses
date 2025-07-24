namespace HorsesForCourses.WebApi;

public class CourseDTO
{
    public string Name { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
}