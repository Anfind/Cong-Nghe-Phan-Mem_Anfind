// backend/JCertPreBackend/Controllers/CourseController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using JCertPreBackend.Data;
using JCertPreBackend.Models;
using JCertPreBackend.DTOs;
using System.Security.Claims;

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
        }

        // GET: api/course - Lấy danh sách khóa học (public)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetCourses()
        {
            var courses = await _context.Courses
                .Where(c => c.IsActive && c.IsPublished)
                .Include(c => c.UserCourses)
                .Include(c => c.Quizzes)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    VideoUrl = c.VideoUrl,
                    Price = c.Price,
                    IsActive = c.IsActive,
                    IsPublished = c.IsPublished,
                    ViewCount = c.ViewCount,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    EnrolledUsersCount = c.UserCourses.Count,
                    QuizzesCount = c.Quizzes.Count
                })
                .ToListAsync();

            return Ok(courses);
        }

        // GET: api/course/admin - Lấy tất cả khóa học cho admin
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetAllCoursesForAdmin()
        {
            var courses = await _context.Courses
                .Include(c => c.UserCourses)
                .Include(c => c.Quizzes)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CourseResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    VideoUrl = c.VideoUrl,
                    Price = c.Price,
                    IsActive = c.IsActive,
                    IsPublished = c.IsPublished,
                    ViewCount = c.ViewCount,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    EnrolledUsersCount = c.UserCourses.Count,
                    QuizzesCount = c.Quizzes.Count
                })
                .ToListAsync();

            return Ok(courses);
        }

        // GET: api/course/{id} - Lấy chi tiết khóa học
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseResponseDto>> GetCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.UserCourses)
                .Include(c => c.Quizzes)
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            if (course == null)
                return NotFound(new { message = "Không tìm thấy khóa học" });

            // Increment view count
            course.ViewCount++;
            await _context.SaveChangesAsync();

            var result = new CourseResponseDto
            {
                Id = course.Id,
                Name = course.Name,
                Type = course.Type,
                Description = course.Description,
                ImageUrl = course.ImageUrl,
                VideoUrl = course.VideoUrl,
                Price = course.Price,
                IsActive = course.IsActive,
                IsPublished = course.IsPublished,
                ViewCount = course.ViewCount,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt,
                EnrolledUsersCount = course.UserCourses.Count,
                QuizzesCount = course.Quizzes.Count
            };

            return Ok(result);
        }

        // POST: api/course/create - Tạo khóa học mới (Admin only)
        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CourseResponseDto>> CreateCourse([FromBody] CourseCreateDto courseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = new Course
            {
                Name = courseDto.Name,
                Type = courseDto.Type,
                Description = courseDto.Description,
                ImageUrl = courseDto.ImageUrl,
                VideoUrl = courseDto.VideoUrl,
                Price = courseDto.Price,
                IsPublished = courseDto.IsPublished,
                IsActive = true,
                ViewCount = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            var result = new CourseResponseDto
            {
                Id = course.Id,
                Name = course.Name,
                Type = course.Type,
                Description = course.Description,
                ImageUrl = course.ImageUrl,
                VideoUrl = course.VideoUrl,
                Price = course.Price,
                IsActive = course.IsActive,
                IsPublished = course.IsPublished,
                ViewCount = course.ViewCount,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt,
                EnrolledUsersCount = 0,
                QuizzesCount = 0
            };

            return CreatedAtAction(nameof(GetCourse), new { id = course.Id }, result);
        }

        // PUT: api/course/{id}/update - Cập nhật khóa học (Admin only)
        [HttpPut("{id}/update")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CourseResponseDto>> UpdateCourse(int id, [FromBody] CourseUpdateDto courseDto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new { message = "Không tìm thấy khóa học" });

            // Update only provided fields
            if (!string.IsNullOrEmpty(courseDto.Name))
                course.Name = courseDto.Name;
            if (!string.IsNullOrEmpty(courseDto.Type))
                course.Type = courseDto.Type;
            if (courseDto.Description != null)
                course.Description = courseDto.Description;
            if (courseDto.ImageUrl != null)
                course.ImageUrl = courseDto.ImageUrl;
            if (courseDto.VideoUrl != null)
                course.VideoUrl = courseDto.VideoUrl;
            if (courseDto.Price.HasValue)
                course.Price = courseDto.Price.Value;
            if (courseDto.IsPublished.HasValue)
                course.IsPublished = courseDto.IsPublished.Value;
            if (courseDto.IsActive.HasValue)
                course.IsActive = courseDto.IsActive.Value;

            course.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var enrolledCount = await _context.UserCourses.CountAsync(uc => uc.CourseId == id);
            var quizzesCount = await _context.Quizzes.CountAsync(q => q.CourseId == id);

            var result = new CourseResponseDto
            {
                Id = course.Id,
                Name = course.Name,
                Type = course.Type,
                Description = course.Description,
                ImageUrl = course.ImageUrl,
                VideoUrl = course.VideoUrl,
                Price = course.Price,
                IsActive = course.IsActive,
                IsPublished = course.IsPublished,
                ViewCount = course.ViewCount,
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt,
                EnrolledUsersCount = enrolledCount,
                QuizzesCount = quizzesCount
            };

            return Ok(result);
        }

        // DELETE: api/course/{id}/delete - Xóa khóa học (Admin only)
        [HttpDelete("{id}/delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new { message = "Không tìm thấy khóa học" });

            // Soft delete - chỉ set IsActive = false
            course.IsActive = false;
            course.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa khóa học thành công" });
        }

        // POST: api/course/{id}/publish - Publish/Unpublish khóa học (Admin only)
        [HttpPost("{id}/publish")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TogglePublishCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return NotFound(new { message = "Không tìm thấy khóa học" });

            course.IsPublished = !course.IsPublished;
            course.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var status = course.IsPublished ? "published" : "unpublished";
            return Ok(new { 
                message = $"Khóa học đã được {status}",
                isPublished = course.IsPublished
            });
        }

        // POST: api/course/enroll - Đăng ký khóa học
        [HttpPost("enroll")]
        [Authorize]
        public async Task<IActionResult> EnrollCourse([FromBody] EnrollRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Check if course exists
            var course = await _context.Courses.FindAsync(request.CourseId);
            if (course == null)
                return NotFound(new { message = "Không tìm thấy khóa học" });

            if (!course.IsActive || !course.IsPublished)
                return BadRequest(new { message = "Khóa học không khả dụng" });

            // Check if already enrolled
            var existingEnrollment = await _context.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == request.CourseId);

            if (existingEnrollment != null)
                return BadRequest(new { message = "Bạn đã đăng ký khóa học này rồi" });

            // Create enrollment
            var enrollment = new UserCourse
            {
                UserId = userId,
                CourseId = request.CourseId,
                EnrolledAt = DateTime.UtcNow,
                Progress = 0
            };

            _context.UserCourses.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = $"Đăng ký khóa học '{course.Name}' thành công",
                enrollmentId = enrollment.Id
            });
        }

        // GET: api/course/user/{userId}/enrolled - Lấy khóa học đã đăng ký
        [HttpGet("user/{userId}/enrolled")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetUserEnrolledCourses(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Only allow user to see their own enrollments or admin to see any
            if (currentUserId != userId && userRole != "Admin")
                return Forbid();

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
                        uc.Course.ImageUrl,
                        uc.Course.VideoUrl
                    }
                })
                .OrderByDescending(uc => uc.EnrolledAt)
                .ToListAsync();

            return Ok(enrolledCourses);
        }

        // POST: api/course/progress - Cập nhật tiến độ học
        [HttpPost("progress")]
        [Authorize]
        public async Task<IActionResult> UpdateProgress([FromBody] UpdateProgressRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var enrollment = await _context.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == request.CourseId);

            if (enrollment == null)
                return NotFound(new { message = "Không tìm thấy thông tin đăng ký" });

            enrollment.Progress = Math.Max(0, Math.Min(100, request.Progress)); // Ensure 0-100
            enrollment.LastAccessedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Cập nhật tiến độ thành công",
                progress = enrollment.Progress
            });
        }
    }

    // Request DTOs
    public class EnrollRequest
    {
        public int CourseId { get; set; }
    }

    public class UpdateProgressRequest
    {
        public int CourseId { get; set; }
        public int Progress { get; set; } // 0-100
    }
}
