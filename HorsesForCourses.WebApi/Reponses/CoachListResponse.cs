using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;


public class CoachListResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int NumberOfCoursesAssignedTo { get; set; }

    public CoachListResponse(int id, string name, string email, int nr)
    {
        Id = id;
        Name = name;
        Email = email;
        NumberOfCoursesAssignedTo = nr;
    }
}