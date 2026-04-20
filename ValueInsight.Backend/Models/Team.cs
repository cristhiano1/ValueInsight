using System.ComponentModel.DataAnnotations;

namespace ValueInsight.Backend.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; set; } = default!;

        [MaxLength(50)]
        public string InviteCode { get; set; } = string.Empty;

        public int? LeaderId { get; set; }
        public User? Leader { get; set; }

        public bool AllowPartialReport { get; set; }

        public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
        public ICollection<CulturalFitResult> CulturalFitResults { get; set; } = new List<CulturalFitResult>();
        public ICollection<TeamJoinRequest> JoinRequests { get; set; } = new List<TeamJoinRequest>();
    }
}
