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

    [HttpGet("/coaches/{id}")]
    public ActionResult<Coach> GetById(Guid id)
    {
        var coach = _repository.GetById(id);
        return coach is null ? NotFound() : Ok(coach);
    }

    [HttpPost("/coaches")]
    public ActionResult<Guid> AddCoach([FromBody] CoachDTO coachrequest)
    {
        //var mail = EmailAddress.From(coachrequest.Email);
        var coach = new Coach(coachrequest.Name, coachrequest.Email);
        _repository.SaveCoach(coach);
        return Ok(coach.Id);
    }

    [HttpPost("/coaches/{id}/skills")]
    public ActionResult ModifySkills([FromBody] ModifyCoachSkillsDTO request, Guid id)
    {
        var coach = _repository.GetById(id);
        if (coach == null)
        {
            return NotFound($"Coach with id '{id}' not found.");
        }

        if (request.SkillsToAdd.Count == 0 && request.SkillsToRemove.Count == 0)
        {
            return BadRequest("A minimum of one skill to either add or remove must be given.");
        }

        foreach (var skill in request.SkillsToAdd.Distinct())
        {
            coach.AddCompetence(skill);
        }
        foreach (var skill in request.SkillsToRemove.Distinct())
        {
            coach.RemoveCompetence(skill);
        }

        _repository.SaveCoach(coach);
        return Ok();
    }
}