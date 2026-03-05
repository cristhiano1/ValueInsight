namespace ValueInsight.Backend.Dtos
{
    public class CoachingResponseDtos
    {
        public int UserId { get; set; }

        public int TeamId { get; set; }

        public double AlignmentScore { get; set; }

        public string AlignmentLevel { get; set; } = string.Empty;

        public string AICoachingAdvice { get; set; } = string.Empty;

        public bool AIEnhanced { get; set; }

        public List<string> Strengths { get; set; } = new();

        public List<string> DevelopmentAreas { get; set; } = new();

        public List<string> CoachingRecommendations { get; set; } = new();

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}