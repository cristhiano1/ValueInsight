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

        public double? AlignmentWithTeamTop5 { get; set; }

        public string? TeamCultureType { get; set; }

        public string? CurrentChallenge { get; set; }

        public string? CurrentGoal { get; set; }

        public string? LinkedValue { get; set; }

        public string? GoalRationale { get; set; }

        public List<string> DominantValues { get; set; } = new();

        public List<string> TeamTopValues { get; set; } = new();

        public List<string> TeamLowestValues { get; set; } = new();

        public List<string> TeamTensionFields { get; set; } = new();

        public List<string> ReflectionInsights { get; set; } = new();
    }

    public class TeamCoachingRequestDtos
    {
        [Required]
        public int TeamId { get; set; }

        [Required]
        public string TeamName { get; set; } = string.Empty;

        public string? CultureType { get; set; }

        [Range(0, 100)]
        public double AlignmentScore { get; set; }

        [Range(0, 100)]
        public double PolarizationScore { get; set; }

        public double MaturityIndex { get; set; }

        public int TeamSize { get; set; }

        public int CompletedMembers { get; set; }

        public int TotalMembers { get; set; }

        public List<string> TopValues { get; set; } = new();

        public List<string> LowestValues { get; set; } = new();

        public List<string> SharedCoreValues { get; set; } = new();

        public List<string> TensionFields { get; set; } = new();
    }
}
