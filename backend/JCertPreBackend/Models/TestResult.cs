// backend/JCertPreBackend/Models/TestResult.cs
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
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? UserAnswers { get; set; } // JSON string chứa đáp án của user

        // Navigation properties
        public User? User { get; set; }
        public Quiz? Quiz { get; set; }
    }
}
