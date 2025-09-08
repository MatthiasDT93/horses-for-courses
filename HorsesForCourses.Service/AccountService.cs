using HorsesForCourses.Core;

namespace HorsesForCourses.Service;

public interface IAccountService
{
    Task<AppUser> AddUser(AppUser user);
}


public class AccountService : IAccountService
{

    private readonly IUnitOfWork _uow;
    private readonly IEFAccountRepository _repository;

    public AccountService(IEFAccountRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task<AppUser> AddUser(AppUser user)
    {
        var newuser = await _repository.AddUserToDB(user);
        await _uow.SaveChangesAsync();
        return newuser;
    }
}