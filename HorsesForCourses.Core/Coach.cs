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
