using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Controllers;
using Xunit.Sdk;
using HorsesForCourses.WebApi;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HorsesForCourses.Tests;

public class CourseControllerTest
{
    private readonly Mock<IEFCourseRepository> repo;
    private readonly Mock<IEFCoachRepository> coachrepo;

    private readonly Mock<IUnitOfWork> uow;


    public CourseController controller { get; set; }

    public CourseControllerTest()
    {
        repo = new Mock<IEFCourseRepository>();
        coachrepo = new Mock<IEFCoachRepository>();
        uow = new Mock<IUnitOfWork>();

        uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        controller = new CourseController(repo.Object, coachrepo.Object, uow.Object);
    }


    [Fact]
    public async void Adding_A_Course_To_Repo_Works()
    {
        var request = new CourseRequest("C# 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));

        var result = await controller.AddCourse(request);
        repo.Verify(r => r.AddCourseToDB(It.Is<Course>(
                                        c => c.CourseName == "C# 101" && c.StartDate == new DateOnly(2025, 8, 8) && c.EndDate == new DateOnly(2026, 8, 8))), Times.Once);
        uow.Verify(r => r.SaveChangesAsync());
    }

    [Fact]
    public async void GetById_Works_For_Courses()
    {
        var request = new CourseRequest("C# 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        var courseid = await controller.AddCourse(request);

        var result = await controller.GetById(courseid.Value);

        repo.Verify(r => r.GetDTOByIdIncludingCoach(courseid.Value));
    }

    [Fact]
    public async Task IsPopulated_works()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // fresh DB for each test
            .Options;

        using var context = new AppDbContext(options);
        var repo1 = new EFCoachRepository(context);
        var repo2 = new EFCourseRepository(context);
        var uow = new EfUnitOfWork(context, repo1, repo2);

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
        var repo = new EFCoachRepository(context);
        var repo2 = new EFCourseRepository(context);
        var uow = new EfUnitOfWork(context, repo, repo2);

        var result = await repo.IsPopulated();

        Assert.False(result);
    }

    [Fact]
    public async Task GetAll_Works_For_Courses()
    {
        repo.Setup(r => r.IsPopulated()).ReturnsAsync(true);

        var result = await controller.GetAll();

        repo.Verify(r => r.GetAllDTOIncludingCoach(1, 5, default));
    }

    [Fact]
    public async void Adding_And_Removing_Skills_To_A_Course_Works()
    {
        List<string> newskills = ["C#", "JavaScript"];
        var course = new Course("C#", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        repo.Setup(r => r.GetByIdIncludingCoach(1)).ReturnsAsync(course);

        var result = await controller.ModifySkills(newskills, 1);
        repo.Verify(r => r.GetByIdIncludingCoach(1));
        Assert.Contains("C#", course.RequiredCompetencies);
        Assert.Contains("JavaScript", course.RequiredCompetencies);
    }

    [Fact]
    public async void Adding_And_Removing_Timeslots_To_A_Course_Works()
    {
        var slot1 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));
        var slot2 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(12, 0), new TimeOnly(14, 0));
        List<Timeslot> newslots = [slot1, slot2];
        var dtolist = TimeslotDTOMapping.TimeslotList_To_DTOList(newslots);
        var course = new Course("C#", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));

        repo.Setup(r => r.GetByIdIncludingCoach(1)).ReturnsAsync(course);

        var result = await controller.ModifyTimeSlots(dtolist, 1);

        repo.Verify(r => r.GetByIdIncludingCoach(1));
        Assert.Contains(slot1, course.Planning);
        Assert.Contains(slot2, course.Planning);
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
        repo.Verify(r => r.GetByIdIncludingCoach(1));

        course.AddCoach(coach);

        var exception2 = Assert.Throws<Exception>(() => course.ConfirmCourse());
        Assert.Equal("Cannot confirm a course that's not in the PENDING state, current state is: FINALISED.", exception2.Message);

    }

    [Fact]
    public async void Adding_A_Coach_To_A_Course_Works()
    {
        var course = new Course("C#", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
        var coach = new Coach("Mark", "mark@skynet.com");
        coach.AddCompetence("C#");
        course.AddCourseMoment(Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0)));
        course.AddRequirement("C#");

        repo.Setup(r => r.GetByIdIncludingCoach(1)).ReturnsAsync(course);
        var result = await controller.ConfirmCourse(1);
        repo.Verify(r => r.GetByIdIncludingCoach(1));

        course.AddCoach(coach);

        Assert.Equal(coach, course.coach);
    }
}