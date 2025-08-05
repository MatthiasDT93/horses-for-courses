using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("/coaches")]
public class CoachController : ControllerBase
{

    private readonly InMemoryCoachRepository _repository;
    private readonly AppDbContext _context;

    public CoachController(InMemoryCoachRepository repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    [HttpGet("/coaches/{id}")]
    public async Task<ActionResult<CoachResponse>> GetById(int id)
    {
        var coach = await _context.Coaches.FirstOrDefaultAsync(c => c.Id == id);
        return coach is null ? NotFound() : Ok(new CoachResponse(coach.Id, coach.Name, coach.Email.Value, coach.competencies, coach.Courses));
    }

    [HttpGet("/coaches")]
    public async Task<ActionResult<List<CoachListResponse>>> GetAll()
    {
        if (!await _context.Coaches.AnyAsync()) { return NotFound(); }
        var list = await _context.Coaches.ToListAsync();
        var result = CoachListResponse.ExtractResponse(list);

        return Ok(result);
    }

    [HttpPost("/coaches")]
    public async Task<ActionResult<int>> AddCoach([FromBody] CoachRequest coachrequest)
    {
        var dto = CoachRequest.Request_To_DTO(coachrequest);
        var coach = CoachDTOMapping.DTO_To_Coach(dto);
        var lastId = await _context.Coaches
                            .OrderByDescending(x => x.Id)
                            .Select(x => x.Id)
                            .FirstOrDefaultAsync();
        coach.AssignId(lastId + 1);
        _context.Coaches.Add(coach);
        await _context.SaveChangesAsync();
        return Ok(coach.Id);
    }

    [HttpPost("/coaches/{id}/skills")]
    public async Task<ActionResult> ModifySkills([FromBody] List<string> newskills, int id)
    {
        var coach = await _context.Coaches.FirstOrDefaultAsync(c => c.Id == id);
        if (coach == null)
        {
            return NotFound($"Coach with id '{id}' not found.");
        }

        coach.OverWriteCompetences(newskills);

        await _context.SaveChangesAsync();
        return Ok();
    }
}