

using HorsesForCourses.WebApi.Controllers;

namespace HorsesForCourses.WebApi;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
}

public class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public EfUnitOfWork(AppDbContext context)
    {
        _context = context;

    }

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}