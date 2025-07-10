using System.Diagnostics.Contracts;

namespace HorsesForCourses.Core;

public class Coach
{
    public List<string> Competencies = new();

    public List<CourseTime> Bookings = new();

    public string Name { get; set; }

    public EmailAddress Email { get; set; }


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

    public void BookIn(CourseTime ct)
    {
        var busy = Bookings.Any(t => ct.OverlapEarly(ct) || ct.OverlapContain(t) || ct.OverlapAfter(t));
        if (!busy)
        {
            Bookings.Add(ct);
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
