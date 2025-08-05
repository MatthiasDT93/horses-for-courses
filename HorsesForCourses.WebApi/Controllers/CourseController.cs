using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("/courses")]
public class CourseController : ControllerBase
{

    private readonly InMemoryCourseRepository _repository;

    private readonly InMemoryCoachRepository _coaches;

    private readonly AppDbContext _context;

    public CourseController(InMemoryCourseRepository repository, InMemoryCoachRepository coaches, AppDbContext context)
    {
        _repository = repository;
        _coaches = coaches;
        _context = context;
    }

    [HttpGet("/courses/{id}")]
    public async Task<ActionResult<CourseResponse>> GetById(int id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        return course is null ? NotFound() : Ok(new CourseResponse(course.Id, course.CourseName, course.StartDate, course.EndDate, course.RequiredCompetencies, course.Planning, course.coach));
    }

    [HttpGet("/courses")]
    public async Task<ActionResult<List<CourseListResponse>>> GetAll()
    {
        if (!await _context.Courses.AnyAsync()) { return NotFound(); }
        var list = await _context.Courses.ToListAsync();
        var result = CourseListResponse.ExtractResponse(list);

        return Ok(result);
    }

    [HttpPost("/courses")]
    public async Task<ActionResult<int>> AddCourse([FromBody] CourseRequest courserequest)
    {
        var dto = CourseRequest.Request_To_DTO(courserequest);
        var course = CourseDTOMapping.DTO_To_Course(dto);
        var lastId = await _context.Courses
                            .OrderByDescending(x => x.Id)
                            .Select(x => x.Id)
                            .FirstOrDefaultAsync();
        course.AssignId(lastId + 1);
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
        return Ok(course.Id);
    }

    [HttpPost("/courses/{id}/skills")]
    public async Task<ActionResult> ModifySkills([FromBody] List<string> newreqs, int id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course == null)
        {
            return NotFound($"Course with id '{id}' not found.");
        }

        course.OverWriteRequirements(newreqs);

        await _context.SaveChangesAsync();
        return Ok();
    }


    [HttpPost("/courses/{id}/timeslots")]
    public async Task<ActionResult> ModifyTimeSlots([FromBody] List<TimeSlotDTO> newslots, int id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course == null)
        {
            return NotFound($"Course with id '{id}' not found.");
        }

        var newnewslots = TimeslotDTOMapping.DTOList_To_TimeslotList(newslots);
        course.OverWriteCourseMoment(newnewslots);

        await _context.SaveChangesAsync();
        return Ok();
    }


    [HttpPost("/courses/{id}/confirm")]
    public async Task<ActionResult> ConfirmCourse(int id)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        if (course == null)
        {
            return NotFound($"Course with id '{id}' not found.");
        }

        course.ConfirmCourse();

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("/courses/{CourseId}/assign-coach")]
    public async Task<ActionResult> AssignCoach(int CourseId, int CoachId)
    {
        var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == CourseId);
        if (course == null)
        {
            return NotFound($"Course with id '{CourseId}' not found.");
        }

        var coach = await _context.Coaches.FirstOrDefaultAsync(c => c.Id == CoachId);
        if (coach == null)
        {
            return NotFound($"Coach with id '{CoachId}' not found.");
        }

        course.AddCoach(coach);

        await _context.SaveChangesAsync();
        return Ok();
    }
}