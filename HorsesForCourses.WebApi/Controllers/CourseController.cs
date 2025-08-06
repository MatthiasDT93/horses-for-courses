using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("/courses")]
public class CourseController : ControllerBase
{

    private readonly EFCourseRepository _repository;

    private readonly EFCoachRepository _coaches;

    private readonly AppDbContext _context;

    public CourseController(EFCourseRepository repository, EFCoachRepository coaches, AppDbContext context)
    {
        _repository = repository;
        _coaches = coaches;
        _context = context;
    }

    [HttpGet("/courses/{id}")]
    public async Task<ActionResult<CourseResponse>> GetById(int id)
    {
        var course = await _repository.GetByIdIncludingCoach(id);
        return course is null ? NotFound() : Ok(new CourseResponse(course.Id, course.CourseName, course.StartDate, course.EndDate, course.RequiredCompetencies, course.Planning, course.coach));
    }

    [HttpGet("/courses")]
    public async Task<ActionResult<List<CourseListResponse>>> GetAll()
    {
        if (!await _repository.IsPopulated()) { return NotFound(); }
        var list = await _repository.GetAllIncludingCoach();
        var result = CourseListResponse.ExtractResponse(list);

        return Ok(result);
    }

    [HttpPost("/courses")]
    public async Task<ActionResult<int>> AddCourse([FromBody] CourseRequest courserequest)
    {
        var dto = CourseRequest.Request_To_DTO(courserequest);
        var course = CourseDTOMapping.DTO_To_Course(dto);
        await _repository.AddCourseToDB(course);
        await _repository.Save();
        return Ok(course.Id);
    }

    [HttpPost("/courses/{id}/skills")]
    public async Task<ActionResult> ModifySkills([FromBody] List<string> newreqs, int id)
    {
        var course = await _repository.GetByIdIncludingCoach(id);
        if (course == null)
        {
            return NotFound($"Course with id '{id}' not found.");
        }

        course.OverWriteRequirements(newreqs);

        await _repository.Save();
        return Ok();
    }


    [HttpPost("/courses/{id}/timeslots")]
    public async Task<ActionResult> ModifyTimeSlots([FromBody] List<TimeSlotDTO> newslots, int id)
    {
        var course = await _repository.GetByIdIncludingCoach(id);
        if (course == null)
        {
            return NotFound($"Course with id '{id}' not found.");
        }

        var newnewslots = TimeslotDTOMapping.DTOList_To_TimeslotList(newslots);
        course.OverWriteCourseMoment(newnewslots);

        await _repository.Save();
        return Ok();
    }


    [HttpPost("/courses/{id}/confirm")]
    public async Task<ActionResult> ConfirmCourse(int id)
    {
        var course = await _repository.GetByIdIncludingCoach(id);
        if (course == null)
        {
            return NotFound($"Course with id '{id}' not found.");
        }

        course.ConfirmCourse();

        await _repository.Save();
        return Ok();
    }

    [HttpPost("/courses/{CourseId}/assign-coach")]
    public async Task<ActionResult> AssignCoach(int CourseId, int CoachId)
    {
        var course = await _repository.GetByIdIncludingCoach(CourseId);
        if (course == null)
        {
            return NotFound($"Course with id '{CourseId}' not found.");
        }

        var coach = await _coaches.GetByIdIncludingCourses(CoachId);
        if (coach == null)
        {
            return NotFound($"Coach with id '{CoachId}' not found.");
        }

        course.AddCoach(coach);

        await _repository.Save();
        return Ok();
    }
}