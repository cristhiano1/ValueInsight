namespace ValueInsight.Backend.Models;

public class AssessmentValueSelection
{
    public int Id { get; set; }
    public int AssessmentRunId { get; set; }
    public AssessmentRun AssessmentRun { get; set; } = null!;

    public int ValueId { get; set; }
    public Value Value { get; set; } = null!;

    public int Rank { get; set; }
}