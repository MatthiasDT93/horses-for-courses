using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("/coaches")]
public class CoachController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IEFCoachRepository _repository;

    public CoachController(IEFCoachRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    [HttpGet("/coaches/{id}")]
    public async Task<ActionResult<CoachResponse>> GetById(int id)
    {
        var coach = await _repository.GetByIdIncludingCourses(id);
        return coach is null ? NotFound() : Ok(new CoachResponse(coach.Id, coach.Name, coach.Email.Value, coach.competencies, coach.Courses));
    }

    [HttpGet("/coaches")]
    public async Task<ActionResult<List<CoachListResponse>>> GetAll()
    {
        if (!await _repository.IsPopulated()) { return NotFound(); }
        var list = await _repository.GetAllIncludingCourses();
        var result = CoachListResponse.ExtractResponse(list);

        return Ok(result);
    }

    [HttpPost("/coaches")]
    public async Task<ActionResult<int>> AddCoach([FromBody] CoachRequest coachrequest)
    {
        var dto = CoachRequest.Request_To_DTO(coachrequest);
        var coach = CoachDTOMapping.DTO_To_Coach(dto);
        await _repository.AddCoachToDB(coach);
        await _uow.SaveChangesAsync();
        return Ok(coach.Id);
    }

    [HttpPost("/coaches/{id}/skills")]
    public async Task<ActionResult> ModifySkills([FromBody] List<string> newskills, int id)
    {
        var coach = await _repository.GetByIdIncludingCourses(id);
        if (coach == null)
        {
            return NotFound($"Coach with id '{id}' not found.");
        }

        coach.OverWriteCompetences(newskills);

        await _uow.SaveChangesAsync();
        return Ok();
    }
}