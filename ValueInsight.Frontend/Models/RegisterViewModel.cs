
using System.ComponentModel.DataAnnotations;

namespace ValueInsight.Frontend.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Team")]
        public int? TeamId { get; set; }

        public List<TeamOptionViewModel> Teams { get; set; } = new();
    }
}
