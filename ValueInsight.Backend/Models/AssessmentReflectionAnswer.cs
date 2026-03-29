namespace ValueInsight.Backend.Models;

public class AssessmentReflectionAnswer
{
    public int Id { get; set; }
    public int AssessmentRunId { get; set; }
    public AssessmentRun AssessmentRun { get; set; } = null!;

    public string QuestionId { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string ResponseText { get; set; } = string.Empty;
}
