namespace ValueInsight.Backend.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int TeamId { get; set; }

        public Team Team { get; set; } = null!;

       
        public ICollection<UserValue> UserValues { get; set; } = new List<UserValue>();
    }
}