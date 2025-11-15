using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Models;

namespace SchoolManagement.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Student entity.
/// </summary>
public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        // Primary key
        builder.HasKey(e => e.StudentId);

        // Required properties with constraints
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Surname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.DateOfBirth)
            .IsRequired();

        // Optional properties with constraints
        builder.Property(e => e.City)
            .HasMaxLength(100);

        builder.Property(e => e.Street)
            .HasMaxLength(200);

        builder.Property(e => e.PostalCode)
            .HasMaxLength(20);

        // Relationship configuration
        builder.HasOne(e => e.SchoolClass)
            .WithMany(e => e.Students)
            .HasForeignKey(e => e.SchoolClassId)
            .OnDelete(DeleteBehavior.SetNull);

        // Index for performance (foreign key)
        builder.HasIndex(e => e.SchoolClassId);
    }
}
