using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ValueInsight.Backend.Models;

namespace ValueInsight.Backend.Data.Configurations
{
    public class UserValueMap : IEntityTypeConfiguration<UserValue>
    {
        public void Configure(EntityTypeBuilder<UserValue> builder)
        {
            // Composite PK (VERY IMPORTANT)
            builder.HasKey(uv => new { uv.UserId, uv.ValueId });

            // Ranking / Priority
            builder.Property(uv => uv.Rank)
                   .IsRequired();

            // Relationship: UserValue → User
            builder.HasOne(uv => uv.User)
                   .WithMany(u => u.UserValues)
                   .HasForeignKey(uv => uv.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship: UserValue → Value
            builder.HasOne(uv => uv.Value)
                   .WithMany(v => v.UserValues)
                   .HasForeignKey(uv => uv.ValueId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}