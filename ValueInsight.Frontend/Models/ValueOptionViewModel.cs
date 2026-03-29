namespace ValueInsight.Frontend.Models
{
    public class ValueOptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? ShortDefinition { get; set; }
    }
}
