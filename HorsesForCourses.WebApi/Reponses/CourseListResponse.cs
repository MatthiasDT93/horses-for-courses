using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;


public class CourseListResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly startdate { get; set; }
    public DateOnly enddate { get; set; }
    public bool hasSchedule { get; set; }
    public bool hasCoach { get; set; }

    public CourseListResponse(int id, string name, DateOnly start, DateOnly end, bool schedule, bool coach)
    {
        Id = id;
        Name = name;
        startdate = start;
        enddate = end;
        hasSchedule = schedule;
        hasCoach = coach;
    }

}