using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.Data.Sqlite;

namespace HorsesForCourses.Tests.Data;

public class CoachPersistancyTests
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
            context.Coaches.Add(new Coach("naam", "em@il"));
            await context.SaveChangesAsync();
        }

        using (var context = new AppDbContext(options))
        {
            var coach = await context.Coaches.FindAsync(1);

            Assert.NotNull(coach);
            Assert.Equal("naam", coach!.Name);
        }
    }
}