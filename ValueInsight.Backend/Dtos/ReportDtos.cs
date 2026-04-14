namespace ValueInsight.Backend.Dtos;

public class CategoryScoreDto
{
    public string Category { get; set; } = string.Empty;
    public double Percentage { get; set; }
}

public class RankedValueDto
{
    public int ValueId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Rank { get; set; }
}

public class ReflectionInsightDto
{
    public string ValueName { get; set; } = string.Empty;
    public string Meaning { get; set; } = string.Empty;
    public string Behavior { get; set; } = string.Empty;
    public string Friction { get; set; } = string.Empty;
}

public class PersonalReportDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int? TeamId { get; set; }
    public string? TeamName { get; set; }
    public List<RankedValueDto> TopValues { get; set; } = new();
    public List<CategoryScoreDto> CategoryProfile { get; set; } = new();
    public List<ReflectionInsightDto> ReflectionInsights { get; set; } = new();
    public double? CulturalFitScore { get; set; }
    public string CulturalFitLabel { get; set; } = string.Empty;
    public double? AlignmentWithTeamTop5 { get; set; }
    public string TeamCultureType { get; set; } = string.Empty;
    public List<string> CoachingQuestions { get; set; } = new();
}

public class TeamReportDto
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int TeamSize { get; set; }
    public string CultureType { get; set; } = string.Empty;

    public double AlignmentScore { get; set; }
    public double PolarizationScore { get; set; }
    public double MaturityIndex { get; set; }

    // ✅ NUEVO
    public double DispersionScore { get; set; }
    public double CompletionRate { get; set; }

    public List<CategoryScoreDto> CategoryProfile { get; set; } = new();
    public List<RankedValueDto> TopValues { get; set; } = new();
    public List<RankedValueDto> LowestValues { get; set; } = new();

    public List<string> SharedCoreValues { get; set; } = new();
    public List<string> TensionFields { get; set; } = new();
}

public class AssessmentHistoryItemDto
{
    public int AssessmentRunId { get; set; }
    public DateTime CompletedAtUtc { get; set; }
    public List<string> TopValues { get; set; } = new();
    public string PrimaryCategory { get; set; } = string.Empty;
}
public class DashboardDto
{
    public string UserName { get; set; } = string.Empty;
    public int AssessmentsCompleted { get; set; }
    public DateTime? LatestAssessmentCompletedAtUtc { get; set; }
    public int? TeamId { get; set; }
    public string? TeamName { get; set; }
    public string TeamCultureType { get; set; } = string.Empty;
    public double? CulturalFitScore { get; set; }
    public List<AssessmentHistoryItemDto> AssessmentHistory { get; set; } = new();
}
