using System.ComponentModel.DataAnnotations;

namespace ValueInsight.Backend.Models
{
    public class CulturalFitResult
    {
        public int Id { get; set; }

        public int TeamId { get; set; }

        [Range(0, 100)]
        public double AlignmentScore { get; set; }

        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Team Team { get; set; } = default!;
    }
}