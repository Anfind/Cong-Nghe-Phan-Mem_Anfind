// backend/JCertPreBackend/Controllers/AdminController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using JCertPreBackend.Data;
using JCertPreBackend.Models;
using JCertPreBackend.DTOs;
using JCertPreBackend.Services;
using System.Security.Claims;

namespace JCertPreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly JCertPreContext _context;
        private readonly IJwtService _jwtService;

        public AdminController(JCertPreContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST: api/admin/login
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] AdminLoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == loginDto.Username && a.IsActive);

            if (admin == null || admin.Password != loginDto.Password)
                return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không đúng" });

            // Update last login
            admin.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = _jwtService.GenerateAdminToken(admin);

            var adminResponse = new AdminResponseDto
            {
                Id = admin.Id,
                Username = admin.Username,
                FullName = admin.FullName,
                Email = admin.Email,
                Role = admin.Role,
                IsActive = admin.IsActive,
                IsSuper = admin.IsSuper,
                CreatedAt = admin.CreatedAt,
                LastLoginAt = admin.LastLoginAt,
                CanManageCourses = admin.CanManageCourses,
                CanManageUsers = admin.CanManageUsers,
                CanManageQuizzes = admin.CanManageQuizzes,
                CanViewReports = admin.CanViewReports
            };

            return Ok(new
            {
                message = "Đăng nhập thành công",
                token,
                admin = adminResponse
            });
        }

        // GET: api/admin/dashboard/stats
        [HttpGet("dashboard/stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            var totalUsers = await _context.Users.CountAsync(u => u.IsActive);
            var totalCourses = await _context.Courses.CountAsync(c => c.IsActive);
            var totalQuizzes = await _context.Quizzes.CountAsync();
            var totalTestResults = await _context.TestResults.CountAsync();
            var activeUsers = await _context.Users.CountAsync(u => u.IsActive && u.LastLoginAt > DateTime.UtcNow.AddDays(-30));
            var publishedCourses = await _context.Courses.CountAsync(c => c.IsActive && c.IsPublished);

            // Calculate total revenue (if courses have prices)
            var totalRevenue = await _context.UserCourses
                .Include(uc => uc.Course)
                .SumAsync(uc => uc.Course!.Price);

            // Top courses by enrollment
            var topCourses = await _context.Courses
                .Include(c => c.UserCourses)
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.UserCourses.Count)
                .Take(5)
                .Select(c => new CourseStatsDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    EnrolledUsers = c.UserCourses.Count,
                    ViewCount = c.ViewCount,
                    Revenue = c.UserCourses.Count * c.Price
                })
                .ToListAsync();

            // Recent user activities (simplified)
            var recentActivities = await _context.TestResults
                .Include(tr => tr.User)
                .OrderByDescending(tr => tr.CompletedAt)
                .Take(10)
                .Select(tr => new UserActivityDto
                {
                    UserId = tr.UserId,
                    Username = tr.User!.Username,
                    Activity = $"Hoàn thành bài thi với điểm {tr.Score}",
                    Timestamp = tr.CompletedAt
                })
                .ToListAsync();

            var stats = new DashboardStatsDto
            {
                TotalUsers = totalUsers,
                TotalCourses = totalCourses,
                TotalQuizzes = totalQuizzes,
                TotalTestResults = totalTestResults,
                ActiveUsers = activeUsers,
                PublishedCourses = publishedCourses,
                TotalRevenue = totalRevenue,
                TopCourses = topCourses,
                RecentActivities = recentActivities
            };

            return Ok(stats);
        }

        // GET: api/admin/users
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            var currentAdminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var users = await _context.Users
                .Include(u => u.UserCourses)
                .Include(u => u.TestResults)
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.IsActive,
                    u.IsEmailVerified,
                    u.CreatedAt,
                    u.LastLoginAt,
                    EnrolledCoursesCount = u.UserCourses.Count,
                    CompletedTestsCount = u.TestResults.Count(tr => tr.IsCompleted),
                    AverageScore = u.TestResults.Any() ? u.TestResults.Average(tr => tr.Score) : 0
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/admin/courses/manage
        [HttpGet("courses/manage")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<CourseResponseDto>>> GetCoursesForManagement()
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

        // PUT: api/admin/users/{id}/toggle-status
        [HttpPut("users/{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng" });

            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var status = user.IsActive ? "kích hoạt" : "vô hiệu hóa";
            return Ok(new { 
                message = $"Đã {status} người dùng thành công",
                isActive = user.IsActive
            });
        }

        // DELETE: api/admin/users/{id}
        [HttpDelete("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var currentAdminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng" });

            // Don't allow deletion if user has test results or course enrollments
            var hasActivity = await _context.TestResults.AnyAsync(tr => tr.UserId == id) ||
                             await _context.UserCourses.AnyAsync(uc => uc.UserId == id);

            if (hasActivity)
            {
                // Soft delete instead
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Đã vô hiệu hóa người dùng (có dữ liệu liên quan)" });
            }

            // Hard delete if no activity
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa người dùng thành công" });
        }

        // GET: api/admin/reports/course-enrollments
        [HttpGet("reports/course-enrollments")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetCourseEnrollmentReport()
        {
            var report = await _context.Courses
                .Include(c => c.UserCourses)
                .ThenInclude(uc => uc.User)
                .Where(c => c.IsActive)
                .Select(c => new
                {
                    CourseId = c.Id,
                    CourseName = c.Name,
                    CourseType = c.Type,
                    TotalEnrollments = c.UserCourses.Count,
                    ActiveEnrollments = c.UserCourses.Count(uc => uc.User!.IsActive),
                    Revenue = c.UserCourses.Count * c.Price,
                    AverageProgress = c.UserCourses.Any() ? c.UserCourses.Average(uc => uc.Progress) : 0,
                    RecentEnrollments = c.UserCourses
                        .OrderByDescending(uc => uc.EnrolledAt)
                        .Take(5)
                        .Select(uc => new
                        {
                            uc.User!.Username,
                            uc.EnrolledAt,
                            uc.Progress
                        })
                })
                .OrderByDescending(c => c.TotalEnrollments)
                .ToListAsync();

            return Ok(report);
        }

        // GET: api/admin/reports/quiz-performance
        [HttpGet("reports/quiz-performance")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<object>> GetQuizPerformanceReport()
        {
            var report = await _context.Quizzes
                .Include(q => q.Course)
                .Include(q => q.TestResults)
                .ThenInclude(tr => tr.User)
                .Select(q => new
                {
                    QuizId = q.Id,
                    QuizTitle = q.Title,
                    CourseName = q.Course!.Name,
                    TotalAttempts = q.TestResults.Count,
                    CompletedAttempts = q.TestResults.Count(tr => tr.IsCompleted),
                    AverageScore = q.TestResults.Any() ? q.TestResults.Average(tr => tr.Score) : 0,
                    HighestScore = q.TestResults.Any() ? q.TestResults.Max(tr => tr.Score) : 0,
                    LowestScore = q.TestResults.Any() ? q.TestResults.Min(tr => tr.Score) : 0,
                    AverageTimeSpent = q.TestResults.Any() ? q.TestResults.Average(tr => tr.TimeSpent) : 0,
                    RecentResults = q.TestResults
                        .OrderByDescending(tr => tr.CompletedAt)
                        .Take(5)
                        .Select(tr => new
                        {
                            tr.User!.Username,
                            tr.Score,
                            tr.TimeSpent,
                            tr.CompletedAt
                        })
                })
                .OrderByDescending(q => q.TotalAttempts)
                .ToListAsync();

            return Ok(report);
        }
    }
}
