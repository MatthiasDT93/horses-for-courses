
using System.Drawing;
using HorsesForCourses.Core;
using HorsesForCourses.WebApi;

namespace HorsesForCourses.Service;

public interface ICoachService
{
    Task<CoachResponse> GetById(int id);
    Task<PagedResult<CoachListResponse>> GetAll(int page, int size, CancellationToken ct);
    Task<Coach> AddCoach(string name, string email);
    Task<bool> ModifySkills(List<string> newskills, int id);


}


public class CoachService : ICoachService
{
    private readonly IUnitOfWork _uow;
    private readonly IEFCoachRepository _repository;

    public CoachService(IEFCoachRepository repository, IUnitOfWork uow)
    {
        _repository = repository;
        _uow = uow;
    }

    public async Task<CoachResponse> GetById(int id)
    {
        var coach = await _repository.GetDTOByIdIncludingCourses(id);
        return coach;
    }

    public async Task<PagedResult<CoachListResponse>> GetAll(int page, int size, CancellationToken ct)
    {
        var request = new PageRequest(page, size);
        var list = await _repository.GetAllDTOIncludingCourses(request.PageNumber, request.PageSize, ct);
        return list;
    }

    public async Task<Coach> AddCoach(string name, string email)
    {
        var coach = new Coach(name, email);
        await _repository.AddCoachToDB(coach);
        await _uow.SaveChangesAsync();

        return coach;
    }

    public async Task<bool> ModifySkills(List<string> newskills, int id)
    {
        var coach = await _repository.GetByIdIncludingCourses(id);
        if (coach != null)
        {
            coach.OverWriteCompetences(newskills);
            await _uow.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
