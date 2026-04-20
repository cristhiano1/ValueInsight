namespace ValueInsight.Backend.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }

        public ICollection<UserValue> UserValues { get; set; } = new List<UserValue>();
        public ICollection<AssessmentRun> AssessmentRuns { get; set; } = new List<AssessmentRun>();
        public ICollection<TeamJoinRequest> JoinRequests { get; set; } = new List<TeamJoinRequest>();
        public ICollection<Team> LedTeams { get; set; } = new List<Team>();
        public ICollection<TeamMember> TeamMemberships { get; set; } = new List<TeamMember>();
    }
}
