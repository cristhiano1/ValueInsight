namespace ValueInsight.Backend.Models;

public class AssessmentValueSelection
{
    public int Id { get; set; }

    public int AssessmentRunId { get; set; }

    public AssessmentRun AssessmentRun { get; set; } = null!; // 🔥 AÑADIR

    public int UserId { get; set; } // ya añadiste esto

    public int ValueId { get; set; }

    public int Rank { get; set; }

    public Value Value { get; set; } = null!;
}