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

public class CoachControllerTest
{
    private readonly Mock<IEFCoachRepository> repo;

    private readonly Mock<IUnitOfWork> uow;

    private readonly Mock<ICoachService> serv;


    public CoachController controller { get; set; }

    public CoachControllerTest()
    {
        repo = new Mock<IEFCoachRepository>();
        uow = new Mock<IUnitOfWork>();
        serv = new Mock<ICoachService>();

        uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        controller = new CoachController(repo.Object, uow.Object, serv.Object);
    }


    [Fact]
    public async void Adding_A_Coach_To_Repo_Works()
    {
        // Arrange
        var request = new CoachRequest("Mark", "mark@skynet.com");
        var coach = new Coach("Mark", "mark@skynet.com");

        serv.Setup(s => s.AddCoach(request.Name, request.Email)).ReturnsAsync(coach);

        // Act
        var result = await controller.AddCoach(request);

        // Assert
        serv.Verify(s => s.AddCoach("Mark", "mark@skynet.com"), Times.Once);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async void GetById_Works_For_Coaches()
    {
        var request = new CoachRequest("Mark", "mark@skynet.com");
        var coach = new Coach("Mark", "mark@skynet.com");
        var response = new CoachResponse(coach.Id, coach.Name, coach.Email.Value, coach.competencies, []);

        serv.Setup(s => s.GetById(coach.Id)).ReturnsAsync(response);

        var result = await controller.GetById(coach.Id);

        serv.Verify(s => s.GetById(coach.Id));
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

        serv.Verify(r => r.GetAll(1, 5, default));
    }

    [Fact]
    public async void Adding_And_Removing_Skills_Controller_Test()
    {
        List<string> newskills = ["C#", "JavaScript"];
        var coach = new Coach("Mark", "mark@example.com");
        repo.Setup(r => r.GetByIdIncludingCourses(1)).ReturnsAsync(coach);

        var result = await controller.ModifySkills(newskills, 1);
        serv.Verify(s => s.ModifySkills(newskills, 1));

    }

    [Fact]
    public async Task Service_ModifySkills_Updates_Coach_Competencies()
    {
        // Arrange
        var newskills = new List<string> { "C#", "JavaScript" };
        var coach = new Coach("Mark", "mark@example.com");

        repo.Setup(r => r.GetByIdIncludingCourses(1)).ReturnsAsync(coach);

        var service = new CoachService(repo.Object, uow.Object);

        // Act
        var result = await service.ModifySkills(newskills, 1);

        // Assert
        Assert.True(result);
        Assert.Contains("C#", coach.competencies);
        Assert.Contains("JavaScript", coach.competencies);
        uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}