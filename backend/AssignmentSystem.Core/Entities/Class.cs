namespace AssignmentSystem.Core.Entities;

public class Class : BaseEntity
{
    public string Name { get; set; } = string.Empty;      
    public string Section { get; set; } = string.Empty;    
    public string? Description { get; set; }

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<ClassEnrollment> Enrollments { get; set; } = new List<ClassEnrollment>();
}