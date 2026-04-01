namespace ValueInsight.Backend.Models
{
    public class UserValue
    {
        public int UserId { get; set; }

        public int ValueId { get; set; }

        public int Rank { get; set; }

        public User User { get; set; } = default!;

        public Value Value { get; set; } = default!;
    }
}

