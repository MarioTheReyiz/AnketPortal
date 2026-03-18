using AnketPortal.API.Models.Enums;

namespace AnketPortal.API.Models
{
    public class Question : BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; } // Enum kullanımı
        public bool IsRequired { get; set; } = true; // Soru zorunlu mu?

        // Foreign Key
        public int SurveyId { get; set; }
        public Survey Survey { get; set; } = null!;

        // Navigation Property - Eğer soru çoktan seçmeliyse seçenekleri olacak
        public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }
}