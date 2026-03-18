namespace AnketPortal.API.Models
{
    public class Survey : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EndDate { get; set; } // Anketin bitiş tarihi

        // Creator (AppUser ile ilişki - Identity entegrasyonu sonrası AppUser tipiyle güncellenebilir)
        // Eski hali: public string CreatorUserId { get; set; }
        // Yeni hali:
        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!; // Anketi oluşturan kullanıcı

        // Navigation Properties
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<SurveyAnswer> Answers { get; set; } = new List<SurveyAnswer>();
    }
}