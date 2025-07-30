using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;


public class CoachRequest
{
    public string Name { get; set; }
    public string Email { get; set; }

    public CoachRequest(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public static CoachDTO Request_To_DTO(CoachRequest request)
    {
        return new CoachDTO(request.Name, request.Email, [], []);
    }
}