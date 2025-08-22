using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Controllers;
using Xunit.Sdk;
using HorsesForCourses.WebApi;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HorsesForCourses.Tests;

public class CoachControllerTest
{
    private readonly Mock<IEFCoachRepository> repo;

    private readonly Mock<IUnitOfWork> uow;


    public CoachController controller { get; set; }

    public CoachControllerTest()
    {
        repo = new Mock<IEFCoachRepository>();
        uow = new Mock<IUnitOfWork>();

        uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        controller = new CoachController(repo.Object, uow.Object);
    }


    [Fact]
    public async void Adding_A_Coach_To_Repo_Works()
    {
        var request = new CoachRequest("Mark", "mark@skynet.com");

        var result = await controller.AddCoach(request);
        repo.Verify(r => r.AddCoachToDB(It.Is<Coach>(
                                        c => c.Name == "Mark" && c.Email.Value == "mark@skynet.com")), Times.Once);
        //uow.Verify(r => r.SaveChangesAsync());
    }

    [Fact]
    public async void GetById_Works_For_Coaches()
    {
        var request = new CoachRequest("Mark", "mark@skynet.com");
        var coachid = await controller.AddCoach(request);

        var result = await controller.GetById(coachid.Value);

        repo.Verify(r => r.GetDTOByIdIncludingCourses(coachid.Value));
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


        var coach = new Coach("Alice", "alice@example.com");
        await repo1.AddCoachToDB(coach);
        await uow.SaveChangesAsync();

        // Act
        var result1 = await repo1.IsPopulated();

        // Assert
        Assert.True(result1);
    }

    [Fact]
    public async Task IsPopulated_Returns_False_When_No_Coaches_Exist()
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
    public async Task GetAll_Works_For_Coaches()
    {
        repo.Setup(r => r.IsPopulated()).ReturnsAsync(true);

        var result = await controller.GetAll();

        repo.Verify(r => r.GetAllDTOIncludingCourses(1, 5, default));
    }

    [Fact]
    public async void Adding_And_Removing_Skills_To_A_Coach_Works()
    {
        List<string> newskills = ["C#", "JavaScript"];
        var coach = new Coach("Mark", "mark@example.com");
        repo.Setup(r => r.GetByIdIncludingCourses(1)).ReturnsAsync(coach);

        var result = await controller.ModifySkills(newskills, 1);
        repo.Verify(r => r.GetByIdIncludingCourses(1));
        Assert.Contains("C#", coach.competencies);
        Assert.Contains("JavaScript", coach.competencies);
    }
}