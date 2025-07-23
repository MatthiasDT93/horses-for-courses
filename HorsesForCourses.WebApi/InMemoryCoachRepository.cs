using HorsesForCourses.Core;

public class InMemoryCoachRepository
{
    private readonly List<Coach> _coaches = new();

    public void SaveCoach(Coach coach)
    {
        int index = _coaches.FindIndex(c => c.Name == coach.Name && c.Email == coach.Email);
        if (index >= 0)
        {
            _coaches[index] = coach;
        }
        else
        {
            _coaches.Add(coach);
        }
    }

    public Coach? GetByName(string name)
    {
        return _coaches.FirstOrDefault(c => c.Name == name);
    }
}