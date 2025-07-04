// backend/JCertPreBackend/Models/UserCourse.cs
namespace JCertPreBackend.Models
{
    public class UserCourse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.Now;
        public DateTime? LastAccessedAt { get; set; }
        public int Progress { get; set; } = 0; // Tiến độ học tập (%)

        // Navigation properties
        public User? User { get; set; }
        public Course? Course { get; set; }
    }
}
