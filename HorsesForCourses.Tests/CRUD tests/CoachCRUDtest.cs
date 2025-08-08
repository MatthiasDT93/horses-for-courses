

using HorsesForCourses.Core;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests;

public class CoachCrudTests : CrudTestBase<AppDbContext, Coach>
{
    protected override AppDbContext CreateContext(DbContextOptions<AppDbContext> options)
    {
        return new AppDbContext(options);
    }

    protected override Coach CreateEntity()
    {
        return new Coach("Mark", "mark@skynet.com");
    }

    protected override DbSet<Coach> GetDbSet(AppDbContext context)
    {
        return context.Coaches;
    }

    protected override object[] GetPrimaryKey(Coach entity)
    {
        return new object[] { entity.Id };
    }

    protected override async Task ModifyEntityAsync(Coach entity)
    {
        entity.OverWriteCompetences(new List<string> { "C#", "Javascript" });
        await Task.CompletedTask;
    }

    protected override async Task AssertUpdatedAsync(Coach entity)
    {
        Assert.Contains("C#", entity.competencies);
        Assert.Contains("Javascript", entity.competencies);
        await Task.CompletedTask;
    }
}