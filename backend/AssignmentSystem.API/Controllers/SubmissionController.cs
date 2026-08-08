using AssignmentSystem.Core.DTOs;
using AssignmentSystem.Core.Entities;
using AssignmentSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionController : ControllerBase
{
    private readonly AppDbContext _db;

    public SubmissionController(AppDbContext db)
    {
        _db = db;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ── Student: Submit Assignment ─────────────────────────────────────────────

    // POST /api/submission
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Submit([FromBody] CreateSubmissionRequest request)
    {
        var studentId = GetUserId();

        // Check assignment exists and is published
        var assignment = await _db.Assignments
            .Include(a => a.Subject)
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId &&
                                      a.Status == AssignmentStatus.Published);

        if (assignment == null)
            return BadRequest(new { message = "Assignment not found or not published" });

        // Check student is enrolled in the class
        var isEnrolled = await _db.ClassEnrollments
            .AnyAsync(e => e.StudentId == studentId &&
                           e.ClassId == assignment.Subject.ClassId);

        if (!isEnrolled)
            return BadRequest(new { message = "You are not enrolled in this class" });

        // Check for duplicate submission
        var existing = await _db.Submissions
            .FirstOrDefaultAsync(s => s.StudentId == studentId &&
                                      s.AssignmentId == request.AssignmentId);

        if (existing != null)
            return BadRequest(new { message = "You have already submitted this assignment" });

        // Determine if submission is late
        var status = DateTime.UtcNow > assignment.Deadline
            ? SubmissionStatus.Late
            : SubmissionStatus.Submitted;

        var submission = new Submission
        {
            Answer = request.Answer,
            AssignmentId = request.AssignmentId,
            StudentId = studentId,
            Status = status,
            SubmittedAt = DateTime.UtcNow
        };

        _db.Submissions.Add(submission);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMySubmission), new { assignmentId = assignment.Id },
            new { submission.Id, Status = submission.Status.ToString(), submission.SubmittedAt });
    }

    // ── Student: Update Submission (before deadline) ───────────────────────────

    // PUT /api/submission/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> UpdateSubmission(Guid id, [FromBody] UpdateSubmissionRequest request)
    {
        var studentId = GetUserId();

        var submission = await _db.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.Id == id && s.StudentId == studentId);

        if (submission == null) return NotFound();

        // Only allow update before deadline
        if (DateTime.UtcNow > submission.Assignment.Deadline)
            return BadRequest(new { message = "Deadline has passed, cannot update submission" });

        submission.Answer = request.Answer;
        submission.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Submission updated successfully" });
    }

    // ── Student: Get My Submission for an Assignment ───────────────────────────

    // GET /api/submission/my/{assignmentId}
    [HttpGet("my/{assignmentId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMySubmission(Guid assignmentId)
    {
        var studentId = GetUserId();

        var submission = await _db.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.StudentId == studentId &&
                                      s.AssignmentId == assignmentId);

        if (submission == null) return NotFound();

        return Ok(new
        {
            submission.Id,
            submission.Answer,
            Status = submission.Status.ToString(),
            submission.Marks,
            submission.Feedback,
            submission.SubmittedAt,
            Assignment = new
            {
                submission.Assignment.Title,
                submission.Assignment.Deadline,
                submission.Assignment.MaxMarks
            }
        });
    }

    // ── Teacher: View Submissions for an Assignment ────────────────────────────

    // GET /api/submission/assignment/{assignmentId}
    [HttpGet("assignment/{assignmentId}")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> GetSubmissionsForAssignment(Guid assignmentId)
    {
        var userId = GetUserId();
        var role = User.FindFirstValue(ClaimTypes.Role);

        // Teachers can only see submissions for their own assignments
        var assignment = await _db.Assignments
            .FirstOrDefaultAsync(a => a.Id == assignmentId &&
                (role == "Admin" || a.TeacherId == userId));

        if (assignment == null) return NotFound();

        var submissions = await _db.Submissions
            .Include(s => s.Student)
            .Where(s => s.AssignmentId == assignmentId)
            .OrderBy(s => s.SubmittedAt)
            .Select(s => new
            {
                s.Id,
                Student = new { s.Student.FullName, s.Student.Email },
                s.Answer,
                Status = s.Status.ToString(),
                s.Marks,
                s.Feedback,
                s.SubmittedAt
            })
            .ToListAsync();

        return Ok(submissions);
    }

    // ── Teacher: Grade a Submission ────────────────────────────────────────────

    // PATCH /api/submission/{id}/grade
    [HttpPatch("{id}/grade")]
    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> GradeSubmission(Guid id, [FromBody] GradeSubmissionRequest request)
    {
        var userId = GetUserId();
        var role = User.FindFirstValue(ClaimTypes.Role);

        var submission = await _db.Submissions
            .Include(s => s.Assignment)
            .FirstOrDefaultAsync(s => s.Id == id &&
                (role == "Admin" || s.Assignment.TeacherId == userId));

        if (submission == null) return NotFound();

        if (request.Marks > submission.Assignment.MaxMarks)
            return BadRequest(new { message = $"Marks cannot exceed {submission.Assignment.MaxMarks}" });

        submission.Marks = request.Marks;
        submission.Feedback = request.Feedback;
        submission.Status = SubmissionStatus.Reviewed;
        submission.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Submission graded successfully" });
    }
}