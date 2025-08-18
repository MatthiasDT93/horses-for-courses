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
    public async Task<IResult> GetById(int id)
    {
        var coach = await _repository.GetDTOByIdIncludingCourses(id);
        return coach is null ? Results.NotFound() : Results.Ok(coach);
    }

    [HttpGet("/coaches")]
    public async Task<IResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int size = 5,
        CancellationToken ct = default
    )
    {
        var request = new PageRequest(page, size);
        if (!await _repository.IsPopulated()) { return Results.NotFound(); }
        var list = await _repository.GetAllDTOIncludingCourses(request.PageNumber, request.PageSize, ct);

        return Results.Ok(list);
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