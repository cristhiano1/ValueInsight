using System.ComponentModel.DataAnnotations;

namespace ValueInsight.Backend.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = default!;

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();

        public ICollection<CulturalFitResult> CulturalFitResults { get; set; } = new List<CulturalFitResult>();
    }
}