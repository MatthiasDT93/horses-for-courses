using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.Tests;

public abstract class CrudTestBase<TContext, TEntity>
    where TContext : DbContext
    where TEntity : class
{
    private readonly SqliteConnection _connection;

    protected CrudTestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected DbContextOptions<TContext> CreateOptions() =>
        new DbContextOptionsBuilder<TContext>()
            .UseSqlite(_connection)
            .Options;

    protected abstract TContext CreateContext(DbContextOptions<TContext> options);
    protected abstract TEntity CreateEntity();
    protected abstract Task ModifyEntityAsync(TEntity entity);
    protected abstract Task AssertUpdatedAsync(TEntity entity);
    protected abstract DbSet<TEntity> GetDbSet(TContext context);

    [Fact]
    public async Task CanCreateReadUpdateDelete()
    {
        // Arrange
        var options = CreateOptions();
        using (var context = CreateContext(options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        TEntity entity;

        // Create
        using (var context = CreateContext(options))
        {
            entity = CreateEntity();
            GetDbSet(context).Add(entity);
            await context.SaveChangesAsync();
        }

        // Read
        using (var context = CreateContext(options))
        {
            var found = await GetDbSet(context).FindAsync(GetPrimaryKey(entity));
            Assert.NotNull(found);
        }

        // Update
        using (var context = CreateContext(options))
        {
            var toUpdate = await GetDbSet(context).FindAsync(GetPrimaryKey(entity));
            await ModifyEntityAsync(toUpdate!);
            await context.SaveChangesAsync();
        }

        // Verify Update
        using (var context = CreateContext(options))
        {
            var updated = await GetDbSet(context).FindAsync(GetPrimaryKey(entity));
            await AssertUpdatedAsync(updated!);
        }

        // Delete
        using (var context = CreateContext(options))
        {
            var toDelete = await GetDbSet(context).FindAsync(GetPrimaryKey(entity));
            GetDbSet(context).Remove(toDelete!);
            await context.SaveChangesAsync();
        }

        // Verify Delete
        using (var context = CreateContext(options))
        {
            var shouldBeNull = await GetDbSet(context).FindAsync(GetPrimaryKey(entity));
            Assert.Null(shouldBeNull);
        }
    }

    protected abstract object[] GetPrimaryKey(TEntity entity);
}