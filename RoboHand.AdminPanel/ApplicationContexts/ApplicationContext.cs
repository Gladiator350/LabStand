using Microsoft.EntityFrameworkCore;
using WpfApp23.Models;

namespace WpfApp23.ApplicationContexts;

public class ApplicationContext : DbContext
{
    public DbSet<Variant> Variants => Set<Variant>();
    public DbSet<Command> Commands => Set<Command>();

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Command>().Navigation(p => p.Angles).AutoInclude();
    }
}