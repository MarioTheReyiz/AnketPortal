namespace AnketPortal.API.Models
{
    public class QuestionOption : BaseEntity
    {
        public string OptionText { get; set; } = string.Empty;

        // Sıralama için (A şıkkı, B şıkkı vb. görünüm kontrolü)
        public int Order { get; set; }

        // Foreign Key
        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }
}