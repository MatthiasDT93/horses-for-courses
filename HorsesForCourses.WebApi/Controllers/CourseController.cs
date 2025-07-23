using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("courses")]
public class CourseController : ControllerBase
{

    private readonly ILogger<CourseController> _logger;

    public CourseController(ILogger<CourseController> logger)
    {
        _logger = logger;
    }



}