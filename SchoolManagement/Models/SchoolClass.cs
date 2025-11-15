namespace SchoolManagement.Models;

public class SchoolClass
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string LeadingTeacher { get; set; } = null!;

    public ICollection<Student> Students { get; set; } = new List<Student>();
}

