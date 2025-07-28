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
        return course is null ? NotFound() : Ok(new CourseDTO(course.CourseName, course.StartDate, course.EndDate, course.RequiredCompetencies, course.Planning));
    }

    [HttpGet("/courses")]
    public ActionResult<List<Course>> GetAll()
    {
        if (_repository.Courses.Count == 0) { return NotFound(); }

        List<CourseDTO> result = new();
        foreach (var course in _repository.Courses)
        {
            result.Add(new CourseDTO(course.CourseName, course.StartDate, course.EndDate, course.RequiredCompetencies, course.Planning));
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

        course.AdjustRequirements(request.SkillsToAdd, request.SkillsToRemove);

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

        course.AdjustCourseMoment(request.TimeSlotsToAdd, request.TimeSlotsToRemove);

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