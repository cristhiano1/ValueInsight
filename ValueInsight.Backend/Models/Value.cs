namespace ValueInsight.Backend.Models
{
    public class Value
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public ICollection<UserValue> UserValues { get; set; } = new List<UserValue>();
    }
}