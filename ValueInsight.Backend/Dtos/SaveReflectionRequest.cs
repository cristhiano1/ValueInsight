namespace ValueInsight.Backend.Dtos
{
    public class SaveReflectionRequest
    {
        public List<ReflectionAnswerDto> Answers { get; set; } = new();
    }

    public class ReflectionAnswerDto
    {
        public string QuestionId { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public string ResponseText { get; set; } = string.Empty;
    }
}
