using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("/courses")]
public class CourseController : ControllerBase
{

    private readonly ILogger<CourseController> _logger;
    private readonly InMemoryCourseRepository _repository;

    private readonly InMemoryCoachRepository _coaches;

    public CourseController(ILogger<CourseController> logger, InMemoryCourseRepository repository, InMemoryCoachRepository coaches)
    {
        _logger = logger;
        _repository = repository;
        _coaches = coaches;
    }

    [HttpGet("/courses/{id}")]
    public ActionResult<Course> GetById(Guid id)
    {
        var course = _repository.GetById(id);
        return course is null ? NotFound() : Ok(course);
    }

    [HttpGet("/courses")]
    public ActionResult<List<Course>> GetAll()
    {
        if (_repository.Courses.Count == 0) { return NotFound(); }

        List<Course> result = new();
        foreach (var course in _repository.Courses)
        {
            result.Add(course);
        }

        return Ok(result);
    }

    [HttpPost("/courses")]
    public ActionResult<Course> AddCourse([FromBody] CourseDTO courserequest)
    {
        //var mail = EmailAddress.From(coachrequest.Email);
        var course = new Course(courserequest.Name, courserequest.Start, courserequest.End);
        _repository.SaveCourse(course);
        return Ok(course.Id);
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

        foreach (var slot in request.TimeSlotsToRemove.Distinct())
        {
            var mapslot = new Timeslot(slot.Day, slot.Start, slot.End);
            course.RemoveCourseMoment(mapslot);
        }
        foreach (var slot in request.TimeSlotsToAdd.Distinct())
        {
            var mapslot = new Timeslot(slot.Day, slot.Start, slot.End);
            course.AddCourseMoment(mapslot);
        }

        _repository.SaveCourse(course);
        return Ok();
    }


    [HttpPost("/courses/{id}/confirm")]
    public ActionResult ConfirmCourse(Guid id)
    {
        var course = _repository.GetById(id);
        if (course == null)
        {
            return NotFound($"Course with id '{id}' not found.");
        }

        course.ConfirmCourse();
        return Ok();
    }

    [HttpPost("/courses/{CourseId}/assign-coach")]
    public ActionResult AssignCoach(Guid CourseId, Guid CoachId)
    {
        var course = _repository.GetById(CourseId);
        if (course == null)
        {
            return NotFound($"Course with id '{CourseId}' not found.");
        }

        var coach = _coaches.GetById(CoachId);
        if (coach == null)
        {
            return NotFound($"Coach with id '{CoachId}' not found.");
        }

        course.AddCoach(coach);
        return Ok();
    }
}