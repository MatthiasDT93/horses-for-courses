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
    public ActionResult<Guid> AddCoach([FromBody] CoachDTO coachrequest)
    {
        //var mail = EmailAddress.From(coachrequest.Email);
        var coach = new Coach(coachrequest.Name, coachrequest.Email);
        _repository.SaveCoach(coach);
        return Ok(coach.Id);
    }


    [HttpGet("/coaches/{id}")]
    public ActionResult<Coach> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        return coach is null ? NotFound() : Ok(coach);
    }

    [HttpPost("/coaches/{id}/skills")]
    public ActionResult ModifySkills(Guid id, [FromBody] ModifySkillDTO request)
    {
        var coach = _repository.GetById(id);
        if (coach == null)
        {
            return NotFound($"Coach with id '{id}' not found.");
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