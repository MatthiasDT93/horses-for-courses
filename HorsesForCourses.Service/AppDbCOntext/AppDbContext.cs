using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace HorsesForCourses.Service;

public class AppDbContext : DbContext
{
    public DbSet<Coach> Coaches => Set<Coach>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<AppUser> Users => Set<AppUser>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========== COACH ==========
        modelBuilder.Entity<Coach>(coach =>
        {
            coach.HasKey(c => c.Id);

            coach.Property(c => c.Name).IsRequired();
            coach.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.Value).HasColumnName("Email").IsRequired();
            });

            // Competencies (List<string>) as owned collection
            coach.Property<List<string>>("Competencies")
                 .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));

            // Bookings as value objects
            coach.OwnsMany(c => c.bookings, booking =>
            {
                booking.WithOwner().HasForeignKey("CoachId");

                booking.Property<int>("Id"); // shadow property
                booking.HasKey("Id");

                booking.Property(b => b.StartDate);
                booking.Property(b => b.EndDate);

                // Timeslot list inside Booking
                booking.OwnsMany(b => b.Planning, ts =>
                {
                    ts.WithOwner().HasForeignKey("BookingId");

                    ts.Property(t => t.Day)
                        .HasConversion(
                            v => v.ToString(),
                            v => Enum.Parse<DayOfWeek>(v))
                        .HasColumnName("Day");

                    ts.Property(t => t.Start).HasColumnName("Start");
                    ts.Property(t => t.End).HasColumnName("End");

                    ts.HasKey("BookingId", "Day", "Start", "End");
                    ts.ToTable("BookingTimeslots");
                });

                booking.ToTable("CoachBookings");
            });

            coach.ToTable("Coaches");
        });

        // ========== COURSE ==========
        modelBuilder.Entity<Course>(course =>
        {
            course.HasKey(c => c.Id);

            course.Property(c => c.CourseName).IsRequired();
            course.Property(nameof(Course.StartDate));
            course.Property(nameof(Course.EndDate));

            // Status enum stored as string
            course.Property<States>("Status")
                .HasConversion(
                    v => v.ToString(),
                    v => (States)Enum.Parse(typeof(States), v))
                .HasColumnName("Status");

            // Optional Coach FK
            course.HasOne(c => c.coach)
                .WithMany(c => c.Courses)
                .HasForeignKey("CoachId")
                .IsRequired(false);

            // Planning as Timeslots
            course.OwnsMany(c => c.Planning, ts =>
            {
                ts.WithOwner().HasForeignKey("CourseId");

                ts.Property(t => t.Day)
                    .HasConversion(
                        v => v.ToString(),
                        v => Enum.Parse<DayOfWeek>(v))
                    .HasColumnName("Day");

                ts.Property(t => t.Start).HasColumnName("Start");
                ts.Property(t => t.End).HasColumnName("End");

                ts.HasKey("CourseId", "Day", "Start", "End");
                ts.ToTable("CourseTimeslots");
            });

            // Required Competencies (List<string>)
            course.Property(c => c.RequiredCompetencies)
                 .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));

            course.ToTable("Courses");
        });

        //===== APP USER ========
        modelBuilder.Entity<AppUser>(user =>
        {
            user.HasKey(u => u.Id);

            user.Property(u => u.Name).IsRequired();
            user.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.Value).HasColumnName("Email").IsRequired();
                email.HasIndex(e => e.Value).IsUnique();
            });
            user.Property(u => u.PasswordHash).IsRequired();
            user.ToTable("Users");
        });
    }
}


public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=horsesforcourses.db"); // Or your connection string

        return new AppDbContext(optionsBuilder.Options);
    }
}