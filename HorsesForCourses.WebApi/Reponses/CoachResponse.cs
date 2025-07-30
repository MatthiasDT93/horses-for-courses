using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;


public class CoachResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> Skills { get; set; } = [];
    public List<Object> Courses { get; set; } = [];

    public CoachResponse(int id, string name, string email, IReadOnlyList<string> skills, List<Course> courses)
    {
        Id = id;
        Name = name;
        Email = email;
        foreach (var skill in skills)
        {
            Skills.Add(skill);
        }
        foreach (var course in courses)
        {
            Courses.Add(new { course.Id, course.CourseName });
        }
    }
}