namespace ValueInsight.Backend.Dtos
{
    public class TeamInsightDtos
    {
        public string CultureType { get; set; }
        public double AlignmentScore { get; set; }
        public double PolarizationScore { get; set; }
        public double MaturityIndex { get; set; }
        public List<string> TopCategories { get; set; } = new();
        public List<string> LowCategories { get; set; } = new();
    }
}
