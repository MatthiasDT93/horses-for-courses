namespace HorsesForCourses.MVC;

public class EditCourseSkillsViewModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
}