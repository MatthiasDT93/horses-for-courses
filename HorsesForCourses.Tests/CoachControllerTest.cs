using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Controllers;
using Xunit.Sdk;
using HorsesForCourses.WebApi;

namespace HorsesForCourses.Tests;

public class CoachControllerTest
{
    public InMemoryCoachRepository repo { get; set; }

    public AppDbContext context { get; set; }

    public CoachController controller { get; set; }

    public CoachControllerTest()
    {
        repo = new();

        controller = new CoachController(repo, context);
    }


    [Fact]
    public async void Adding_A_Coach_To_Repo_Works()
    {
        var request = new CoachRequest("Mark", "mark@skynet.com");

        var result = await controller.AddCoach(request);

        var coachid = repo.Coaches[0].Id;

        Assert.Single(repo.Coaches);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(coachid, okResult.Value);
    }

    [Fact]
    public void GetById_Works_For_Coaches()
    {
        var request = new CoachRequest("Mark", "mark@skynet.com");
        controller.AddCoach(request);
        var coachid = repo.Coaches[0].Id;
        var coach = repo.Coaches[0];

        var newid = Math.Abs(Guid.NewGuid().GetHashCode());

        var faulty = controller.GetById(newid);
        var righty = controller.GetById(coachid);

        Assert.IsType<NotFoundResult>(faulty.Result);
        var okResult = Assert.IsType<OkObjectResult>(righty.Result);

        var newdto = new CoachDTO(coach.Name, coach.Email.Value.ToString(), coach.competencies.ToList(), coach.bookings.ToList());
        Assert.Equivalent(newdto, okResult.Value);
    }

    [Fact]
    public void GetAll_Works_For_Coaches()
    {
        var dto1 = new CoachDTO(1, "Mark", "mark@skynet.com", ["cooking"], []);
        var dto2 = new CoachDTO(2, "Bob", "Bob@skynet.com", ["cooking"], []);


        controller.AddCoach(dto1);
        controller.AddCoach(dto2);

        var result = controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsType<List<CoachDTO>>(okResult.Value);
        Assert.True(list.Count == 2);
        Assert.Equal("Mark", list[0].Name);
        Assert.Equal("Bob", list[1].Name);
    }

    [Fact]
    public void Adding_And_Removing_Skills_To_A_Coach_Works()
    {
        var dto = new CoachDTO(1, "Mark", "mark@skynet.com", ["cooking", "football"], []);
        controller.AddCoach(dto);
        var coachid = repo.Coaches[0].Id;

        var skillsdto = new ModifyCoachSkillsDTO();
        skillsdto.SkillsToAdd = ["C#", "JavaScript"];
        skillsdto.SkillsToRemove = ["cooking", "football"];

        var result = controller.ModifySkills(skillsdto, coachid);

        Assert.IsType<OkResult>(result);
        // Safely unwrap ActionResult<CoachDTO>
        var getResult = controller.GetById(coachid);
        Assert.NotNull(getResult.Result); // Make sure it's not null

        var okResult = getResult.Result as OkObjectResult;
        Assert.NotNull(okResult); // Ensure we got a 200 OK

        var updatedCoach = okResult.Value as CoachDTO;
        Assert.NotNull(updatedCoach); // Ensure value exists

        Assert.Equal(["C#", "JavaScript"], updatedCoach.Competencies);
    }
}