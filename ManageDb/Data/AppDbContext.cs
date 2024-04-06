using ManageDb.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace ManageDb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<TechReg> TechRegs { get; set; } = null!;
        public DbSet<TNVEDCode> TNVEDCodes { get; set;} = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TNVEDCode>()
                .HasMany(c => c.TechRegs)
                .WithMany(tr => tr.TNVEDCodes)
                .UsingEntity("TNVEDCodeTechReg");
        }
    }
}
