
using System.Drawing;
using System.Dynamic;
using HorsesForCourses.Core;


namespace HorsesForCourses.Service;

public interface ICourseService
{
    Task<CourseResponse> GetById(int id);
    Task<PagedResult<CourseListResponse>> GetAll(int page, int size, CancellationToken ct);
    Task<Course> AddCourse(string name, DateOnly start, DateOnly end);
    Task<bool> ModifySkills(List<string> newskills, int id);
    Task<bool> ModifyTimeSlots(List<TimeSlotDTO> newslots, int id);
    Task<bool> ConfirmCourse(int id);
    Task<(bool, bool)> AssignCoach(int courseid, int coachid);

}


public class CourseService : ICourseService
{
    private readonly IUnitOfWork _uow;
    private readonly IEFCourseRepository _repository;

    private readonly IEFCoachRepository _coaches;

    public CourseService(IEFCourseRepository repository, IEFCoachRepository coaches, IUnitOfWork uow)
    {
        _repository = repository;
        _coaches = coaches;
        _uow = uow;
    }

    public async Task<CourseResponse> GetById(int id)
    {
        var course = await _repository.GetDTOByIdIncludingCoach(id);
        return course;
    }

    public async Task<PagedResult<CourseListResponse>> GetAll(int page, int size, CancellationToken ct)
    {
        var request = new PageRequest(page, size);
        var list = await _repository.GetAllDTOIncludingCoach(request.PageNumber, request.PageSize, ct);
        return list;
    }

    public async Task<Course> AddCourse(string name, DateOnly start, DateOnly end)
    {
        var course = new Course(name, start, end);
        await _repository.AddCourseToDB(course);
        await _uow.SaveChangesAsync();

        return course;
    }

    public async Task<bool> ModifySkills(List<string> newreqs, int id)
    {
        var course = await _repository.GetByIdIncludingCoach(id);
        if (course != null)
        {
            course.OverWriteRequirements(newreqs);
            await _uow.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> ModifyTimeSlots(List<TimeSlotDTO> newslots, int id)
    {
        var course = await _repository.GetByIdIncludingCoach(id);
        if (course != null)
        {
            var newnewslots = TimeslotDTOMapping.DTOList_To_TimeslotList(newslots);
            course.OverWriteCourseMoment(newnewslots);
            await _uow.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> ConfirmCourse(int id)
    {
        var course = await _repository.GetByIdIncludingCoach(id);
        if (course != null)
        {
            course.ConfirmCourse();
            await _uow.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<(bool, bool)> AssignCoach(int courseid, int coachid)
    {
        var course = await _repository.GetByIdIncludingCoach(courseid);

        var coach = await _coaches.GetByIdIncludingCourses(coachid);

        var result1 = course != null;
        var result2 = coach != null;

        if (result1 && result2)
        {
            course.AddCoach(coach);

            await _uow.SaveChangesAsync();
        }

        return (result1, result2);
    }

}
