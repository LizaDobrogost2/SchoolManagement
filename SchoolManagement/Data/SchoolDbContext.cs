using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;

namespace SchoolManagement.Data;

/// <summary>
/// Database context for the School Management application.
/// Uses in-memory database for development and testing.
/// </summary>
public class SchoolDbContext : DbContext
{
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Students DbSet.
    /// </summary>
    public DbSet<Student> Students { get; set; } = null!;

    /// <summary>
    /// Gets or sets the SchoolClasses DbSet.
    /// </summary>
    public DbSet<SchoolClass> SchoolClasses { get; set; } = null!;

    /// <summary>
    /// Configures the model using Fluent API through separate configuration classes.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchoolDbContext).Assembly);
    }
}

