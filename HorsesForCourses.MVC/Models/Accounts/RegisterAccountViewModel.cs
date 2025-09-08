namespace HorsesForCourses.MVC.Models.Accounts;

public class RegisterAccountViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Pass { get; set; } = string.Empty;
    public string PassConfirm { get; set; } = string.Empty;
}