// backend/JCertPreBackend/Models/Quiz.cs
namespace JCertPreBackend.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int TimeLimit { get; set; } // Thời gian làm bài (phút)
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public Course? Course { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
        public List<TestResult> TestResults { get; set; } = new List<TestResult>();
    }
}
