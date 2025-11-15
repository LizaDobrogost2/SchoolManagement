using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data.Configurations;
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
    /// This follows the Single Responsibility Principle - each configuration class
    /// handles one entity's configuration.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        // This automatically finds and applies all IEntityTypeConfiguration implementations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchoolDbContext).Assembly);

        // Alternative explicit approach (if you prefer explicit over convention):
        // modelBuilder.ApplyConfiguration(new StudentConfiguration());
        // modelBuilder.ApplyConfiguration(new SchoolClassConfiguration());
    }
}

