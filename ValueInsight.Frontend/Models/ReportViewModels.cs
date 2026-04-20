namespace ValueInsight.Frontend.Models;

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

public class TeamMemberProgressViewModel
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool HasCompletedAssessment { get; set; }
    public bool IsLeader { get; set; }
}

public class AssessmentQuestionAnswerViewModel
{
    public string QuestionId { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string ResponseText { get; set; } = string.Empty;
}

public class AssessmentHistoryItemViewModel
{
    public int AssessmentRunId { get; set; }
    public DateTime CompletedAtUtc { get; set; }
    public List<string> TopValues { get; set; } = new();
    public string PrimaryCategory { get; set; } = string.Empty;
    public List<CategoryScoreViewModel> CategoryProfile { get; set; } = new();
    public List<ReflectionInsightViewModel> ReflectionInsights { get; set; } = new();
    public List<AssessmentQuestionAnswerViewModel> ReflectionQuestions { get; set; } = new();
}

public class TeamHistoryItemViewModel
{
    public DateTime SnapshotDateUtc { get; set; }
    public List<string> TopValues { get; set; } = new();
    public double AlignmentScore { get; set; }
    public double PolarizationScore { get; set; }
    public double MaturityIndex { get; set; }
    public string CultureType { get; set; } = string.Empty;
    public int TeamSize { get; set; }
}

public class TeamHistorySummaryViewModel
{
    public bool HasHistory { get; set; }
    public double? AlignmentChange { get; set; }
    public double? PolarizationChange { get; set; }
    public double? MaturityChange { get; set; }
    public List<string> PreviousTopValues { get; set; } = new();
    public List<string> CurrentTopValues { get; set; } = new();
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
    public List<AssessmentHistoryItemViewModel> AssessmentHistory { get; set; } = new();
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

public class CoachingRequestViewModel
{
    public int UserId { get; set; }
    public int TeamId { get; set; }
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
    public List<string> GoalSuggestions { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}
public class CoachingPromptsPageViewModel
{
    public PersonalReportViewModel PersonalReport { get; set; } = new();
    public CoachingResponseViewModel? CoachingResult { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CurrentChallenge { get; set; }
    public string? CurrentGoal { get; set; }
    public string? LinkedValue { get; set; }
    public string? GoalRationale { get; set; }
    public int UserId { get; set; }
    public int TeamId { get; set; }
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
    public bool IsReady { get; set; }
    public bool AllowPartialReport { get; set; }
    public int CompletedMembers { get; set; }
    public int TotalMembers { get; set; }
    public string LeaderName { get; set; } = string.Empty;
    public List<CategoryScoreViewModel> CategoryProfile { get; set; } = new();
    public List<RankedValueViewModel> TopValues { get; set; } = new();
    public List<RankedValueViewModel> LowestValues { get; set; } = new();
    public List<RankedValueViewModel> ValueFrequency { get; set; } = new();
    public List<string> SharedCoreValues { get; set; } = new();
    public List<string> TensionFields { get; set; } = new();
    public List<TeamHistoryItemViewModel> History { get; set; } = new();
    public TeamHistorySummaryViewModel? HistorySummary { get; set; }
    public List<TeamMemberProgressViewModel> Members { get; set; } = new();
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

public class DashboardViewModel
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
    public List<AssessmentHistoryItemViewModel> AssessmentHistory { get; set; } = new();
}

public class TeamJoinPageViewModel
{
    public string InviteCode { get; set; } = string.Empty;
    public List<TeamOptionViewModel> Teams { get; set; } = new();
    public string? Message { get; set; }
}

public class TeamManagementViewModel
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public int? LeaderId { get; set; }
    public string? LeaderName { get; set; }
    public bool AllowPartialReport { get; set; }
    public bool TeamReportReady { get; set; }
    public bool CanManage { get; set; }
    public int CompletedMembers { get; set; }
    public int TotalMembers { get; set; }
    public List<TeamManagementMemberViewModel> Members { get; set; } = new();
    public List<PendingJoinRequestViewModel> PendingJoinRequests { get; set; } = new();
    public string? Message { get; set; }
}

public class TeamManagementMemberViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsLeader { get; set; }
    public bool HasCompletedAssessment { get; set; }
}

public class PendingJoinRequestViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAtUtc { get; set; }
}


public class ManageableTeamCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public string? LeaderName { get; set; }
    public int MembersCount { get; set; }
    public int CompletedMembers { get; set; }
    public int PendingJoinRequests { get; set; }
    public bool AllowPartialReport { get; set; }
    public bool TeamReportReady { get; set; }
}

public class TeamWorkspaceViewModel
{
    public bool IsAdmin { get; set; }
    public int? CurrentTeamId { get; set; }
    public string? JoinRequestStatus { get; set; }
    public string? JoinRequestTeamName { get; set; }
    public DateTime? JoinRequestedAtUtc { get; set; }
    public bool TeamReportReady { get; set; }
    public List<ManageableTeamCardViewModel> ManageableTeams { get; set; } = new();
}


public class TeamCoachingRequestViewModel
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string? CultureType { get; set; }
    public double AlignmentScore { get; set; }
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

public class TeamCoachingResponseViewModel
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
    public DateTime GeneratedAt { get; set; }
}

public class TeamCoachingPageViewModel
{
    public TeamReportViewModel TeamReport { get; set; } = new();
    public TeamCoachingResponseViewModel? CoachingResult { get; set; }
    public string? ErrorMessage { get; set; }
}
