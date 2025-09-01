

using HorsesForCourses.Core;
using HorsesForCourses.Service;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests;

public class CourseCrudTests : CrudTestBase<AppDbContext, Course>
{
    protected override AppDbContext CreateContext(DbContextOptions<AppDbContext> options)
    {
        return new AppDbContext(options);
    }

    protected override Course CreateEntity()
    {
        return new Course("C# 101", new DateOnly(2025, 8, 8), new DateOnly(2026, 8, 8));
    }

    protected override DbSet<Course> GetDbSet(AppDbContext context)
    {
        return context.Courses;
    }

    protected override object[] GetPrimaryKey(Course entity)
    {
        return new object[] { entity.Id };
    }

    protected override async Task ModifyEntityAsync(Course entity)
    {
        entity.OverWriteRequirements(new List<string> { "C#", "EF" });
        await Task.CompletedTask;
    }

    protected override async Task AssertUpdatedAsync(Course entity)
    {
        Assert.Contains("C#", entity.RequiredCompetencies);
        Assert.Contains("EF", entity.RequiredCompetencies);
        await Task.CompletedTask;
    }
}