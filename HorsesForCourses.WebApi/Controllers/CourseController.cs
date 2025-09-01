using HorsesForCourses.Core;
using HorsesForCourses.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("/courses")]
public class CourseController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IEFCourseRepository _repository;

    private readonly IEFCoachRepository _coaches;

    private readonly ICourseService _courseService;


    public CourseController(IEFCourseRepository repository, IEFCoachRepository coaches, IUnitOfWork uow, ICourseService courseService)
    {
        _repository = repository;
        _coaches = coaches;
        _uow = uow;
        _courseService = courseService;
    }

    [HttpGet("/courses/{id}")]
    public async Task<ActionResult<CourseResponse>> GetById(int id)
    {
        var course = await _courseService.GetById(id);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpGet("/courses")]
    public async Task<ActionResult<List<CourseListResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int size = 5,
        CancellationToken ct = default
    )
    {
        var list = await _courseService.GetAll(page, size, ct);

        return Ok(list);
    }

    [HttpPost("/courses")]
    public async Task<ActionResult<int>> AddCourse([FromBody] CourseRequest courserequest)
    {
        var course = await _courseService.AddCourse(courserequest.Name, courserequest.Start, courserequest.End);
        return Ok(course.Id);
    }

    [HttpPost("/courses/{id}/skills")]
    public async Task<ActionResult> ModifySkills([FromBody] List<string> newreqs, int id)
    {
        var result = await _courseService.ModifySkills(newreqs, id);
        if (!result)
        {
            return NotFound($"Course with id '{id}' not found.");
        }
        return Ok();
    }


    [HttpPost("/courses/{id}/timeslots")]
    public async Task<ActionResult> ModifyTimeSlots([FromBody] List<TimeSlotDTO> newslots, int id)
    {
        var result = await _courseService.ModifyTimeSlots(newslots, id);
        if (!result)
        {
            return NotFound($"Course with id '{id}' not found.");
        }
        return Ok();
    }


    [HttpPost("/courses/{id}/confirm")]
    public async Task<ActionResult> ConfirmCourse(int id)
    {
        var result = await _courseService.ConfirmCourse(id);
        if (!result)
        {
            return NotFound($"Course with id '{id}' not found.");
        }
        return Ok();
    }

    [HttpPost("/courses/{CourseId}/assign-coach")]
    public async Task<ActionResult> AssignCoach(int CourseId, int CoachId)
    {
        var result = await _courseService.AssignCoach(CourseId, CoachId);
        if (!result.Item1)
        {
            return NotFound($"Course with id '{CourseId}' not found.");
        }
        if (!result.Item2)
        {
            return NotFound($"Coach with id '{CoachId}' not found.");
        }
        return Ok();
    }
}