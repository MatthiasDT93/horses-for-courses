using HorsesForCourses.Core;

public enum Actions
{
    add,
    remove
}

public class ModifySkillDTO
{
    public string Skill { get; set; }
    public string Action { get; set; }
}

public class ModifyTimeslotDTO
{
    public Timeslot Slot { get; set; }
    public string Action { get; set; }
}

public class ModifyCoachDTO
{
    public Coach Coach { get; set; }
    public string Action { get; set; }
}