namespace ValueInsight.Backend.Dtos
{
    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? TeamId { get; set; }
        public string? Role { get; set; }
    }
}
