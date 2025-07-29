

using HorsesForCourses.Core;
using HorsesForCourses.WebApi;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
namespace HorsesForCourses.Tests;

public class CourseControllerTest
{
    public InMemoryCourseRepository courserepo { get; set; }
    public InMemoryCoachRepository coachrepo { get; set; }

    public CourseController controller { get; set; }

    public CourseControllerTest()
    {
        courserepo = new();
        coachrepo = new();
        controller = new CourseController(courserepo, coachrepo);
    }


    [Fact]
    public void Adding_A_Course_Works()
    {
        var dto = new CourseDTO("cooking 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8), ["cooking"], [new Timeslot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0))]);
        var result = controller.AddCourse(dto);
        var courseid = courserepo.Courses[0].Id;

        Assert.Single(courserepo.Courses);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(courseid, okResult.Value);
    }

    [Fact]
    public void GetById_Works_For_Courses()
    {
        var dto = new CourseDTO("cooking 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8), ["cooking"], [new Timeslot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0))]);
        var result = controller.AddCourse(dto);
        var courseid = courserepo.Courses[0].Id;
        var course = courserepo.Courses[0];

        var newid = new Guid();

        var faulty = controller.GetById(newid);
        var righty = controller.GetById(courseid);

        Assert.IsType<NotFoundResult>(faulty.Result);
        var okResult = Assert.IsType<OkObjectResult>(righty.Result);

        var newdto = new CourseDTO(course.CourseName, course.StartDate, course.EndDate, ["cooking"], [new Timeslot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0))]);
        Assert.Equivalent(newdto, okResult.Value);
    }
}