using AssignmentSystem.Core.Entities;
using AssignmentSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AssignmentSystem.Core.DTOs;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentController : ControllerBase
{
    private readonly AppDbContext _db;

    public AssignmentController(AppDbContext db)
    {
        _db = db;
    }

    // Helper: get current user's ID from JWT token
    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Helper: get current user's role from JWT token
    private string GetUserRole() =>
        User.FindFirstValue(ClaimTypes.Role)!;

    // ── Teacher: Create Assignment ─────────────────────────────────────────────

    // POST /api/assignment
    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequest request)
    {
        // Make sure the subject exists and belongs to this teacher
        var subject = await _db.Subjects
            .FirstOrDefaultAsync(s => s.Id == request.SubjectId && s.TeacherId == GetUserId());

        if (subject == null)
            return BadRequest(new { message = "Subject not found or not assigned to you" });

        var assignment = new Assignment
        {
            Title = request.Title,
            Description = request.Description,
            Deadline = request.Deadline,
            MaxMarks = request.MaxMarks,
            Status = request.Publish ? AssignmentStatus.Published : AssignmentStatus.Draft,
            SubjectId = request.SubjectId,
            TeacherId = GetUserId()
        };

        _db.Assignments.Add(assignment);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, new
            {
                assignment.Id,
                assignment.Title,
                assignment.Description,
                assignment.Deadline,
                assignment.MaxMarks,
                Status = assignment.Status.ToString(),
                assignment.SubjectId,
                assignment.TeacherId,
                assignment.CreatedAt
            });
    }

    // ── Teacher: Update Assignment ─────────────────────────────────────────────

    // PUT /api/assignment/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> UpdateAssignment(Guid id, [FromBody] CreateAssignmentRequest request)
    {
        var assignment = await _db.Assignments
            .FirstOrDefaultAsync(a => a.Id == id && a.TeacherId == GetUserId());

        if (assignment == null) return NotFound();

        assignment.Title = request.Title;
        assignment.Description = request.Description;
        assignment.Deadline = request.Deadline;
        assignment.MaxMarks = request.MaxMarks;
        assignment.Status = request.Publish ? AssignmentStatus.Published : AssignmentStatus.Draft;
        assignment.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new
            {
                assignment.Id,
                assignment.Title,
                assignment.Description,
                assignment.Deadline,
                assignment.MaxMarks,
                Status = assignment.Status.ToString(),
                assignment.UpdatedAt
            });
    }

    // ── Teacher: Delete Assignment ─────────────────────────────────────────────

    // DELETE /api/assignment/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> DeleteAssignment(Guid id)
    {
        var assignment = await _db.Assignments
            .FirstOrDefaultAsync(a => a.Id == id && a.TeacherId == GetUserId());

        if (assignment == null) return NotFound();

        _db.Assignments.Remove(assignment);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Assignment deleted" });
    }

    // ── Shared: Get Single Assignment ──────────────────────────────────────────

    // GET /api/assignment/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAssignment(Guid id)
    {
        var role = GetUserRole();
        var userId = GetUserId();

        var assignment = await _db.Assignments
            .Include(a => a.Subject)
                .ThenInclude(s => s.Class)
            .Include(a => a.Teacher)
            .Include(a => a.Submissions)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (assignment == null) return NotFound();

        // Students can only see published assignments for their class
        if (role == "Student")
        {
            var isEnrolled = await _db.ClassEnrollments
                .AnyAsync(e => e.StudentId == userId &&
                               e.ClassId == assignment.Subject.ClassId);

            if (!isEnrolled || assignment.Status != AssignmentStatus.Published)
                return NotFound();
        }

        return Ok(new
        {
            assignment.Id,
            assignment.Title,
            assignment.Description,
            assignment.Deadline,
            assignment.MaxMarks,
            Status = assignment.Status.ToString(),
            Subject = new { assignment.Subject.Id, assignment.Subject.Name, assignment.Subject.Code },
            Class = new { assignment.Subject.Class.Id, assignment.Subject.Class.Name, assignment.Subject.Class.Section },
            Teacher = assignment.Teacher.FullName,
            SubmissionCount = assignment.Submissions.Count,
            assignment.CreatedAt
        });
    }

    // ── Teacher: Get My Assignments ────────────────────────────────────────────

    // GET /api/assignment/my
    [HttpGet("my")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> GetMyAssignments()
    {
        var userId = GetUserId();
        var role = GetUserRole();

        var query = _db.Assignments
            .Include(a => a.Subject)
                .ThenInclude(s => s.Class)
            .Include(a => a.Submissions)
            .AsQueryable();

        // Admin sees all, Teacher sees only their own
        if (role == "Teacher")
            query = query.Where(a => a.TeacherId == userId);

        var assignments = await query
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Deadline,
                a.MaxMarks,
                Status = a.Status.ToString(),
                Subject = a.Subject.Name,
                Class = a.Subject.Class.Name + " " + a.Subject.Class.Section,
                SubmissionCount = a.Submissions.Count,
                a.CreatedAt
            })
            .ToListAsync();

        return Ok(assignments);
    }

    // ── Student: Get Assignments For My Class ──────────────────────────────────

    // GET /api/assignment/class
    [HttpGet("class")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetClassAssignments()
    {
        var studentId = GetUserId();

        // Get all classes the student is enrolled in
        var classIds = await _db.ClassEnrollments
            .Where(e => e.StudentId == studentId)
            .Select(e => e.ClassId)
            .ToListAsync();

        var assignments = await _db.Assignments
            .Include(a => a.Subject)
                .ThenInclude(s => s.Class)
            .Include(a => a.Teacher)
            .Include(a => a.Submissions.Where(s => s.StudentId == studentId))
            .Where(a => classIds.Contains(a.Subject.ClassId) &&
                        a.Status == AssignmentStatus.Published)
            .OrderByDescending(a => a.Deadline)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Deadline,
                a.MaxMarks,
                Subject = a.Subject.Name,
                Class = a.Subject.Class.Name + " " + a.Subject.Class.Section,
                Teacher = a.Teacher.FullName,
                MySubmission = a.Submissions.Select(s => new
                {
                    s.Id,
                    Status = s.Status.ToString(),
                    s.Marks,
                    s.SubmittedAt
                }).FirstOrDefault()
            })
            .ToListAsync();

        return Ok(assignments);
    }
}