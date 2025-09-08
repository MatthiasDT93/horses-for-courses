using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Service;

public interface IEFAccountRepository
{
    Task<AppUser> AddUserToDB(AppUser user);
    Task<AppUser?> GetUser(string email);
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

    public async Task<AppUser?> GetUser(string mail)
    {
        var result = await _context.Users
                                    .FirstOrDefaultAsync(u => u.Email.Value == mail);
        return result;
    }
}