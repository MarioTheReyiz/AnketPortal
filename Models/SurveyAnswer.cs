namespace AnketPortal.API.Models
{
    public class SurveyAnswer : BaseEntity
    {
        public string? TextAnswer { get; set; } // Metin tipindeki cevaplar için

        // Eğer çoktan seçmeli bir soruya cevap verildiyse, seçilen şıkkın ID'si
        public int? SelectedOptionId { get; set; }
        public QuestionOption? SelectedOption { get; set; }

        // Hangi ankete ve hangi soruya ait?
        public int SurveyId { get; set; }
        public Survey Survey { get; set; } = null!;

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        // Cevabı veren kullanıcı (Anonim de olabilir, Identity'den de gelebilir)
        // Eski hali: public string? UserId { get; set; }
        // Yeni hali:
        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!; // Cevabı veren kullanıcı
    }
}