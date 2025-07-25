using System.Linq.Expressions;
using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class CoachTest
{
    Coach coach;
    List<Timeslot> planning = new();

    Booking booking;
    public CoachTest()
    {
        coach = new Coach("Mark", "mark@skynet.com");
        var slot1 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(10, 0));
        var slot2 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(12, 0), new TimeOnly(15, 0));
        var slot3 = Timeslot.From(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(10, 0));
        var slot4 = Timeslot.From(DayOfWeek.Thursday, new TimeOnly(9, 0), new TimeOnly(10, 0));

        var startdate = new DateOnly(2025, 8, 8);
        var enddate = new DateOnly(2026, 8, 8);

        planning.Add(slot1);
        planning.Add(slot2);
        planning.Add(slot3);
        planning.Add(slot4);

        booking = Booking.From(planning, startdate, enddate);
    }


    [Fact]
    public void Creating_Coach()
    {
        Assert.Equal("Mark", coach.Name);
        Assert.Equal(EmailAddress.From("mark@skynet.com"), coach.Email);
    }


    [Fact]
    public void Adding_Competences()
    {
        coach.AddCompetence("Javascript");
        coach.AddCompetence("HTML");

        Assert.Contains("Javascript", coach.competencies);
        Assert.Contains("HTML", coach.competencies);

        var exception = Assert.Throws<Exception>(() => coach.AddCompetence("Javascript"));
        Assert.Equal("Coach Mark already has this competence.", exception.Message);
    }

    [Fact]
    public void Remove_Competences()
    {
        coach.AddCompetence("Javascript");
        coach.AddCompetence("HTML");

        coach.RemoveCompetence("Javascript");

        Assert.DoesNotContain("Javascript", coach.competencies);
        Assert.Contains("HTML", coach.competencies);

        var exception = Assert.Throws<Exception>(() => coach.RemoveCompetence("hoola hooping"));
        Assert.Equal("Coach Mark does not have this competence.", exception.Message);
    }

    [Fact]
    public void Book_A_Coach()
    {
        coach.BookIn(booking);


        var slot1 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(10, 0));
        var result = coach.bookings.Any(b =>
        b.Planning.Any(s =>
            s.Day == slot1.Day &&
            s.Start == slot1.Start &&
            s.End == slot1.End
            )
        );
        Assert.True(result);
    }

    [Fact]
    public void Cannot_Book_In_A_Coach_When_They_Are_Busy()
    {
        coach.BookIn(booking);

        var startdate2 = new DateOnly(2025, 2, 2);
        var enddate2 = new DateOnly(2026, 3, 3);

        var list = new List<Timeslot> { Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(10, 0)) };

        var newbooking = new Booking(list, startdate2, enddate2);
        var exception = Assert.Throws<Exception>(() => coach.BookIn(newbooking));
        Assert.Equal("Coach's schedule does not match with this planning.", exception.Message);
    }

    [Fact]
    public void Testing_For_Competence()
    {
        coach.AddCompetence("Javascript");
        coach.AddCompetence("HTML");
        coach.AddCompetence("C#");

        var compfalse = new List<string> { "Javascript", "HTML", "C#", "Python" };
        var comptrue = new List<string> { "Javascript", "HTML", "C#" };

        Assert.False(coach.IsCompetent(compfalse));
        Assert.True(coach.IsCompetent(comptrue));
    }
}
