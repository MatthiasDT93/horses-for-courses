using System.Diagnostics.Contracts;

namespace HorsesForCourses.Core;

public class Coach
{
    private List<string> Competencies = new();

    public IReadOnlyList<string> competencies => Competencies;

    private List<Timeslot> Bookings = new();

    public IReadOnlyList<Timeslot> bookings => Bookings;

    public string Name { get; }

    public EmailAddress Email { get; }


    public Coach(string name, string mail)
    {
        Name = name;
        Email = EmailAddress.From(mail);
    }

    public void AddCompetence(string comp)
    {
        if (!Competencies.Contains(comp))
        {
            Competencies.Add(comp);
        }
        else
        {
            throw new Exception($"Coach {Name} already has this competence.");
        }
    }

    public void RemoveCompetence(string comp)
    {
        if (!Competencies.Remove(comp))
        {
            throw new Exception($"Coach {Name} does not have this competence.");
        }
    }

    public void BookIn(Timeslot slot)
    {
        var busy = Bookings.Any(t => slot.Overlap(t));
        if (!busy)
        {
            Bookings.Add(slot);
        }
        else
        {
            throw new ArgumentException($"Coach {Name} is busy around this time.");
        }
    }

    public bool IsCompetent(List<string> requirements)
    {
        var competent = Competencies.All(c => requirements.Contains(c));

        return competent;
    }
}
