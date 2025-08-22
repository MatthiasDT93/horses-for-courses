using System.Linq;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;

public interface IEFCoachRepository
{
    Task AddCoachToDB(Coach coach);
    Task<List<Coach>> GetAllIncludingCourses();
    Task<Coach?> GetByIdIncludingCourses(int id);
    Task<CoachResponse?> GetDTOByIdIncludingCourses(int id);
    Task<PagedResult<CoachListResponse>> GetAllDTOIncludingCourses(int page, int size, CancellationToken ct);
    Task<bool> IsPopulated();
    Task Save();
}

public class EFCoachRepository : IEFCoachRepository
{
    private readonly IUnitOfWork _uow;
    private readonly AppDbContext _context;

    public EFCoachRepository(AppDbContext context, IUnitOfWork uow)
    {
        _uow = uow;
        _context = context;
    }

    public async Task<Coach?> GetByIdIncludingCourses(int id)
    {
        var result = await _context.Coaches
                            .Include(c => c.Courses)
                            .FirstOrDefaultAsync(c => c.Id == id);
        return result;
    }

    public async Task<CoachResponse?> GetDTOByIdIncludingCourses(int id)
    {
        return await _context.Coaches
                                .AsNoTracking()
                                .OrderBy(c => c.Id)
                                .Where(c => c.Id == id)
                                .Select(c => new CoachResponse(
                                    c.Id,
                                    c.Name,
                                    c.Email.Value,
                                    c.competencies,
                                    c.Courses
                                        .Select(cd => new CoachReponseCourseDTO(cd.Id, cd.CourseName))
                                    )
                                ).FirstOrDefaultAsync();
    }

    public async Task<List<Coach>> GetAllIncludingCourses()
    {
        return await _context.Coaches
                            .Include(c => c.Courses)
                            .ToListAsync();
    }

    public async Task<PagedResult<CoachListResponse>> GetAllDTOIncludingCourses(int page, int size, CancellationToken ct)
    {
        var request = new PageRequest(page, size);
        return await _context.Coaches
                                .AsNoTracking()
                                .OrderBy(c => c.Id)
                                .Select(c => new CoachListResponse(
                                    c.Id,
                                    c.Name,
                                    c.Email.Value,
                                    c.Courses.Count
                                    )
                                ).ToPagedResultAsync(request, ct);
    }

    public async Task<bool> IsPopulated()
    {
        return await _context.Coaches.AnyAsync();
    }

    public async Task AddCoachToDB(Coach coach)
    {
        await _context.Coaches.AddAsync(coach);
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}