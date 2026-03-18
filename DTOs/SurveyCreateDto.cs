namespace AnketPortal.API.DTOs
{
    public class SurveyCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EndDate { get; set; }
        // Eklenirken ID veya oluşturulma tarihi istemiyoruz, sadece gerekli veriler!
    }
}