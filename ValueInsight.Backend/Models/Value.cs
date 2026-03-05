using System.ComponentModel.DataAnnotations;

namespace ValueInsight.Backend.Models
{
    public class Value
    {
        public int Id { get; set; }

        [Required, MaxLength(80)]
        public string Name { get; set; } = default!;

        public ValueCategory Category { get; set; }

        [MaxLength(240)]
        public string? ShortDefinition { get; set; }

        [MaxLength(240)]
        public string? BehaviorIndicator { get; set; }

        public ICollection<UserValue> UserValues { get; set; } = new List<UserValue>();
    }
}