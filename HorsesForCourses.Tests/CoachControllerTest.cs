using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Controllers;
using Xunit.Sdk;
using HorsesForCourses.WebApi;

namespace HorsesForCourses.Tests;

public class CoachControllerTest
{
    public InMemoryCoachRepository repo { get; set; }

    public CoachController controller { get; set; }

    public CoachControllerTest()
    {
        repo = new();
        controller = new CoachController(repo);
    }


    [Fact]
    public void Adding_A_Coach_To_Repo_Works()
    {
        var dto = new CoachDTO("Mark", "mark@skynet.com", [], []);

        var result = controller.AddCoach(dto);

        var coachid = repo.Coaches[0].Id;

        Assert.Single(repo.Coaches);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(coachid, okResult.Value);
    }

    [Fact]
    public void GetById_Works_For_Coaches()
    {
        var dto = new CoachDTO("Mark", "mark@skynet.com", [], []);
        controller.AddCoach(dto);
        var coachid = repo.Coaches[0].Id;
        var coach = repo.Coaches[0];

        var newid = new Guid();

        var faulty = controller.GetById(newid);
        var righty = controller.GetById(coachid);

        Assert.IsType<NotFoundResult>(faulty.Result);
        var okResult = Assert.IsType<OkObjectResult>(righty.Result);

        var newdto = new CoachDTO(coach.Name, coach.Email.Value.ToString(), coach.competencies.ToList(), coach.bookings.ToList());
        Assert.Equivalent(newdto, okResult.Value);
    }


}