using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Controllers;
using Xunit.Sdk;
using HorsesForCourses.WebApi;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using HorsesForCourses.Service;

namespace HorsesForCourses.Tests;

public class CourseControllerTest
{
    private readonly Mock<IEFCourseRepository> repo;
    private readonly Mock<IEFCoachRepository> coachrepo;

    private readonly Mock<IUnitOfWork> uow;

    private readonly Mock<ICourseService> serv;


    public CourseController controller { get; set; }

    public CourseControllerTest()
    {
        repo = new Mock<IEFCourseRepository>();
        coachrepo = new Mock<IEFCoachRepository>();
        uow = new Mock<IUnitOfWork>();
        serv = new Mock<ICourseService>();

        uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        controller = new CourseController(repo.Object, coachrepo.Object, uow.Object, serv.Object);
    }


    [Fact]
    public async void Adding_A_Course_To_Repo_Works()
    {
        var request = new CourseRequest("C# 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        var course = new Course("C# 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));

        serv.Setup(s => s.AddCourse(request.Name, request.Start, request.End)).ReturnsAsync(course);

        var result = await controller.AddCourse(request);

        serv.Verify(s => s.AddCourse("C# 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8)), Times.Once);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async void GetById_Works_For_Courses()
    {
        var request = new CourseRequest("C# 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        var course = new Course("C# 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        var response = new CourseResponse(course.Id, course.CourseName, course.StartDate, course.EndDate, course.RequiredCompetencies, course.Planning, course.coach!);

        serv.Setup(s => s.GetById(course.Id)).ReturnsAsync(response);
        serv.Setup(s => s.AddCourse(request.Name, request.Start, request.End)).ReturnsAsync(course);

        var courseid = await controller.AddCourse(request);

        var result = await controller.GetById(courseid.Value);

        serv.Verify(s => s.GetById(courseid.Value));
    }

    [Fact]
    public async Task IsPopulated_works()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // fresh DB for each test
            .Options;

        using var context = new AppDbContext(options);
        var uow = new EfUnitOfWork(context);
        var repo1 = new EFCoachRepository(context, uow);
        var repo2 = new EFCourseRepository(context, uow);


        var course = new Course("C# 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        await repo2.AddCourseToDB(course);
        await uow.SaveChangesAsync();

        // Act
        var result1 = await repo2.IsPopulated();

        // Assert
        Assert.True(result1);
    }

    [Fact]
    public async Task IsPopulated_Returns_False_When_No_Coures_Exist()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var uow = new EfUnitOfWork(context);
        var repo = new EFCoachRepository(context, uow);
        var repo2 = new EFCourseRepository(context, uow);


        var result = await repo.IsPopulated();

        Assert.False(result);
    }

    [Fact]
    public async Task GetAll_Works_For_Courses()
    {
        repo.Setup(r => r.IsPopulated()).ReturnsAsync(true);

        var result = await controller.GetAll();

        serv.Verify(s => s.GetAll(1, 5, default));
    }

    [Fact]
    public async void Adding_And_Removing_Skills_Controller_Test()
    {
        List<string> newskills = ["C#", "JavaScript"];
        var course = new Course("C#", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        repo.Setup(r => r.GetByIdIncludingCoach(1)).ReturnsAsync(course);

        var result = await controller.ModifySkills(newskills, 1);
        serv.Verify(s => s.ModifySkills(newskills, 1));
    }

    [Fact]
    public async void Service_ModifySkills_Updates_Course_Requirements()
    {
        List<string> newskills = ["C#", "JavaScript"];
        var course = new Course("C#", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        repo.Setup(r => r.GetByIdIncludingCoach(1)).ReturnsAsync(course);

        var service = new CourseService(repo.Object, coachrepo.Object, uow.Object);

        var result = await service.ModifySkills(newskills, 1);

        Assert.True(result);
        Assert.Contains("C#", course.RequiredCompetencies);
        Assert.Contains("JavaScript", course.RequiredCompetencies);
        uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void Adding_And_Removing_Timeslots_Controller_Test()
    {
        var slot1 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));
        var slot2 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(12, 0), new TimeOnly(14, 0));
        List<Timeslot> newslots = [slot1, slot2];
        var dtolist = TimeslotDTOMapping.TimeslotList_To_DTOList(newslots);
        var course = new Course("C#", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));

        repo.Setup(r => r.GetByIdIncludingCoach(1)).ReturnsAsync(course);

        var result = await controller.ModifyTimeSlots(dtolist, 1);

        serv.Verify(s => s.ModifyTimeSlots(dtolist, 1));
    }

    [Fact]
    public async void Service_ModifyTimeSlote_Updates_Course_Planning()
    {
        var slot1 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));
        var slot2 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(12, 0), new TimeOnly(14, 0));
        List<Timeslot> newslots = [slot1, slot2];
        var dtolist = TimeslotDTOMapping.TimeslotList_To_DTOList(newslots);
        var course = new Course("C#", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));

        repo.Setup(r => r.GetByIdIncludingCoach(1)).ReturnsAsync(course);

        var service = new CourseService(repo.Object, coachrepo.Object, uow.Object);

        var result = await service.ModifyTimeSlots(dtolist, 1);

        Assert.True(result);
        Assert.Contains(slot1, course.Planning);
        Assert.Contains(slot2, course.Planning);
        uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async void Confirming_A_Course_Works()
    {
        var course = new Course("C#", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        var coach = new Coach("Mark", "mark@skynet.com");
        coach.AddCompetence("C#");
        course.AddCourseMoment(Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0)));
        course.AddRequirement("C#");

        repo.Setup(r => r.GetByIdIncludingCoach(1)).ReturnsAsync(course);
        var result = await controller.ConfirmCourse(1);
        serv.Verify(s => s.ConfirmCourse(1));
    }

    [Fact]
    public async void Service_ConfirmCourse_Updates_Status()
    {
        var course = new Course("C#", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        var coach = new Coach("Mark", "mark@skynet.com");
        var service = new CourseService(repo.Object, coachrepo.Object, uow.Object);
        coach.AddCompetence("C#");
        course.AddCourseMoment(Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0)));
        course.AddRequirement("C#");

        repo.Setup(r => r.GetByIdIncludingCoach(1)).ReturnsAsync(course);
        var result = await service.ConfirmCourse(1);

        course.AddCoach(coach);

        var exception2 = Assert.Throws<Exception>(() => course.ConfirmCourse());
        Assert.Equal("Cannot confirm a course that's not in the PENDING state, current state is: FINALISED.", exception2.Message);
    }

    [Fact]
    public async void Adding_A_Coach_To_A_Course_Controller_Test()
    {
        var course = new Course("C#", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        var coach = new Coach("Mark", "mark@skynet.com");
        var service = new CourseService(repo.Object, coachrepo.Object, uow.Object);
        coach.AddCompetence("C#");
        course.AddCourseMoment(Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0)));
        course.AddRequirement("C#");

        repo.Setup(r => r.GetByIdIncludingCoach(course.Id)).ReturnsAsync(course);
        await service.ConfirmCourse(course.Id);

        var result = await controller.AssignCoach(course.Id, coach.Id);
        serv.Verify(s => s.AssignCoach(course.Id, coach.Id), Times.Once);
    }

    [Fact]
    public async void Serivce_AssignCoach_Updates_Assigned_Coach()
    {
        var course = new Course("C#", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        var coach = new Coach("Mark", "mark@skynet.com");
        var service = new CourseService(repo.Object, coachrepo.Object, uow.Object);
        coach.AddCompetence("C#");
        course.AddCourseMoment(Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0)));
        course.AddRequirement("C#");

        repo.Setup(r => r.GetByIdIncludingCoach(course.Id)).ReturnsAsync(course);
        coachrepo.Setup(r => r.GetByIdIncludingCourses(coach.Id)).ReturnsAsync(coach);
        await service.ConfirmCourse(course.Id);
        var result = await service.AssignCoach(course.Id, coach.Id);

        Assert.Equal((true, true), result);
        Assert.Equivalent(coach, course.coach);
    }
}