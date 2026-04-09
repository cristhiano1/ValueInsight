namespace ValueInsight.Backend.Models;

public class AssessmentRun
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // 🔥 AÑADIR ESTO
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public string Status { get; set; } = "Active";

    public ICollection<AssessmentValueSelection> ValueSelections { get; set; } = new List<AssessmentValueSelection>();
    public ICollection<AssessmentReflectionAnswer> ReflectionAnswers { get; set; } = new List<AssessmentReflectionAnswer>();
}