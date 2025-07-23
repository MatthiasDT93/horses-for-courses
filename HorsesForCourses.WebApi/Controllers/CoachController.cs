using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("/coaches")]
public class CoachController : ControllerBase
{

    private readonly ILogger<CoachController> _logger;

    private readonly InMemoryCoachRepository _repository;

    public CoachController(ILogger<CoachController> logger, InMemoryCoachRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpPost("/coaches")]
    public ActionResult<Coach> AddCoach([FromBody] Coach coachrequest)
    {
        //var mail = EmailAddress.From(coachrequest.Email);
        var coach = new Coach(coachrequest.Name, coachrequest.Email.ToString());
        _repository.SaveCoach(coach);
        return Ok();
    }


    [HttpGet("{name}")]
    public ActionResult<Coach> GetByName(string name)
    {
        var coach = _repository.GetByName(name);
        return coach is null ? NotFound() : Ok(coach);
    }

    [HttpPost("/coaches/{name}/skills")]
    public ActionResult ModifySkills(string name, [FromBody] ModifySkillDTO request)
    {
        var coach = _repository.GetByName(name);
        if (coach == null)
        {
            return NotFound($"Coach with name '{name}' not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Skill))
        {
            return BadRequest("Skill must be provided.");
        }

        switch (request.Action.ToLower())
        {
            case "add":
                coach.AddCompetence(request.Skill);
                break;
            case "remove":
                coach.RemoveCompetence(request.Skill);
                break;
            default:
                return BadRequest("invalid action: needs to be 'add' or 'remove'");
        }
        _repository.SaveCoach(coach);
        return Ok();
    }
}