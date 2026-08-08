using AssignmentSystem.Core.DTOs;
using AssignmentSystem.Core.Entities;
using AssignmentSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]  // Only Admin can access anything in here
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    // ── Users ─────────────────────────────────────────────────────────────────

    // GET /api/admin/users
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _db.Users
            .Select(u => new UserProfileResponse
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    // POST /api/admin/users
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new { message = "Email already exists" });

        if (!Enum.TryParse<UserRole>(request.Role, out var role))
            return BadRequest(new { message = "Invalid role. Use Admin, Teacher, or Student" });

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllUsers), new { id = user.Id }, new UserProfileResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        });
    }

    // PATCH /api/admin/users/{id}/toggle-active
    [HttpPatch("users/{id}/toggle-active")]
    public async Task<IActionResult> ToggleUserActive(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { message = $"User is now {(user.IsActive ? "active" : "inactive")}" });
    }

    // ── Classes ───────────────────────────────────────────────────────────────

    // GET /api/admin/classes
    [HttpGet("classes")]
    public async Task<IActionResult> GetAllClasses()
    {
        var classes = await _db.Classes
            .Include(c => c.Subjects)
                .ThenInclude(s => s.Teacher)
            .Include(c => c.Enrollments)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Section,
                c.Description,
                c.CreatedAt,
                StudentCount = c.Enrollments.Count,
                Subjects = c.Subjects.Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.Code,
                    Teacher = s.Teacher.FullName
                })
            })
            .ToListAsync();

        return Ok(classes);
    }

    // POST /api/admin/classes
    [HttpPost("classes")]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest request)
    {
        var cls = new Class
        {
            Name = request.Name,
            Section = request.Section,
            Description = request.Description
        };

        _db.Classes.Add(cls);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllClasses), new { id = cls.Id }, cls);
    }

    // ── Subjects ──────────────────────────────────────────────────────────────

    // POST /api/admin/subjects
    [HttpPost("subjects")]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest request)
    {
        var classExists = await _db.Classes.AnyAsync(c => c.Id == request.ClassId);
        if (!classExists) return BadRequest(new { message = "Class not found" });

        var teacher = await _db.Users.FirstOrDefaultAsync(u =>
            u.Id == request.TeacherId && u.Role == UserRole.Teacher);
        if (teacher == null) return BadRequest(new { message = "Teacher not found" });

        var subject = new Subject
        {
            Name = request.Name,
            Code = request.Code,
            ClassId = request.ClassId,
            TeacherId = request.TeacherId
        };

        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAllClasses), new { id = subject.Id }, subject);
    }

    // POST /api/admin/enroll
    [HttpPost("enroll")]
    public async Task<IActionResult> EnrollStudent([FromBody] EnrollStudentRequest request)
    {
        var student = await _db.Users.FirstOrDefaultAsync(u =>
            u.Id == request.StudentId && u.Role == UserRole.Student);
        if (student == null) return BadRequest(new { message = "Student not found" });

        var classExists = await _db.Classes.AnyAsync(c => c.Id == request.ClassId);
        if (!classExists) return BadRequest(new { message = "Class not found" });

        var alreadyEnrolled = await _db.ClassEnrollments.AnyAsync(e =>
            e.StudentId == request.StudentId && e.ClassId == request.ClassId);
        if (alreadyEnrolled) return BadRequest(new { message = "Student already enrolled" });

        var enrollment = new ClassEnrollment
        {
            StudentId = request.StudentId,
            ClassId = request.ClassId
        };

        _db.ClassEnrollments.Add(enrollment);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Student enrolled successfully" });
    }
}