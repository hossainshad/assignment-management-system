namespace AssignmentSystem.Core.DTOs;

// Shape of the login request body
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// Shape of the login response
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

// Shape of the current user response
public class UserProfileResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
// Admin DTOs
public class CreateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;  // "Admin", "Teacher", "Student"
}

public class CreateClassRequest
{
    public string Name { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CreateSubjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public Guid TeacherId { get; set; }
}

public class EnrollStudentRequest
{
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
}
// Assignment DTOs
public class CreateAssignmentRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int MaxMarks { get; set; }
    public bool Publish { get; set; } = false;
    public Guid SubjectId { get; set; }
}
public class CreateSubmissionRequest
{
    public string Answer { get; set; } = string.Empty;
    public Guid AssignmentId { get; set; }
}

public class UpdateSubmissionRequest
{
    public string Answer { get; set; } = string.Empty;
}

public class GradeSubmissionRequest
{
    public int Marks { get; set; }
    public string? Feedback { get; set; }
}