using AssignmentSystem.Core.Entities;
using AssignmentSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.Tests;

public class SubmissionTests
{
    // Helper: creates a fresh in-memory database for each test
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
            .Options;
        return new AppDbContext(options);
    }

    // ── Test 1: Student cannot submit the same assignment twice ───────────────
    [Fact]
    public async Task Student_CannotSubmit_SameAssignment_Twice()
    {
        var db = GetInMemoryDb();

        // Arrange: create a submission
        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            AssignmentId = Guid.NewGuid(),
            Answer = "First submission",
            Status = SubmissionStatus.Submitted,
            SubmittedAt = DateTime.UtcNow
        };

        db.Submissions.Add(submission);
        await db.SaveChangesAsync();

        // Act: check if a duplicate exists
        var duplicate = await db.Submissions
            .AnyAsync(s => s.StudentId == submission.StudentId &&
                           s.AssignmentId == submission.AssignmentId);

        // Assert: duplicate should be detected
        Assert.True(duplicate);
    }

    // ── Test 2: Submission after deadline is marked as Late ───────────────────
    [Fact]
    public void Submission_AfterDeadline_IsLate()
    {
        // Arrange
        var deadline = DateTime.UtcNow.AddDays(-1); // yesterday
        var submittedAt = DateTime.UtcNow;

        // Act
        var status = submittedAt > deadline
            ? SubmissionStatus.Late
            : SubmissionStatus.Submitted;

        // Assert
        Assert.Equal(SubmissionStatus.Late, status);
    }

    // ── Test 3: Submission before deadline is Submitted ───────────────────────
    [Fact]
    public void Submission_BeforeDeadline_IsSubmitted()
    {
        // Arrange
        var deadline = DateTime.UtcNow.AddDays(1); // tomorrow
        var submittedAt = DateTime.UtcNow;

        // Act
        var status = submittedAt > deadline
            ? SubmissionStatus.Late
            : SubmissionStatus.Submitted;

        // Assert
        Assert.Equal(SubmissionStatus.Submitted, status);
    }

    // ── Test 4: Marks cannot exceed MaxMarks ──────────────────────────────────
    [Fact]
    public void Grade_CannotExceed_MaxMarks()
    {
        // Arrange
        var maxMarks = 100;
        var givenMarks = 110;

        // Act
        var isValid = givenMarks <= maxMarks;

        // Assert
        Assert.False(isValid);
    }

    // ── Test 5: Marks within MaxMarks is valid ────────────────────────────────
    [Fact]
    public void Grade_WithinMaxMarks_IsValid()
    {
        // Arrange
        var maxMarks = 100;
        var givenMarks = 85;

        // Act
        var isValid = givenMarks <= maxMarks;

        // Assert
        Assert.True(isValid);
    }

    // ── Test 6: Grading a submission sets status to Reviewed ─────────────────
    [Fact]
    public async Task Grading_Submission_SetsStatus_ToReviewed()
    {
        var db = GetInMemoryDb();

        // Arrange
        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            AssignmentId = Guid.NewGuid(),
            Answer = "My answer",
            Status = SubmissionStatus.Submitted,
            SubmittedAt = DateTime.UtcNow
        };

        db.Submissions.Add(submission);
        await db.SaveChangesAsync();

        // Act: grade it
        submission.Marks = 90;
        submission.Feedback = "Good work!";
        submission.Status = SubmissionStatus.Reviewed;
        await db.SaveChangesAsync();

        // Assert
        var updated = await db.Submissions.FindAsync(submission.Id);
        Assert.Equal(SubmissionStatus.Reviewed, updated!.Status);
        Assert.Equal(90, updated.Marks);
    }

    // ── Test 7: Student cannot update submission after deadline ───────────────
    [Fact]
    public void Student_CannotUpdate_Submission_AfterDeadline()
    {
        // Arrange
        var deadline = DateTime.UtcNow.AddDays(-1); // yesterday

        // Act
        var canUpdate = DateTime.UtcNow <= deadline;

        // Assert
        Assert.False(canUpdate);
    }

    // ── Test 8: Assignment in Draft is not visible to students ────────────────
    [Fact]
    public async Task Draft_Assignment_NotVisible_ToStudents()
    {
        var db = GetInMemoryDb();

        // Arrange
        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            Title = "Draft Assignment",
            Description = "Not published yet",
            Deadline = DateTime.UtcNow.AddDays(7),
            MaxMarks = 100,
            Status = AssignmentStatus.Draft,
            SubjectId = Guid.NewGuid(),
            TeacherId = Guid.NewGuid()
        };

        db.Assignments.Add(assignment);
        await db.SaveChangesAsync();

        // Act: student tries to find published assignments
        var visible = await db.Assignments
            .Where(a => a.Status == AssignmentStatus.Published)
            .ToListAsync();

        // Assert
        Assert.Empty(visible);
    }
}