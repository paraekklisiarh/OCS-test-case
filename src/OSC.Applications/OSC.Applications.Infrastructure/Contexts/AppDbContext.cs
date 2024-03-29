using Microsoft.EntityFrameworkCore;
using OSC.Applications.Domain.Entitites;

namespace OSC.Applications.Infrastructure.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Application?> Applications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>().HasKey(a => a.Id);
        modelBuilder.Entity<Application>().HasIndex(a => a.AuthorId).IsUnique();

        modelBuilder.Entity<Application>().Property(a => a.Id).ValueGeneratedOnAdd();
        
        // Enums Activities, ApplicationStatus должны храниться в виде строк
        modelBuilder.Entity<Application>().Property(a => a.Activity).HasConversion<string>();
        modelBuilder.Entity<Application>().Property(a => a.Status).HasConversion<string>();
    }
}