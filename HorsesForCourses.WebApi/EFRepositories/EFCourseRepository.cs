using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;

public class EFCourseRepository
{
    private readonly AppDbContext _context;

    public EFCourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Course?> GetByIdIncludingCoach(int id)
    {
        var result = await _context.Courses
                            .Include(c => c.coach)
                            .FirstOrDefaultAsync(c => c.Id == id);
        return result;
    }

    public async Task<List<Course>> GetAllIncludingCoach()
    {
        return await _context.Courses
                            .Include(c => c.coach)
                            .ToListAsync();
    }

    public async Task<bool> IsPopulated()
    {
        return await _context.Courses.AnyAsync();
    }

    public async Task AddCourseToDB(Course course)
    {
        var lastId = await _context.Courses
                            .OrderByDescending(x => x.Id)
                            .Select(x => x.Id)
                            .FirstOrDefaultAsync();
        course.AssignId(lastId + 1);
        _context.Courses.Add(course);
    }

    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }



}