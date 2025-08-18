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
        var coach = await _repository.GetDTOByIdIncludingCourses(id);
        return coach is null ? NotFound() : Ok(coach);
    }

    [HttpGet("/coaches")]
    public async Task<ActionResult<List<CoachListResponse>>> GetAll()
    {
        if (!await _repository.IsPopulated()) { return NotFound(); }
        var list = await _repository.GetAllDTOIncludingCourses();

        return Ok(list);
    }

    [HttpPost("/coaches")]
    public async Task<ActionResult<int>> AddCoach([FromBody] CoachRequest coachrequest)
    {
        var coach = new Coach(coachrequest.Name, coachrequest.Email);
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