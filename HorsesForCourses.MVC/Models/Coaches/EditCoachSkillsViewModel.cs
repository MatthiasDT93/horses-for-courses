namespace HorsesForCourses.MVC;

public class EditCoachSkillsViewModel
{
    public int CoachId { get; set; }
    public string CoachName { get; set; } = string.Empty;
    public List<string> CurrentSkills { get; set; } = new();
    public List<string> NewSkills { get; set; } = new();
}