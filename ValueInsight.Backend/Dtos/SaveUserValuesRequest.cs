namespace ValueInsight.Backend.Dtos
{
    public class SaveUserValuesRequest
    {
        public List<int> OrderedValueIds { get; set; } = new();
    }
}
