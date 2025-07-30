using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;


public class CourseResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly startdate { get; set; }
    public DateOnly enddate { get; set; }
    public List<string> Skills { get; set; } = [];
    public List<TimeSlotDTO> timeslots { get; set; } = [];
    public Object? coach { get; set; }

    public CourseResponse(int id, string name, DateOnly start, DateOnly end, List<string> skills, List<Timeslot> slots, Coach c)
    {
        Id = id;
        Name = name;
        startdate = start;
        enddate = end;
        foreach (var skill in skills)
        {
            Skills.Add(skill);
        }
        timeslots = TimeslotDTOMapping.TimeslotList_To_DTOList(slots);
        coach = c != null ? new { Id = c.Id, Name = c.Name } : null;
    }
}