using Microsoft.EntityFrameworkCore;
using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Data
{
    public class ValueInsightDbContext : DbContext
    {
        public ValueInsightDbContext(DbContextOptions<ValueInsightDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Value> Values => Set<Value>();
        public DbSet<UserValue> UserValues => Set<UserValue>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<CulturalFitResult> CulturalFitResults => Set<CulturalFitResult>();
        public DbSet<ReflectionAnswer> ReflectionAnswers => Set<ReflectionAnswer>();
        public DbSet<AssessmentRun> AssessmentRuns => Set<AssessmentRun>();
        public DbSet<AssessmentValueSelection> AssessmentValueSelections => Set<AssessmentValueSelection>();
        public DbSet<AssessmentReflectionAnswer> AssessmentReflectionAnswers => Set<AssessmentReflectionAnswer>();
        public DbSet<TeamJoinRequest> TeamJoinRequests => Set<TeamJoinRequest>();
        public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ValueInsightDbContext).Assembly);

            modelBuilder.Entity<Value>().HasData(SeedData.Values);
            modelBuilder.Entity<Team>().HasData(TeamSeedData.Teams);

            modelBuilder.Entity<Team>()
                .HasOne(x => x.Leader)
                .WithMany(x => x.LedTeams)
                .HasForeignKey(x => x.LeaderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Team>()
                .HasIndex(x => x.InviteCode)
                .IsUnique(true);

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

            modelBuilder.Entity<TeamJoinRequest>()
                .HasOne(x => x.User)
                .WithMany(x => x.JoinRequests)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamJoinRequest>()
                .HasOne(x => x.Team)
                .WithMany(x => x.JoinRequests)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMember>()
                .HasOne(x => x.User)
                .WithMany(x => x.TeamMemberships)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMember>()
                .HasOne(x => x.Team)
                .WithMany(x => x.Members)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMember>()
                .HasIndex(x => new { x.TeamId, x.UserId })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
