namespace ValueInsight.Backend.Models;

public class TeamMember
{
    public int Id { get; set; }

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public bool HasCompletedAssessment { get; set; }
    public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;
}
