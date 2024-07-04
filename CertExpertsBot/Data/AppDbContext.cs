using CertExpertsBot.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CertExpertsBot.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TechReg> TechRegs { get; set; } = null!;
        public DbSet<TNVEDCode> TNVEDCodes { get; set; }

        public AppDbContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var settings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("./appsettings.json"));
            optionsBuilder.UseSqlite(settings!.ConnectionStrings["MemoryConnection"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TNVEDCode>()
                .HasMany(c => c.TechRegs)
                .WithMany(tr => tr.TNVEDCodes)
                .UsingEntity("TNVEDCodeTechReg");
        }
    }
}
