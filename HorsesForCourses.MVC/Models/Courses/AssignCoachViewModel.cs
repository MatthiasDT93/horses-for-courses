using HorsesForCourses.MVC.Models.Coaches;
using HorsesForCourses.Service;

namespace HorsesForCourses.MVC.Models.Courses;

public class AssignCoachViewModel
{
    public int CourseId { get; set; }

    public int CoachId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public PagedResult<CoachListResponse> Coaches { get; set; }
}