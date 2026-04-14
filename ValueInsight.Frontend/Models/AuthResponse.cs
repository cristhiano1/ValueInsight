namespace ValueInsight.Frontend.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string? Role { get; set; }
        public int? TeamId { get; set; }
        public string? Name { get; set; }
    }
}