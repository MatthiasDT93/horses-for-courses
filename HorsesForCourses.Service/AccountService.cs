using System.Threading.Tasks;
using HorsesForCourses.Core;
using System.Text.Json;


namespace HorsesForCourses.Service;

public interface IAccountService
{
    Task<AppUser> AddUser(AppUser user);
    Task<AppUser?> GetUser(string email);
    Task RemoveUser(AppUser user);
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


    public async Task<AppUser?> GetUser(string email)
    {
        var result = await _repository.GetUser(email);
        if (result is null) throw new Exception("User does not exist.");
        return result;
    }

    public async Task RemoveUser(AppUser user)
    {
        _repository.RemoveUser(user);
        await _uow.SaveChangesAsync();
    }
}