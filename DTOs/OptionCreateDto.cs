namespace AnketPortal.API.DTOs
{
    public class OptionCreateDto
    {
        public int QuestionId { get; set; } // Hangi soruya şık ekliyoruz?
        public string OptionText { get; set; } = string.Empty; // Şıkkın metni (Örn: "Evet")
        public int Order { get; set; } // Şıkkın sırası (1=A, 2=B gibi)
    }
}   