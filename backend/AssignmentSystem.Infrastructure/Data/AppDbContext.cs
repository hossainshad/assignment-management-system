using AssignmentSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.Infrastructure.Data;

// AppDbContext is your database session — all queries go through this
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Each DbSet = one table in PostgreSQL
    public DbSet<User> Users => Set<User>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<ClassEnrollment> ClassEnrollments => Set<ClassEnrollment>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User: make Email unique
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // User: store Role enum as string ("Admin", "Teacher", "Student") not int
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        // Assignment: store Status enum as string
        modelBuilder.Entity<Assignment>()
            .Property(a => a.Status)
            .HasConversion<string>();

        // Submission: store Status enum as string
        modelBuilder.Entity<Submission>()
            .Property(s => s.Status)
            .HasConversion<string>();

        // ClassEnrollment: one student can only be enrolled once per class
        modelBuilder.Entity<ClassEnrollment>()
            .HasIndex(ce => new { ce.StudentId, ce.ClassId })
            .IsUnique();

        // Submission: one student can only submit once per assignment
        modelBuilder.Entity<Submission>()
            .HasIndex(s => new { s.StudentId, s.AssignmentId })
            .IsUnique();

        // Prevent cascade delete loops (EF Core throws if you don't do this)
        modelBuilder.Entity<Subject>()
            .HasOne(s => s.Teacher)
            .WithMany()
            .HasForeignKey(s => s.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Teacher)
            .WithMany(u => u.Assignments)
            .HasForeignKey(a => a.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}