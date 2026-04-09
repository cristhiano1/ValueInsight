namespace ValueInsight.Backend.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty; // ✔ FIX

        public string Role { get; set; } = "User";

        public int? TeamId { get; set; }

        public Team? Team { get; set; } = null!;

        public ICollection<UserValue> UserValues { get; set; } = new List<UserValue>();

        public ICollection<AssessmentRun> AssessmentRuns { get; set; } = new List<AssessmentRun>();
    }
}