namespace SchoolManagement.Models;

public class Student
{
    public string StudentId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }

    public string? City { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }

    public int? SchoolClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }
}

