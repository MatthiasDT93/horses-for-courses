using System.Diagnostics.Contracts;

namespace HorsesForCourses.Core;

public class Coach
{
    public Guid Id { get; private set; }
    private List<string> Competencies = new();

    public IReadOnlyList<string> competencies => Competencies;

    private List<Booking> Bookings = new();

    public IReadOnlyList<Booking> bookings => Bookings;

    public string Name { get; }

    public EmailAddress Email { get; }


    public Coach(string name, string mail)
    {
        Name = name;
        Email = EmailAddress.From(mail);
        Id = Guid.NewGuid();
    }

    public void AddCompetence(string comp)
    {
        if (!Competencies.Contains(comp))
        {
            Competencies.Add(comp);
        }
        else
            throw new Exception($"Coach {Name} already has this competence.");
    }

    public void RemoveCompetence(string comp)
    {
        if (!Competencies.Remove(comp)) { throw new Exception($"Coach {Name} does not have this competence."); }
    }

    public void AdjustCompetences(List<string> toAdd, List<string> toRemove)
    {
        if (toAdd.Count == 0 && toRemove.Count == 0)
        {
            throw new Exception("A minimum of one skill to either add or remove must be given.");
        }
        foreach (var skill in toRemove.Distinct())
        {
            RemoveCompetence(skill);
        }
        foreach (var skill in toAdd.Distinct())
        {
            AddCompetence(skill);
        }
    }

    public void BookIn(Booking newbooking)
    {
        if (!Bookings.Any(booking => booking.BookingOverlap(newbooking))) { Bookings.Add(newbooking); }
        else throw new Exception("Coach's schedule does not match with this planning.");
    }

    public bool IsCompetent(List<string> requirements)
    {
        var competent = requirements.All(c => Competencies.Contains(c));

        return competent;
    }
}
