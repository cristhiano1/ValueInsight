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

public class TeamMemberProgressDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool HasCompletedAssessment { get; set; }
    public bool IsLeader { get; set; }
}

public class AssessmentQuestionAnswerDto
{
    public string QuestionId { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string ResponseText { get; set; } = string.Empty;
}

public class AssessmentHistoryItemDto
{
    public int AssessmentRunId { get; set; }
    public DateTime CompletedAtUtc { get; set; }
    public List<string> TopValues { get; set; } = new();
    public string PrimaryCategory { get; set; } = string.Empty;
    public List<CategoryScoreDto> CategoryProfile { get; set; } = new();
    public List<ReflectionInsightDto> ReflectionInsights { get; set; } = new();
    public List<AssessmentQuestionAnswerDto> ReflectionQuestions { get; set; } = new();
}

public class TeamHistoryItemDto
{
    public DateTime SnapshotDateUtc { get; set; }
    public List<string> TopValues { get; set; } = new();
    public double AlignmentScore { get; set; }
    public double PolarizationScore { get; set; }
    public double MaturityIndex { get; set; }
    public string CultureType { get; set; } = string.Empty;
    public int TeamSize { get; set; }
}

public class TeamHistorySummaryDto
{
    public bool HasHistory { get; set; }
    public double? AlignmentChange { get; set; }
    public double? PolarizationChange { get; set; }
    public double? MaturityChange { get; set; }
    public List<string> PreviousTopValues { get; set; } = new();
    public List<string> CurrentTopValues { get; set; } = new();
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
    public List<AssessmentHistoryItemDto> AssessmentHistory { get; set; } = new();
    public List<string> ValueConflicts { get; set; } = new();
    public double? CulturalFitScore { get; set; }
    public string CulturalFitLabel { get; set; } = string.Empty;
    public double? AlignmentWithTeamTop5 { get; set; }
    public string TeamCultureType { get; set; } = string.Empty;
    public List<string> CoachingQuestions { get; set; } = new();
    public bool IsAdmin { get; set; }
    public bool IsTeamLeader { get; set; }
    public bool TeamReportReady { get; set; }
    public bool AllowPartialReport { get; set; }
    public int TeamMembersCompleted { get; set; }
    public int TeamMembersTotal { get; set; }
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
    public bool IsReady { get; set; }
    public bool AllowPartialReport { get; set; }
    public int CompletedMembers { get; set; }
    public int TotalMembers { get; set; }
    public string LeaderName { get; set; } = string.Empty;
    public List<CategoryScoreDto> CategoryProfile { get; set; } = new();
    public List<RankedValueDto> TopValues { get; set; } = new();
    public List<RankedValueDto> LowestValues { get; set; } = new();
    public List<RankedValueDto> ValueFrequency { get; set; } = new();
    public List<string> SharedCoreValues { get; set; } = new();
    public List<string> TensionFields { get; set; } = new();
    public List<TeamHistoryItemDto> History { get; set; } = new();
    public TeamHistorySummaryDto? HistorySummary { get; set; }
    public List<TeamMemberProgressDto> Members { get; set; } = new();
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
    public bool IsAdmin { get; set; }
    public bool IsTeamLeader { get; set; }
    public bool TeamReportReady { get; set; }
    public bool AllowPartialReport { get; set; }
    public int TeamMembersCompleted { get; set; }
    public int TeamMembersTotal { get; set; }
    public string? TeamLeaderName { get; set; }
    public string? InviteCode { get; set; }
    public int PendingJoinRequests { get; set; }
    public List<AssessmentHistoryItemDto> AssessmentHistory { get; set; } = new();
}
