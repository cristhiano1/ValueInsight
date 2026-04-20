namespace ValueInsight.Frontend.Models
{
    public class TeamOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LeaderName { get; set; }
        public string? InviteCode { get; set; }
    }
}
