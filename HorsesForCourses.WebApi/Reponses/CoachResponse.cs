using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;

public class CoachReponseCourseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }

    public CoachReponseCourseDTO(int id, string name)
    {
        Id = id;
        Name = name;
    }
}

public class CoachResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public IReadOnlyList<string> Skills { get; set; } = [];
    public IEnumerable<CoachReponseCourseDTO> Courses { get; set; } = [];

    public CoachResponse(int id, string name, string email, IReadOnlyList<string> skills, IEnumerable<CoachReponseCourseDTO> courses)
    {
        Id = id;
        Name = name;
        Email = email;
        Skills = skills;
        Courses = courses;
    }
}