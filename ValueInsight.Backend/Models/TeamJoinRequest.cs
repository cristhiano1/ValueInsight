namespace ValueInsight.Backend.Models;

public class TeamJoinRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;
    public string Status { get; set; } = "Pending";
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public int? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
}