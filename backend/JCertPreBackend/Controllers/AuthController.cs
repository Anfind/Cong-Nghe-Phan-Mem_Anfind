// backend/JCertPreBackend/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JCertPreBackend.Data;
using JCertPreBackend.Models;
using JCertPreBackend.Services;
using System.ComponentModel.DataAnnotations;

namespace JCertPreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JCertPreContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(JCertPreContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // First try to find in Users table
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

            if (user != null && user.Password == request.Password)
            {
                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var token = _jwtService.GenerateToken(user);

                return Ok(new 
                { 
                    message = "Đăng nhập thành công",
                    token,
                    userType = "user",
                    user = new
                    {
                        user.Id,
                        user.Username,
                        user.FullName,
                        user.Email,
                        user.Role
                    }
                });
            }

            // If not found in Users, try Admins table
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == request.Username);

            if (admin != null && admin.Password == request.Password)
            {
                // Update last login for admin
                admin.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var token = _jwtService.GenerateAdminToken(admin);

                return Ok(new 
                { 
                    message = "Đăng nhập admin thành công",
                    token,
                    userType = "admin",
                    user = new
                    {
                        admin.Id,
                        admin.Username,
                        admin.FullName,
                        admin.Email,
                        Role = "Admin"
                    }
                });
            }

            return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không đúng" });
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validation
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Tên đăng nhập và mật khẩu không được để trống" });

            if (request.Password.Length < 6)
                return BadRequest(new { message = "Mật khẩu phải có ít nhất 6 ký tự" });

            if (string.IsNullOrEmpty(request.FullName))
                return BadRequest(new { message = "Họ tên không được để trống" });

            if (string.IsNullOrEmpty(request.Email))
                return BadRequest(new { message = "Email không được để trống" });

            // Check if username exists
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại" });

            // Check if email exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { message = "Email đã được sử dụng" });

            // Create new user
            var user = new User
            {
                Username = request.Username,
                Password = request.Password, // Store plain text password
                FullName = request.FullName,
                Email = request.Email,
                Role = "Student",
                IsActive = true,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Đăng ký thành công",
                user = new
                {
                    user.Id,
                    user.Username,
                    user.FullName,
                    user.Email,
                    user.Role
                }
            });
        }

        // POST: api/auth/change-password
        [HttpPost("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng" });

            // Verify current password
            if (user.Password != request.CurrentPassword)
                return BadRequest(new { message = "Mật khẩu hiện tại không đúng" });

            // Validate new password
            if (request.NewPassword.Length < 6)
                return BadRequest(new { message = "Mật khẩu mới phải có ít nhất 6 ký tự" });

            // Update password (plain text)
            user.Password = request.NewPassword;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đổi mật khẩu thành công" });
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult Logout()
        {
            // In JWT, logout is typically handled client-side by removing the token
            // Server-side logout would require token blacklisting (not implemented for simplicity)
            return Ok(new { message = "Đăng xuất thành công" });
        }

        // GET: api/auth/test-cors
        [HttpGet("test-cors")]
        public IActionResult TestCors()
        {
            return Ok(new { 
                message = "CORS is working correctly!", 
                timestamp = DateTime.UtcNow,
                server = "JCertPre Backend",
                port = "5032",
                environment = "Development"
            });
        }

        // GET: api/auth/debug - Debug endpoint to check seed data
        [HttpGet("debug")]
        public async Task<IActionResult> DebugAuth()
        {
            var usersCount = await _context.Users.CountAsync();
            var adminsCount = await _context.Admins.CountAsync();
            
            var users = await _context.Users.Select(u => new { 
                u.Username, 
                u.FullName, 
                u.Role, 
                u.IsActive 
            }).ToListAsync();
            
            var admins = await _context.Admins.Select(a => new { 
                a.Username, 
                a.FullName, 
                a.IsSuper 
            }).ToListAsync();

            return Ok(new { 
                message = "Debug info",
                usersCount,
                adminsCount,
                users,
                admins,
                timestamp = DateTime.UtcNow
            });
        }
    }

    // Request DTOs
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}