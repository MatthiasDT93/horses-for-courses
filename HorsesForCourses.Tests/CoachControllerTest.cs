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
    private readonly Mock<IEFCourseRepository> courserepo;

    private readonly Mock<IUnitOfWork> uow;

    public AppDbContext context { get; set; }

    public CoachController controller { get; set; }

    public CoachControllerTest()
    {
        courserepo = new Mock<IEFCourseRepository>();
        repo = new Mock<IEFCoachRepository>();
        controller = new CoachController(repo.Object, uow.Object);
    }


    [Fact]
    public async void Adding_A_Coach_To_Repo_Works()
    {
        var request = new CoachRequest("Mark", "mark@skynet.com");

        var result = await controller.AddCoach(request);
        repo.Verify(r => r.AddCoachToDB(It.Is<Coach>(
                                        c => c.Name == "Mark" && c.Email.Value == "mark@skynet.com")), Times.Once);
        repo.Verify(r => r.Save());
    }

    [Fact]
    public async void GetById_Works_For_Coaches()
    {
        var request = new CoachRequest("Mark", "mark@skynet.com");
        var coachid = await controller.AddCoach(request);

        var result = await controller.GetById(coachid.Value);

        repo.Verify(r => r.GetByIdIncludingCourses(coachid.Value));
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

        var coach = new Coach("Alice", "alice@example.com");
        await repo1.AddCoachToDB(coach);
        await repo1.Save();

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
        var repo = new EFCoachRepository(context);

        var result = await repo.IsPopulated();

        Assert.False(result);
    }

    [Fact(Skip = "wip")]
    public async Task GetAll_Works_For_Coaches()
    {
        repo.Setup(r => r.IsPopulated()).ReturnsAsync(true);

        var result = await controller.GetAll();

        repo.Verify(r => r.GetAllIncludingCourses());

        // var okResult = Assert.IsType<OkObjectResult>(result.Result);
        // var list = Assert.IsType<List<CoachListResponse>>(okResult.Value);
        // Assert.True(list.Count == 2);
        // Assert.Equal("Mark", list[0].Name);
        // Assert.Equal("Bob", list[1].Name);
    }

    [Fact(Skip = "wip")]
    public async void Adding_And_Removing_Skills_To_A_Coach_Works()
    {
        var request1 = new CoachRequest("Mark", "mark@skynet.com");
        var coachid = await controller.AddCoach(request1);

        List<string> newskills = ["C#", "JavaScript"];


        var result = await controller.ModifySkills(newskills, coachid.Value);

        Assert.IsType<OkResult>(result);
        // Safely unwrap ActionResult<CoachDTO>
        var getResult = await controller.GetById(coachid.Value);
        Assert.NotNull(getResult.Result); // Make sure it's not null

        var okResult = Assert.IsType<OkObjectResult>(getResult.Result);
        Assert.NotNull(okResult); // Ensure we got a 200 OK

        var updatedCoach = okResult.Value as CoachDTO;
        Assert.NotNull(updatedCoach); // Ensure value exists

        Assert.Equal(["C#", "JavaScript"], updatedCoach.Competencies);
    }
}