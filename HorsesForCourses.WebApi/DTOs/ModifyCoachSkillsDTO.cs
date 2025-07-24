namespace HorsesForCourses.WebApi;

public class ModifyCoachSkillsDTO
{
    public List<string> SkillsToAdd { get; set; } = new();

    public List<string> SkillsToRemove { get; set; } = new();
}