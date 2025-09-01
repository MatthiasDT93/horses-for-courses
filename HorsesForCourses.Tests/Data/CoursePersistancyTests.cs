using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.Data.Sqlite;
using HorsesForCourses.Service;

namespace HorsesForCourses.Tests.Data;

public class CoursePersistancyTests
{
    [Fact]
    public async Task ShouldPersistData()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new AppDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var coach = new Coach("naam", "em@il");
            coach.AddCompetence("dev");
            context.Coaches.Add(coach);

            var course = new Course("cursus", new DateOnly(2025, 1, 1), new DateOnly(2025, 2, 1));
            course.AddRequirement("dev");
            course.AddCourseMoment(
                Timeslot.From(DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(12, 0)));
            course.ConfirmCourse();
            course.AddCoach(coach);
            context.Courses.Add(course);

            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var coach = await context.Coaches
                .Include(c => c.Courses)
                .FirstOrDefaultAsync(c => c.Id == 1);

            Assert.NotNull(coach);
            Assert.Equal("naam", coach!.Name);
            Assert.NotNull(coach.Courses);
            Assert.Single(coach.Courses);

            var course = await context.Courses.FindAsync(1);
            Assert.NotNull(course);
            Assert.Equal("cursus", course!.CourseName);
            Assert.NotNull(course.coach);
        }
    }
}