using System.ComponentModel.DataAnnotations;

namespace ValueInsight.Backend.Dtos
{
    public class CoachingRequestDtos
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int TeamId { get; set; }

        [Range(0, 100)]
        public double AlignmentScore { get; set; }

        public List<string> DominantValues { get; set; } = new();
    }
}