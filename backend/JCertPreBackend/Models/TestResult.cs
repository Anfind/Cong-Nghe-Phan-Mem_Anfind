// backend/JCertPreBackend/Models/TestResult.cs
using System.ComponentModel.DataAnnotations;

namespace JCertPreBackend.Models
{
    public class TestResult
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int QuizId { get; set; }
        
        public int Score { get; set; } // Điểm số (0-100)
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public int TimeSpent { get; set; } // Thời gian làm bài (giây)
        
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime EndTime { get; set; } = DateTime.UtcNow;
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
        
        // JSON string chứa đáp án của user
        public string? UserAnswers { get; set; }
        
        // For MySQL JSON storage compatibility  
        public string Answers { get; set; } = "{}";
        
        public bool IsCompleted { get; set; } = false;
        public string? Notes { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Quiz? Quiz { get; set; }
    }
}
