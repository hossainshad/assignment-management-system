using AssignmentSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Only seed if no users exist yet
        if (await context.Users.AnyAsync()) return;

        // Create demo users (passwords are hashed with BCrypt)
        var admin = new User
        {
            FullName = "System Admin",
            Email = "admin@school.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = UserRole.Admin
        };

        var teacher = new User
        {
            FullName = "John Teacher",
            Email = "teacher@school.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"),
            Role = UserRole.Teacher
        };

        var student = new User
        {
            FullName = "Jane Student",
            Email = "student@school.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
            Role = UserRole.Student
        };

        await context.Users.AddRangeAsync(admin, teacher, student);

        // Create a demo class
        var demoClass = new Class
        {
            Name = "Class 10",
            Section = "A",
            Description = "Demo class for testing"
        };

        await context.Classes.AddAsync(demoClass);
        await context.SaveChangesAsync();

        // Create a demo subject and assign the teacher
        var subject = new Subject
        {
            Name = "Mathematics",
            Code = "MATH101",
            ClassId = demoClass.Id,
            TeacherId = teacher.Id
        };

        await context.Subjects.AddAsync(subject);

        // Enroll the student in the class
        var enrollment = new ClassEnrollment
        {
            StudentId = student.Id,
            ClassId = demoClass.Id
        };

        await context.ClassEnrollments.AddAsync(enrollment);
        await context.SaveChangesAsync();
    }
}