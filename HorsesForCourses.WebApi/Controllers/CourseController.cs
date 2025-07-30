using HorsesForCourses.Core;
using Microsoft.AspNetCore.Mvc;

namespace HorsesForCourses.WebApi.Controllers;

[ApiController]
[Route("/courses")]
public class CourseController : ControllerBase
{

    private readonly InMemoryCourseRepository _repository;

    private readonly InMemoryCoachRepository _coaches;

    public CourseController(InMemoryCourseRepository repository, InMemoryCoachRepository coaches)
    {
        _repository = repository;
        _coaches = coaches;
    }

    [HttpGet("/courses/{id}")]
    public ActionResult<CourseResponse> GetById(int id)
    {
        var course = _repository.GetById(id);
        return course is null ? NotFound() : Ok(new CourseResponse(course.Id, course.CourseName, course.StartDate, course.EndDate, course.RequiredCompetencies, course.Planning, course.coach));
    }

    [HttpGet("/courses")]
    public ActionResult<List<CourseListResponse>> GetAll()
    {
        if (_repository.Courses.Count == 0) { return NotFound(); }

        var result = CourseListResponse.ExtractResponse(_repository.Courses);

        return Ok(result);
    }

    [HttpPost("/courses")]
    public ActionResult<Guid> AddCourse([FromBody] CourseRequest courserequest)
    {
        var dto = CourseRequest.Request_To_DTO(courserequest, _repository.GenerateNewId());
        var course = CourseDTOMapping.DTO_To_Course(dto);
        _repository.SaveCourse(course);
        return Ok(course.Id);
    }

    [HttpPost("/courses/{id}/skills")]
    public ActionResult ModifySkills([FromBody] List<string> newreqs, int id)
    {
        var course = _repository.GetById(id);
        if (course == null)
        {
            return NotFound($"Course with id '{id}' not found.");
        }

        course.OverWriteRequirements(newreqs);

        _repository.SaveCourse(course);
        return Ok();
    }


    [HttpPost("/courses/{id}/timeslots")]
    public ActionResult ModifyTimeSlots([FromBody] List<TimeSlotDTO> newslots, int id)
    {
        var course = _repository.GetById(id);
        if (course == null)
        {
            return NotFound($"Course with id '{id}' not found.");
        }

        var newnewslots = TimeslotDTOMapping.DTOList_To_TimeslotList(newslots);
        course.OverWriteCourseMoment(newnewslots);

        _repository.SaveCourse(course);
        return Ok();
    }


    [HttpPost("/courses/{id}/confirm")]
    public ActionResult ConfirmCourse(int id)
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
    public ActionResult AssignCoach(int CourseId, int CoachId)
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