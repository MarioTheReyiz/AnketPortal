using Microsoft.AspNetCore.Identity;

namespace AnketPortal.API.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation Properties (Kullanıcının sistemdeki ayak izleri)
        // 1. Kullanıcının kendi oluşturduğu anketler
        public ICollection<Survey> CreatedSurveys { get; set; } = new List<Survey>();

        // 2. Kullanıcının çözdüğü anketlere verdiği cevaplar
        public ICollection<SurveyAnswer> Answers { get; set; } = new List<SurveyAnswer>();
    }
}