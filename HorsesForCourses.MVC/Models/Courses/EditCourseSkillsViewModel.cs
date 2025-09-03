namespace HorsesForCourses.MVC;

public class EditCourseSkillsViewModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public List<string> CurrentSkills { get; set; } = new();
    public List<string> NewSkills { get; set; } = new();
}