namespace ValueInsight.Backend.Models
{
    public class ReflectionAnswer
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string QuestionId { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string ResponseText { get; set; } = string.Empty;
    }
}
