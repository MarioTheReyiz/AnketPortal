namespace AnketPortal.API.DTOs
{
    public class AnswerSubmitDto
    {
        public int SurveyId { get; set; }
        public List<QuestionAnswerDto> Answers { get; set; } = new();
    }

    public class QuestionAnswerDto
    {
        public int QuestionId { get; set; }
        public string? TextAnswer { get; set; } // Metin soruları için
        public int? SelectedOptionId { get; set; } // Çoktan seçmeli sorular için
    }
}