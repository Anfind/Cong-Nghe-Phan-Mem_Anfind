// backend/JCertPreBackend/Controllers/VideoController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using JCertPreBackend.Data;
using JCertPreBackend.Models;
using System.Security.Claims;

namespace JCertPreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly JCertPreContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public VideoController(
            JCertPreContext context, 
            IConfiguration configuration, 
            IWebHostEnvironment environment)
        {
            _context = context;
            _configuration = configuration;
            _environment = environment;
        }

        // POST: api/video/upload/{courseId}
        [HttpPost("upload/{courseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadVideo(int courseId, IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
                return BadRequest(new { message = "Vui lòng chọn file video" });

            // Check if course exists
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound(new { message = "Không tìm thấy khóa học" });

            // Validate file
            var fileValidation = ValidateVideoFile(videoFile);
            if (!fileValidation.IsValid)
                return BadRequest(new { message = fileValidation.ErrorMessage });

            try
            {
                // Create upload directory if not exists
                var uploadPath = GetUploadPath();
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Generate unique filename
                var fileName = GenerateUniqueFileName(videoFile.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(stream);
                }

                // Generate URL for accessing the video
                var videoUrl = $"/videos/{fileName}";

                // Update course with video URL
                course.VideoUrl = videoUrl;
                course.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Upload video thành công",
                    videoUrl,
                    fileName,
                    fileSize = videoFile.Length,
                    courseId = course.Id,
                    courseName = course.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi upload video: {ex.Message}" });
            }
        }

        // DELETE: api/video/delete/{courseId}
        [HttpDelete("delete/{courseId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVideo(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound(new { message = "Không tìm thấy khóa học" });

            if (string.IsNullOrEmpty(course.VideoUrl))
                return BadRequest(new { message = "Khóa học chưa có video" });

            try
            {
                // Extract filename from URL
                var fileName = Path.GetFileName(course.VideoUrl);
                var filePath = Path.Combine(GetUploadPath(), fileName);

                // Delete physical file if exists
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Update course
                course.VideoUrl = null;
                course.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Xóa video thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi xóa video: {ex.Message}" });
            }
        }

        // GET: api/video/info/{courseId}
        [HttpGet("info/{courseId}")]
        public async Task<ActionResult<object>> GetVideoInfo(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return NotFound(new { message = "Không tìm thấy khóa học" });

            if (string.IsNullOrEmpty(course.VideoUrl))
                return Ok(new { hasVideo = false, message = "Khóa học chưa có video" });

            try
            {
                var fileName = Path.GetFileName(course.VideoUrl);
                var filePath = Path.Combine(GetUploadPath(), fileName);

                var fileInfo = new FileInfo(filePath);
                var exists = fileInfo.Exists;

                return Ok(new
                {
                    hasVideo = exists,
                    videoUrl = course.VideoUrl,
                    fileName,
                    fileSize = exists ? fileInfo.Length : 0,
                    lastModified = exists ? fileInfo.LastWriteTime : (DateTime?)null,
                    courseName = course.Name
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi lấy thông tin video: {ex.Message}" });
            }
        }

        // GET: api/video/list
        [HttpGet("list")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetVideoList()
        {
            var coursesWithVideos = await _context.Courses
                .Where(c => !string.IsNullOrEmpty(c.VideoUrl))
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Type,
                    c.VideoUrl,
                    c.IsActive,
                    c.IsPublished,
                    c.UpdatedAt,
                    EnrolledCount = c.UserCourses.Count
                })
                .OrderByDescending(c => c.UpdatedAt)
                .ToListAsync();

            // Add file info for each video
            var result = new List<object>();
            var uploadPath = GetUploadPath();

            foreach (var course in coursesWithVideos)
            {
                var fileName = Path.GetFileName(course.VideoUrl);
                if (!string.IsNullOrEmpty(fileName))
                {
                    var filePath = Path.Combine(uploadPath, fileName);
                    var fileInfo = new FileInfo(filePath);

                    result.Add(new
                    {
                        course.Id,
                        course.Name,
                        course.Type,
                        course.VideoUrl,
                        course.IsActive,
                        course.IsPublished,
                        course.UpdatedAt,
                        course.EnrolledCount,
                        FileName = fileName,
                        FileSize = fileInfo.Exists ? fileInfo.Length : 0,
                        FileExists = fileInfo.Exists,
                        LastModified = fileInfo.Exists ? fileInfo.LastWriteTime : (DateTime?)null
                    });
                }
            }

            return Ok(result);
        }

        // Private helper methods
        private (bool IsValid, string ErrorMessage) ValidateVideoFile(IFormFile file)
        {
            var maxSizeMB = _configuration.GetValue<int>("FileStorage:MaxFileSizeMB", 100);
            var allowedExtensions = _configuration.GetSection("FileStorage:AllowedVideoExtensions").Get<string[]>() 
                                  ?? new[] { "mp4", "avi", "mkv", "mov" };

            // Check file size
            var maxSizeBytes = maxSizeMB * 1024 * 1024;
            if (file.Length > maxSizeBytes)
                return (false, $"File quá lớn. Kích thước tối đa: {maxSizeMB}MB");

            // Check file extension
            var extension = Path.GetExtension(file.FileName)?.TrimStart('.').ToLower();
            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
                return (false, $"Định dạng file không được hỗ trợ. Cho phép: {string.Join(", ", allowedExtensions)}");

            // Check file content (basic MIME type validation)
            var contentType = file.ContentType.ToLower();
            if (!contentType.StartsWith("video/"))
                return (false, "File không phải là video");

            return (true, string.Empty);
        }

        private string GetUploadPath()
        {
            var configPath = _configuration.GetValue<string>("FileStorage:VideoStoragePath", "wwwroot/videos");
            return Path.Combine(_environment.ContentRootPath, configPath ?? "wwwroot/videos");
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var guid = Guid.NewGuid().ToString("N")[..8]; // First 8 chars of GUID
            
            return $"{nameWithoutExtension}_{timestamp}_{guid}{extension}";
        }
    }
}
