using HorsesForCourses.Core;

namespace HorsesForCourses.Service;

public interface IEFAccountRepository
{
    Task<AppUser> AddUserToDB(AppUser user);
}

public class EFAccountRepository : IEFAccountRepository
{
    private readonly IUnitOfWork _uow;
    private readonly AppDbContext _context;

    public EFAccountRepository(AppDbContext context, IUnitOfWork uow)
    {
        _uow = uow;
        _context = context;
    }

    public async Task<AppUser> AddUserToDB(AppUser user)
    {
        await _context.Users.AddAsync(user);
        return user;
    }
}