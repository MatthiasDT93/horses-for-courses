using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("/coaches")]
public class CoachController : ControllerBase
{

    private readonly InMemoryCoachRepository _repository;

    public CoachController(InMemoryCoachRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("/coaches/{id}")]
    public ActionResult<CoachResponse> GetById(int id)
    {
        var coach = _repository.GetById(id);
        return coach is null ? NotFound() : Ok(new CoachResponse(coach.Id, coach.Name, coach.Email.Value, coach.competencies, coach.Courses));
    }

    [HttpGet("/coaches")]
    public ActionResult<List<CoachListResponse>> GetAll()
    {
        if (_repository.Coaches.Count == 0) { return NotFound(); }

        var result = CoachListResponse.ExtractResponse(_repository.Coaches);

        return Ok(result);
    }

    [HttpPost("/coaches")]
    public ActionResult<int> AddCoach([FromBody] CoachRequest coachrequest)
    {
        var dto = CoachRequest.Request_To_DTO(coachrequest, _repository.GenerateNewId());
        var coach = CoachDTOMapping.DTO_To_Coach(dto);
        _repository.SaveCoach(coach);
        return Ok(coach.Id);
    }

    [HttpPost("/coaches/{id}/skills")]
    public ActionResult ModifySkills([FromBody] List<string> newskills, int id)
    {
        var coach = _repository.GetById(id);
        if (coach == null)
        {
            return NotFound($"Coach with id '{id}' not found.");
        }

        coach.OverWriteCompetences(newskills);

        _repository.SaveCoach(coach);
        return Ok();
    }
}