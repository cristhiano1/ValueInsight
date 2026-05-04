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

        public List<string> GoalSuggestions { get; set; } = new();

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public List<string> ReflectionQuestions { get; set; } = new();
    }

    public class TeamCoachingResponseDtos
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; } = string.Empty;

        public string CultureType { get; set; } = string.Empty;

        public double AlignmentScore { get; set; }

        public double PolarizationScore { get; set; }

        public double MaturityIndex { get; set; }

        public bool AIEnhanced { get; set; }

        public string AICoachingAdvice { get; set; } = string.Empty;

        public List<string> Strengths { get; set; } = new();

        public List<string> Risks { get; set; } = new();

        public List<string> LeadershipAdvice { get; set; } = new();

        public List<string> SuggestedInterventions { get; set; } = new();

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
