using HorsesForCourses.Core;

public class InMemoryCoachRepository
{
    private readonly List<Coach> _coaches = new();

    public void SaveCoach(Coach coach)
    {
        int index = _coaches.FindIndex(c => c.Id == coach.Id);
        if (index >= 0)
        {
            _coaches[index] = coach;
        }
        else
        {
            _coaches.Add(coach);
        }
    }

    public Coach? GetById(Guid id)
    {
        return _coaches.FirstOrDefault(c => c.Id == id);
    }
}