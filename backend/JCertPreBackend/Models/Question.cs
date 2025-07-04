// backend/JCertPreBackend/Models/Question.cs
using System.ComponentModel.DataAnnotations;

namespace JCertPreBackend.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        
        [Required]
        public string Text { get; set; } = string.Empty;
        
        public string? Option1 { get; set; }
        public string? Option2 { get; set; }
        public string? Option3 { get; set; }
        public string? Option4 { get; set; }
        
        // For MySQL JSON storage compatibility
        public string Options { get; set; } = "[]";
        
        [Required]
        public string CorrectAnswer { get; set; } = string.Empty;
        
        public string? Explanation { get; set; }
        public int Order { get; set; }
        public int Points { get; set; } = 1;

        // Navigation properties
        public Quiz? Quiz { get; set; }
    }
}
