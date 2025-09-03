namespace HorsesForCourses.MVC;

public class EditCoachSkillsViewModel
{
    public int CoachId { get; set; }
    public string CoachName { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
}