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
        public DbSet<ReflectionAnswer> ReflectionAnswers => Set<ReflectionAnswer>();
        public DbSet<AssessmentRun> AssessmentRuns => Set<AssessmentRun>();
        public DbSet<AssessmentValueSelection> AssessmentValueSelections => Set<AssessmentValueSelection>();
        public DbSet<AssessmentReflectionAnswer> AssessmentReflectionAnswers => Set<AssessmentReflectionAnswer>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ValueInsightDbContext).Assembly);

            // Seed initial values
            modelBuilder.Entity<Value>().HasData(SeedData.Values);
            modelBuilder.Entity<Team>().HasData(TeamSeedData.Teams);

            modelBuilder.Entity<AssessmentRun>()
                .HasOne(x => x.User)
                .WithMany(x => x.AssessmentRuns)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssessmentValueSelection>()
                .HasOne(x => x.AssessmentRun)
                .WithMany(x => x.ValueSelections)
                .HasForeignKey(x => x.AssessmentRunId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssessmentValueSelection>()
                .HasOne(x => x.Value)
                .WithMany()
                .HasForeignKey(x => x.ValueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AssessmentReflectionAnswer>()
                .HasOne(x => x.AssessmentRun)
                .WithMany(x => x.ReflectionAnswers)
                .HasForeignKey(x => x.AssessmentRunId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}