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
    Task<List<CoachListResponse>> GetAllDTOIncludingCourses();
    Task<bool> IsPopulated();
    Task Save();
}

public class EFCoachRepository : IEFCoachRepository
{
    private readonly AppDbContext _context;

    public EFCoachRepository(AppDbContext context)
    {
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

    public async Task<List<CoachListResponse>> GetAllDTOIncludingCourses()
    {
        return await _context.Coaches
                                .AsNoTracking()
                                .Select(c => new CoachListResponse(
                                    c.Id,
                                    c.Name,
                                    c.Email.Value,
                                    c.Courses.Count
                                    )
                                ).ToListAsync();
    }

    public async Task<bool> IsPopulated()
    {
        return await _context.Coaches.AnyAsync();
    }

    public async Task AddCoachToDB(Coach coach)
    {
        var lastId = await _context.Coaches
                            .OrderByDescending(x => x.Id)
                            .Select(x => x.Id)
                            .FirstOrDefaultAsync();
        coach.AssignId(lastId + 1);
        _context.Coaches.Add(coach);
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }
}