namespace HorsesForCourses.WebApi;

public class ModifyCourseSkillsDTO
{
    public List<string> SkillsToAdd = new();

    public List<string> SkillsToRemove = new();
}