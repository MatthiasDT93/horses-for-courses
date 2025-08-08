

using HorsesForCourses.WebApi.Controllers;

namespace HorsesForCourses.WebApi;

public interface IUnitOfWork : IDisposable
{
    IEFCoachRepository Coachrepo { get; }
    IEFCourseRepository Courserepo { get; }
    Task<int> SaveChangesAsync();
}

public class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IEFCoachRepository Coachrepo { get; }
    public IEFCourseRepository Courserepo { get; }

    public EfUnitOfWork(AppDbContext context, IEFCoachRepository coachrepo, IEFCourseRepository courserepo)
    {
        _context = context;
        Coachrepo = coachrepo;
        Courserepo = courserepo;
    }

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}