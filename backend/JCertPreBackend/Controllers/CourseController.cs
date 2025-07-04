// backend/JCertPreBackend/Controllers/CourseController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JCertPreBackend.Data;
using JCertPreBackend.Models;

namespace JCertPreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly JCertPreContext _context;

        public CourseController(JCertPreContext context)
        {
            _context = context;
            SeedCourseData();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses
                .Where(c => c.IsActive)
                .OrderBy(c => c.Id)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Quizzes)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (course == null)
                return NotFound(new { message = "Không tìm thấy khóa học" });

            return Ok(course);
        }

        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollCourse([FromBody] EnrollRequest request)
        {
            // Check if course exists
            var course = await _context.Courses.FindAsync(request.CourseId);
            if (course == null)
                return NotFound(new { message = "Không tìm thấy khóa học" });

            // Check if user exists
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng" });

            // Check if already enrolled
            var existingEnrollment = await _context.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == request.UserId && uc.CourseId == request.CourseId);

            if (existingEnrollment != null)
                return BadRequest(new { message = "Bạn đã đăng ký khóa học này rồi" });

            // Create enrollment
            var enrollment = new UserCourse
            {
                UserId = request.UserId,
                CourseId = request.CourseId,
                EnrolledAt = DateTime.Now
            };

            _context.UserCourses.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = $"Đăng ký khóa học '{course.Name}' thành công",
                enrollmentId = enrollment.Id
            });
        }

        [HttpGet("user/{userId}/enrolled")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserEnrolledCourses(int userId)
        {
            var enrolledCourses = await _context.UserCourses
                .Include(uc => uc.Course)
                .Where(uc => uc.UserId == userId)
                .Select(uc => new
                {
                    uc.Id,
                    uc.EnrolledAt,
                    uc.LastAccessedAt,
                    uc.Progress,
                    Course = new
                    {
                        uc.Course!.Id,
                        uc.Course.Name,
                        uc.Course.Type,
                        uc.Course.Description,
                        uc.Course.ImageUrl
                    }
                })
                .OrderByDescending(uc => uc.EnrolledAt)
                .ToListAsync();

            return Ok(enrolledCourses);
        }

        [HttpPost("progress")]
        public async Task<IActionResult> UpdateProgress([FromBody] UpdateProgressRequest request)
        {
            var enrollment = await _context.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == request.UserId && uc.CourseId == request.CourseId);

            if (enrollment == null)
                return NotFound(new { message = "Không tìm thấy thông tin đăng ký" });

            enrollment.Progress = request.Progress;
            enrollment.LastAccessedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật tiến độ thành công" });
        }

        private void SeedCourseData()
        {
            if (!_context.Courses.Any())
            {
                _context.Courses.AddRange(
                    new Course 
                    { 
                        Id = 1, 
                        Name = "JLPT N5", 
                        Type = "Video",
                        Description = "Khóa học luyện thi JLPT N5 với nội dung toàn diện bao gồm ngữ pháp, từ vựng, đọc hiểu và nghe hiểu.",
                        ImageUrl = "/images/jlpt-n5.jpg",
                        IsActive = true
                    },
                    new Course 
                    { 
                        Id = 2, 
                        Name = "NAT-TEST N4", 
                        Type = "Livestream",
                        Description = "Khóa học luyện thi NAT-TEST N4 với phương pháp học hiện đại và đề thi thử phong phú.",
                        ImageUrl = "/images/nat-test-n4.jpg",
                        IsActive = true
                    }
                );
                _context.SaveChanges();
            }
        }
    }

    public class EnrollRequest
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
    }

    public class UpdateProgressRequest
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public int Progress { get; set; } // 0-100
    }
}