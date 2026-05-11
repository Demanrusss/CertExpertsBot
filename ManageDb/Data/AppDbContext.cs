using ManageDb.Models;
using Microsoft.EntityFrameworkCore;

namespace ManageDb.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<TechRegModel> TechRegs { get; set; } = null!;
    public DbSet<TNVEDCodeModel> TNVEDCodes { get; set;} = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TNVEDCodeModel>()
            .HasMany(c => c.TechRegs)
            .WithMany(tr => tr.TNVEDCodes)
            .UsingEntity("TNVEDCodeTechReg");
    }
}