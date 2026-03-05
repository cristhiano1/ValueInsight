using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Data
{
    public class ValueInsightDbContext : DbContext
    {
        public ValueInsightDbContext(DbContextOptions<ValueInsightDbContext> options)
            : base(options)
        {
        }

        // 🔥 DbSets (FALTAN AHORA MISMO)
        public DbSet<User> Users => Set<User>();
        public DbSet<Value> Values => Set<Value>();
        public DbSet<UserValue> UserValues => Set<UserValue>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<CulturalFitResult> CulturalFitResults => Set<CulturalFitResult>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ValueInsightDbContext).Assembly);

            // Seed initial values
            modelBuilder.Entity<Value>().HasData(SeedData.Values);

            base.OnModelCreating(modelBuilder);
        }
    }
}