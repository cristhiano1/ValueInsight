namespace ValueInsight.Frontend.Models;
using System.Text.Json.Serialization;
public class CategoryScoreViewModel
{
    public string Category { get; set; } = string.Empty;
    public double Percentage { get; set; }
}

public class RankedValueViewModel
{
    public int ValueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Rank { get; set; }
}

public class ReflectionInsightViewModel
{
    public string ValueName { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public string Behavior { get; set; } = string.Empty;
    public string Friction { get; set; } = string.Empty;
}

public class PersonalReportViewModel
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int? TeamId { get; set; }
    public string? TeamName { get; set; }
    public List<RankedValueViewModel> TopValues { get; set; } = new();
    public List<CategoryScoreViewModel> CategoryProfile { get; set; } = new();
    public List<ReflectionInsightViewModel> ReflectionInsights { get; set; } = new();
    public double? CulturalFitScore { get; set; }
    public string CulturalFitLabel { get; set; } = string.Empty;
    public double? AlignmentWithTeamTop5 { get; set; }
    public string TeamCultureType { get; set; } = string.Empty;
    public List<string> CoachingQuestions { get; set; } = new();
}

public class CoachingRequestViewModel
{
    public int UserId { get; set; }
    public int TeamId { get; set; }
    public double AlignmentScore { get; set; }
    public List<string> DominantValues { get; set; } = new();
}

public class CoachingResponseViewModel
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
    public DateTime GeneratedAt { get; set; }
}

public class CoachingPromptsPageViewModel
{
    public PersonalReportViewModel PersonalReport { get; set; } = new();
    public CoachingResponseViewModel? CoachingResult { get; set; }
    public string? ErrorMessage { get; set; }
}


public class TeamReportViewModel
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int TeamSize { get; set; }
    public string CultureType { get; set; } = string.Empty;
    public double AlignmentScore { get; set; }
    public double PolarizationScore { get; set; }
    public double MaturityIndex { get; set; }

    public string? TeamInsight { get; set; }
    public List<CategoryScoreViewModel> CategoryProfile { get; set; } = new();
    public List<RankedValueViewModel> TopValues { get; set; } = new();
    public List<RankedValueViewModel> LowestValues { get; set; } = new();
    public List<string> SharedCoreValues { get; set; } = new();
    public List<string> TensionFields { get; set; } = new();
}

public class TopValueReflectionInputViewModel
{
    public int ValueId { get; set; }
    public string ValueName { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public string Behavior { get; set; } = string.Empty;
    public string Friction { get; set; } = string.Empty;
}

public class ConcretizeTopValuesViewModel
{
    public List<TopValueReflectionInputViewModel> TopValues { get; set; } = new();
}

public class AssessmentHistoryItemViewModel
{
    public int AssessmentRunId { get; set; }
    public DateTime CompletedAtUtc { get; set; }
    public List<string> TopValues { get; set; } = new();
    public string PrimaryCategory { get; set; } = string.Empty;
}


public class DashboardViewModel
{

    // 🔵 COACH DASHBOARD (NUEVO)
    public int TeamSize { get; set; }
    public int PendingAssessments { get; set; }

    public double? AlignmentScore { get; set; }
    public double? PolarizationScore { get; set; }
    public string UserName { get; set; } = string.Empty;
    [JsonPropertyName("completedAssessments")]
    public int AssessmentsCompleted { get; set; }
    public DateTime? LatestAssessmentCompletedAtUtc { get; set; }
    public int? TeamId { get; set; }
    public string? TeamName { get; set; }
    public string TeamCultureType { get; set; } = string.Empty;
    public double? CulturalFitScore { get; set; }
    public List<AssessmentHistoryItemViewModel> AssessmentHistory { get; set; } = new();
}