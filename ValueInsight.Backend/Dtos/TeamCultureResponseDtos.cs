namespace ValueInsight.Backend.Dtos
{
    public class TeamCultureResponseDtos
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; } = string.Empty;

        public List<string> DominantValues { get; set; } = new();
    }
}