using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Models;

namespace SchoolManagement.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the SchoolClass entity.
/// </summary>
public class SchoolClassConfiguration : IEntityTypeConfiguration<SchoolClass>
{
    public void Configure(EntityTypeBuilder<SchoolClass> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);

        // Required properties with constraints
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.LeadingTeacher)
            .IsRequired()
            .HasMaxLength(100);

        // Index for performance (class name lookup)
        builder.HasIndex(e => e.Name);
    }
}
