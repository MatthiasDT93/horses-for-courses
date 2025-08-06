using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;



public class EFCoachRepository
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

    public async Task<List<Coach>> GetAllIncludingCourses()
    {
        return await _context.Coaches
                            .Include(c => c.Courses)
                            .ToListAsync();
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