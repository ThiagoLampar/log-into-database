using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LogIntoDatabase.Models
{
    public class ContosoContext : DbContext
    {
        public ContosoContext()
        {
        }

        public ContosoContext(DbContextOptions<ContosoContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<Log> Log { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(60)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.Before)
                    .HasMaxLength(3000)
                    .IsUnicode(false);

                entity.Property(e => e.After)
                    .HasMaxLength(3000)
                    .IsUnicode(false);

                entity.Property(e => e.Url)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasMaxLength(15)
                    .IsUnicode(false);
            });
        }
    }
}
