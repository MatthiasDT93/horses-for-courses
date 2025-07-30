

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
        var dto = new CourseDTO(1,"cooking 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8), ["cooking"], [new Timeslot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0))]);
        var result = controller.AddCourse(dto);
        var courseid = courserepo.Courses[0].Id;

        Assert.Single(courserepo.Courses);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(courseid, okResult.Value);
    }

    [Fact]
    public void GetById_Works_For_Courses()
    {
        var dto = new CourseDTO(1,"cooking 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8), ["cooking"], [new Timeslot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0))]);
        controller.AddCourse(dto);
        var courseid = courserepo.Courses[0].Id;
        var course = courserepo.Courses[0];

        var newid = Math.Abs(Guid.NewGuid().GetHashCode());

        var faulty = controller.GetById(newid);
        var righty = controller.GetById(courseid);

        Assert.IsType<NotFoundResult>(faulty.Result);
        var okResult = Assert.IsType<OkObjectResult>(righty.Result);

        var newdto = new CourseDTO(course.CourseName, course.StartDate, course.EndDate, ["cooking"], [new Timeslot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0))]);
        Assert.Equivalent(newdto, okResult.Value);
    }

    [Fact]
    public void GetAll_Works_For_Courses()
    {
        var dto1 = new CourseDTO(1,"cooking 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8), ["cooking"], [new Timeslot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0))]);
        var dto2 = new CourseDTO(2,"cleaning 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8), ["cleaning"], [new Timeslot(DayOfWeek.Monday, new TimeOnly(12, 0), new TimeOnly(13, 0))]);
        controller.AddCourse(dto1);
        controller.AddCourse(dto2);

        var result = controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<CourseDTO>>(okResult.Value);
        Assert.True(list.Count == 2);
        Assert.Equal("cooking 101", list[0].Name);
        Assert.Equal("cleaning 101", list[1].Name);
    }

    [Fact]
    public void Modifying_Requirements_Of_A_Course_Works()
    {
        var dto = new CourseDTO(1,"cooking 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8), ["cooking", "cutting vegetables"], [new Timeslot(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0))]);
        controller.AddCourse(dto);
        var courseid = courserepo.Courses[0].Id;
        var course = courserepo.Courses[0];

        var skillsdto = new ModifyCourseSkillsDTO();
        skillsdto.SkillsToAdd = ["C#", "JavaScript"];
        skillsdto.SkillsToRemove = ["cooking", "cutting vegetables"];

        var result = controller.ModifySkills(skillsdto, courseid);

        Assert.IsType<OkResult>(result);
        // Safely unwrap ActionResult<CoachDTO>
        var getResult = controller.GetById(courseid);
        Assert.NotNull(getResult.Result); // Make sure it's not null

        var okResult = getResult.Result as OkObjectResult;
        Assert.NotNull(okResult); // Ensure we got a 200 OK

        var updatedCourse = okResult.Value as CourseDTO;
        Assert.NotNull(updatedCourse); // Ensure value exists

        Assert.Equal(["C#", "JavaScript"], updatedCourse.Requirements);
    }
}