using HorsesForCourses.Core;



public class InMemoryCoachRepository
{
    private int nextid = 1;
    private readonly List<Coach> _coaches = new();

    public IReadOnlyList<Coach> Coaches => _coaches;

    public void SaveCoach(Coach coach)
    {
        int index = _coaches.FindIndex(c => c.Id == coach.Id);
        if (index >= 0)
        {
            _coaches[index] = coach;
        }
        else
        {
            coach.AssignId(nextid);
            _coaches.Add(coach);
            nextid++;
        }
    }

    public Coach? GetById(int id)
    {
        return _coaches.FirstOrDefault(c => c.Id == id);
    }

    public int GenerateNewId()
    {
        return nextid++;
    }
}