namespace ValueInsight.Frontend.Models
{
    public class ValueCategoryGroupViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public List<ValueOptionViewModel> Values { get; set; } = new();
    }
}
