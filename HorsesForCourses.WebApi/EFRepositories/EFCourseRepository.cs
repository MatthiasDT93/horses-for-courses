using System.Threading.Tasks;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;

public interface IEFCourseRepository
{
    Task AddCourseToDB(Course course);
    Task<List<Course>> GetAllIncludingCoach();
    Task<Course?> GetByIdIncludingCoach(int id);
    Task<CourseResponse?> GetDTOByIdIncludingCoach(int id);
    Task<PagedResult<CourseListResponse>> GetAllDTOIncludingCoach(int page, int size, CancellationToken ct);
    Task<bool> IsPopulated();
    Task Save();
}

public class EFCourseRepository : IEFCourseRepository
{
    private readonly IUnitOfWork _uow;
    private readonly AppDbContext _context;

    public EFCourseRepository(AppDbContext context, IUnitOfWork uow)
    {
        _context = context;
        _uow = uow;
    }

    public async Task<Course?> GetByIdIncludingCoach(int id)
    {
        var result = await _context.Courses
                            .Include(c => c.coach)
                            .FirstOrDefaultAsync(c => c.Id == id);
        return result;
    }

    public async Task<CourseResponse?> GetDTOByIdIncludingCoach(int id)
    {
        return await _context.Courses
                                .AsNoTracking()
                                .Where(c => c.Id == id)
                                .Select(c => new CourseResponse(
                                    c.Id,
                                    c.CourseName,
                                    c.StartDate,
                                    c.EndDate,
                                    c.RequiredCompetencies,
                                    c.Planning,
                                    c.coach!
                                )).FirstOrDefaultAsync();
    }

    public async Task<List<Course>> GetAllIncludingCoach()
    {
        return await _context.Courses
                            .Include(c => c.coach)
                            .ToListAsync();
    }

    public async Task<PagedResult<CourseListResponse>> GetAllDTOIncludingCoach(int page, int size, CancellationToken ct)
    {
        var request = new PageRequest(page, size);
        return await _context.Courses
                                .AsNoTracking()
                                .OrderBy(c => c.Id)
                                .Select(c => new CourseListResponse(
                                    c.Id,
                                    c.CourseName,
                                    c.StartDate,
                                    c.EndDate,
                                    c.Planning.Count != 0,
                                    c.coach != null
                                )).ToPagedResultAsync(request, ct);
    }

    public async Task<bool> IsPopulated()
    {
        return await _context.Courses.AnyAsync();
    }

    public async Task AddCourseToDB(Course course)
    {
        await _context.Courses.AddAsync(course);
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }



}