namespace HorsesForCourses.WebApi;

public class ModifyCoachSkillsDTO
{
    public List<string> SkillsToAdd = new();

    public List<string> SkillsToRemove = new();
}