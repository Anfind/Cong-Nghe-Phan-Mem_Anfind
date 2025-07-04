// backend/JCertPreBackend/Models/Question.cs
namespace JCertPreBackend.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string? Text { get; set; }
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public string? Option4 { get; set; }
        public int CorrectAnswer { get; set; } // 0-3
        public string? Explanation { get; set; }
        public int Order { get; set; }

        // Navigation properties
        public Quiz? Quiz { get; set; }
    }
}
