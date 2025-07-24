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

    [HttpPost("/courses")]
    public ActionResult<Course> AddCoach([FromBody] CourseDTO courserequest)
    {
        //var mail = EmailAddress.From(coachrequest.Email);
        var course = new Course(courserequest.Name, courserequest.Start, courserequest.End);
        _repository.SaveCourse(course);
        return Ok();
    }

}