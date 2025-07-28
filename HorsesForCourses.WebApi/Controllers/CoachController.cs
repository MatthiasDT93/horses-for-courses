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
        return coach is null ? NotFound() : Ok(new CoachDTO(coach.Name, coach.Email.Value.ToString(), coach.competencies.ToList(), coach.bookings.ToList()));
    }

    [HttpGet("/coaches")]
    public ActionResult<List<Coach>> GetAll()
    {
        if (_repository.Coaches.Count == 0) { return NotFound(); }

        List<CoachDTO> result = new();
        foreach (var coach in _repository.Coaches)
        {
            var coachdto = new CoachDTO(coach.Name, coach.Email.ToString(), coach.competencies.ToList(), coach.bookings.ToList());
            result.Add(coachdto);
        }

        return Ok(result);
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

        coach.AdjustCompetences(request.SkillsToAdd, request.SkillsToRemove);

        _repository.SaveCoach(coach);
        return Ok();
    }
}