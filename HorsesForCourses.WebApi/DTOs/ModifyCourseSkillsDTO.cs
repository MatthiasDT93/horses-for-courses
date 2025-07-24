namespace HorsesForCourses.WebApi;

public class ModifyCourseSkillsDTO
{
    public List<string> SkillsToAdd { get; set; } = new();

    public List<string> SkillsToRemove { get; set; } = new();
}