

using HorsesForCourses.Core;
using HorsesForCourses.Service;
using HorsesForCourses.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests;

public class AppUserCrudTests : CrudTestBase<AppDbContext, AppUser>
{
    protected override AppDbContext CreateContext(DbContextOptions<AppDbContext> options)
    {
        return new AppDbContext(options);
    }

    protected override AppUser CreateEntity()
    {
        return AppUser.From("Mark", "mark@skynet.com", "123", "123");
    }

    protected override DbSet<AppUser> GetDbSet(AppDbContext context)
    {
        return context.Users;
    }

    protected override object[] GetPrimaryKey(AppUser entity)
    {
        return new object[] { entity.Id };
    }

    protected override async Task ModifyEntityAsync(AppUser entity)
    {
        await Task.CompletedTask;
    }

    protected override async Task AssertUpdatedAsync(AppUser entity)
    {
        await Task.CompletedTask;
    }
}