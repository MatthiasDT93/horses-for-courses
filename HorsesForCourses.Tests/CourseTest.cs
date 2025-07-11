using System.Net;
using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class CourseTest
{
    Course course;

    public DateOnly coursestart;

    public DateOnly courseend;

    public Coach coach;
    public Coach coach2;

    public CourseTest()
    {
        coursestart = new DateOnly(2025, 7, 11);
        courseend = new DateOnly(2025, 11, 11);
        course = new Course("Programming 1", coursestart, courseend);
        coach = new Coach("Mark", "mark@skynet.com");
        coach2 = new Coach("Benny", "benny@skynet.com");
    }

    [Fact]
    public void Make_A_Course()
    {
        Assert.Equal("Programming 1", course.CourseName);
        Assert.Equal(coursestart, course.StartDate);
        Assert.Equal(courseend, course.EndDate);
    }

    [Fact]
    public void Adding_Requirements()
    {
        course.AddRequirement("Javascript");
        course.AddRequirement("HTML");
        course.AddRequirement("CSS");

        Assert.Contains("Javascript", course.RequiredCompetencies);

        var exception = Assert.Throws<Exception>(() => course.AddRequirement("Javascript"));
        Assert.Equal("This required competence is already added.", exception.Message);
    }

    [Fact]
    public void Removing_Requirements()
    {
        course.AddRequirement("Javascript");
        course.AddRequirement("HTML");
        course.AddRequirement("CSS");

        course.RemoveRequirement("CSS");
        Assert.DoesNotContain("CSS", course.RequiredCompetencies);

        var exception = Assert.Throws<Exception>(() => course.RemoveRequirement("C#"));
        Assert.Equal("This course does not have this requirement.", exception.Message);
    }

    [Fact]
    public void Adding_CourseMoment()
    {
        var slot = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));
        course.AddCourseMoment(slot);
        Assert.Contains(slot, course.Planning);

        var slot2 = Timeslot.From(DayOfWeek.Monday, new TimeOnly(10, 0), new TimeOnly(12, 0));
        var exception = Assert.Throws<Exception>(() => course.AddCourseMoment(slot2));
        Assert.Equal("There is overlap between the time slots.", exception.Message);
    }

    [Fact]
    public void Removing_CourseMoment()
    {
        var slot = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));
        course.AddCourseMoment(slot);
        Assert.Contains(slot, course.Planning);

        course.RemoveCourseMoment(slot);
        Assert.DoesNotContain(slot, course.Planning);

        var slot2 = Timeslot.From(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(12, 0));
        var exception = Assert.Throws<Exception>(() => course.RemoveCourseMoment(slot2));
        Assert.Equal("This is not yet planned in.", exception.Message);
    }

    [Fact]
    public void Confirming_Course()
    {
        var exception = Assert.Throws<Exception>(() => course.ConfirmCourse());
        Assert.Equal("Cannot confirm a course that does not have a planning yet.", exception.Message);

        var slot = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));
        course.AddCourseMoment(slot);

        course.ConfirmCourse();
        var exception2 = Assert.Throws<Exception>(() => course.ConfirmCourse());
        Assert.Equal("Cannot confirm a course that's not in the PENDING state, current state is: CONFIRMED.", exception2.Message);
    }

    [Fact]
    public void Adding_A_Coach()
    {
        var exception1 = Assert.Throws<Exception>(() => course.AddCoach(coach));
        Assert.Equal("Course needs to be CONFIRMED before adding a coach.", exception1.Message);

        course.AddRequirement("Javascript");
        course.AddRequirement("HTML");
        course.AddRequirement("CSS");

        var slot = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));
        course.AddCourseMoment(slot);

        course.ConfirmCourse();
        var exception2 = Assert.Throws<Exception>(() => course.AddCoach(coach));
        Assert.Equal("The coach does not meet the requirements for teaching this course.", exception2.Message);

        coach.AddCompetence("Javascript");
        coach.AddCompetence("HTML");
        coach.AddCompetence("CSS");

        var planning = new List<Timeslot> { slot };

        coach.BookIn(planning);
        var exception3 = Assert.Throws<Exception>(() => course.AddCoach(coach));
        Assert.Equal("The coach's schedule does not match the planning of the course.", exception3.Message);

        coach2.AddCompetence("Javascript");
        coach2.AddCompetence("HTML");
        coach2.AddCompetence("CSS");

        course.AddCoach(coach2);
        Assert.Equal("Benny", course.coach.Name);
    }

    [Fact]
    public void After_FINALISED_No_Further_Adjustments_Possible()
    {
        course.AddRequirement("Javascript");
        course.AddRequirement("HTML");
        course.AddRequirement("CSS");

        var slot = Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(11, 0));
        course.AddCourseMoment(slot);

        course.ConfirmCourse();

        coach.AddCompetence("Javascript");
        coach.AddCompetence("HTML");
        coach.AddCompetence("CSS");

        course.AddCoach(coach);

        var exception1 = Assert.Throws<Exception>(() => course.AddRequirement("Python"));
        Assert.Equal("Course has been finalised and cannot be altered.", exception1.Message);

        var exception2 = Assert.Throws<Exception>(() => course.RemoveRequirement("Python"));
        Assert.Equal("Course has been finalised and cannot be altered.", exception2.Message);

        var exception3 = Assert.Throws<Exception>(() => course.AddCourseMoment(slot));
        Assert.Equal("Course has been finalised and cannot be altered.", exception3.Message);

        var exception4 = Assert.Throws<Exception>(() => course.RemoveCourseMoment(slot));
        Assert.Equal("Course has been finalised and cannot be altered.", exception4.Message);

        var exception5 = Assert.Throws<Exception>(() => course.ConfirmCourse());
        Assert.Equal("Cannot confirm a course that's not in the PENDING state, current state is: FINALISED.", exception5.Message);

        var exception6 = Assert.Throws<Exception>(() => course.AddCoach(coach));
        Assert.Equal("Course has been finalised and cannot be altered.", exception6.Message);

    }
}