using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.Service;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("/coaches")]
public class CoachController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IEFCoachRepository _repository;
    private readonly ICoachService _coachService;

    public CoachController(IEFCoachRepository repository, IUnitOfWork uow, ICoachService coachService)
    {
        _repository = repository;
        _uow = uow;
        _coachService = coachService;
    }

    [HttpGet("/coaches/{id}")]
    public async Task<IResult> GetById(int id)
    {
        var coach = await _coachService.GetById(id);
        return coach is null ? Results.NotFound() : Results.Ok(coach);
    }

    [HttpGet("/coaches")]
    public async Task<IResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int size = 5,
        CancellationToken ct = default
    )
    {
        var list = await _coachService.GetAll(page, size, ct);

        return Results.Ok(list);
    }

    [HttpPost("/coaches")]
    public async Task<ActionResult<int>> AddCoach([FromBody] CoachRequest coachrequest)
    {
        var coach = await _coachService.AddCoach(coachrequest.Name, coachrequest.Email);
        return Ok(coach.Id);
    }

    [HttpPost("/coaches/{id}/skills")]
    public async Task<ActionResult> ModifySkills([FromBody] List<string> newskills, int id)
    {
        var result = await _coachService.ModifySkills(newskills, id);
        if (!result)
        {
            return NotFound($"Coach with id '{id}' not found.");
        }
        return Ok();
    }
}