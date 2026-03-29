using System.ComponentModel.DataAnnotations;

namespace ValueInsight.Frontend.Models
{
    public class ValueSelectionViewModel
    {
        public List<ValueCategoryGroupViewModel> Categories { get; set; } = new();

        public List<ValueOptionViewModel> AvailableValues { get; set; } = new();

        [Required]
        public List<int> SelectedValueIds { get; set; } = new();
    }
}