namespace AssignmentSystem.Core.Entities;

public enum SubmissionStatus
{
    Submitted,
    Late,
    Reviewed
}

public class Submission : BaseEntity
{
    public string Answer { get; set; } = string.Empty;
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;
    public int? Marks { get; set; }
    public string? Feedback { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Guid AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;

    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;
}