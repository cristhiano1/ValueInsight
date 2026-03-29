using System.ComponentModel.DataAnnotations;

namespace ValueInsight.Frontend.Models
{
    public class RankTopThreeViewModel
    {
        public List<ValueOptionViewModel> TopFiveValues { get; set; } = new();

        [Required]
        public int FirstValueId { get; set; }

        [Required]
        public int SecondValueId { get; set; }

        [Required]
        public int ThirdValueId { get; set; }
    }
}