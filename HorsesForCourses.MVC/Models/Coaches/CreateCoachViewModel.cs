

namespace HorsesForCourses.MVC.Models.Coaches;

public class CreateCoachViewModel
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public CreateCoachViewModel() { }

    public CreateCoachViewModel(string name, string email)
    {
        Name = name;
        Email = email;
    }
}