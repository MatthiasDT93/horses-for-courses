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

    public AppDbContext context { get; set; }

    public CoachController controller { get; set; }

    public CoachControllerTest()
    {
        repo = new Mock<IEFCoachRepository>();
        controller = new CoachController(repo);
    }


    [Fact]
    public async void Adding_A_Coach_To_Repo_Works()
    {
        var request = new CoachRequest("Mark", "mark@skynet.com");

        var result = await controller.AddCoach(request);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var coachid = Assert.IsType<int>(okResult.Value);

        // Get the coach back from controller (which uses the repo)
        var getResult = await controller.GetById(coachid);
        var getOkResult = Assert.IsType<OkObjectResult>(getResult.Result);
        var coachDto = Assert.IsType<CoachResponse>(getOkResult.Value);

        Assert.Equal("Mark", coachDto.Name);
        Assert.Equal("mark@skynet.com", coachDto.Email);
    }

    // [Fact(Skip = "wip")]
    // public async void GetById_Works_For_Coaches()
    // {
    //     var request = new CoachRequest("Mark", "mark@skynet.com");
    //     var coachid = await controller.AddCoach(request);

    //     var coach = await repo.GetByIdIncludingCourses(coachid.Value);

    //     var newid = Math.Abs(Guid.NewGuid().GetHashCode());

    //     var faulty = await controller.GetById(newid);
    //     var righty = await controller.GetById(coachid.Value);

    //     Assert.IsType<NotFoundResult>(faulty.Result);
    //     var okResult = Assert.IsType<OkObjectResult>(righty.Result);

    //     var newdto = new CoachDTO(coach.Name, coach.Email.Value.ToString(), coach.competencies.ToList(), coach.bookings.ToList());
    //     Assert.Equivalent(newdto, okResult.Value);
    // }

    [Fact]
    public async Task GetAll_Works_For_Coaches()
    {
        var request1 = new CoachRequest("Mark", "mark@skynet.com");
        var request2 = new CoachRequest("Mark", "mark@skynet.com");

        await controller.AddCoach(request1);
        await controller.AddCoach(request2);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<CoachDTO>>(okResult.Value);
        Assert.True(list.Count == 2);
        Assert.Equal("Mark", list[0].Name);
        Assert.Equal("Bob", list[1].Name);
    }

    [Fact]
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