using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("courses")]
public class CourseController : ControllerBase
{

    private readonly ILogger<CourseController> _logger;
    private readonly InMemoryCourseRepository _repository;

    public CourseController(ILogger<CourseController> logger, InMemoryCourseRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    [HttpGet("/courses/{id}")]
    public ActionResult<Course> GetById(Guid id)
    {
        var course = _repository.GetById(id);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpPost("/courses")]
    public ActionResult<Course> AddCourse([FromBody] CourseDTO courserequest)
    {
        //var mail = EmailAddress.From(coachrequest.Email);
        var course = new Course(courserequest.Name, courserequest.Start, courserequest.End);
        _repository.SaveCourse(course);
        return Ok(course);
    }

    [HttpPost("/courses/{id}/skills")]
    public ActionResult ModifySkills([FromBody] ModifyCourseSkillsDTO request, Guid id)
    {
        var course = _repository.GetById(id);
        if (course == null)
        {
            return NotFound($"Course with id '{id}' not found.");
        }

        if (request.SkillsToAdd.Count == 0 && request.SkillsToRemove.Count == 0)
        {
            return BadRequest("A minimum of one skill to either add or remove must be given.");
        }

        foreach (var skill in request.SkillsToAdd.Distinct())
        {
            course.AddRequirement(skill);
        }
        foreach (var skill in request.SkillsToRemove)
        {
            course.RemoveRequirement(skill);
        }

        _repository.SaveCourse(course);
        return Ok();
    }


    [HttpPost("/courses/{id}/timeslots")]
    public ActionResult ModifyTimeSlots([FromBody] ModifyTimeSlotsDTO request, Guid id)
    {
        var course = _repository.GetById(id);
        if (course == null)
        {
            return NotFound($"Course with id '{id}' not found.");
        }


        foreach (var slot in request.TimeSlotsToAdd.Distinct())
        {
            course.AddCourseMoment(slot);
        }
        foreach (var slot in request.TimeSlotsToRemove.Distinct())
        {
            course.RemoveCourseMoment(slot);
        }

        _repository.SaveCourse(course);
        return Ok();
    }
}