using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class CoachTest
{
    Coach coach;
    public CoachTest()
    {
        coach = new Coach("Mark", "mark@skynet.com");
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
    }

    [Fact]
    public void Remove_Competences()
    {
        coach.AddCompetence("Javascript");
        coach.AddCompetence("HTML");

        coach.RemoveCompetence("Javascript");

        Assert.DoesNotContain("Javascript", coach.competencies);
        Assert.Contains("HTML", coach.competencies);
    }

    [Fact]
    public void Book_A_Coach()
    {

        //coach.BookIn()
    }

}