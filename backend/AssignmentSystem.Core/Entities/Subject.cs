namespace AssignmentSystem.Core.Entities;

public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;   
    public string Code { get; set; } = string.Empty;   

    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;

    public Guid TeacherId { get; set; }
    public User Teacher { get; set; } = null!;         

    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}