// backend/JCertPreBackend/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using JCertPreBackend.Data;
using JCertPreBackend.Models;

namespace JCertPreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JCertPreContext _context;

        public AuthController(JCertPreContext context)
        {
            _context = context;
            // Seed default user
            if (!_context.Users.Any())
            {
                _context.Users.Add(new User 
                { 
                    Id = 1, 
                    Username = "test", 
                    Password = "123456",
                    FullName = "Test User",
                    Email = "test@example.com"
                });
                _context.SaveChanges();
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);
            if (user == null) 
                return Unauthorized(new { message = "Sai tên đăng nhập hoặc mật khẩu" });
            
            // Update last login
            user.LastLoginAt = DateTime.Now;
            _context.SaveChanges();

            return Ok(new { 
                UserId = user.Id, 
                Username = user.Username,
                FullName = user.FullName,
                Message = "Đăng nhập thành công" 
            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // Validation
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Tên đăng nhập và mật khẩu không được để trống" });

            if (request.Password.Length < 6)
                return BadRequest(new { message = "Mật khẩu phải có ít nhất 6 ký tự" });

            // Check if username exists
            if (_context.Users.Any(u => u.Username == request.Username))
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại" });

            // Check if email exists
            if (!string.IsNullOrEmpty(request.Email) && _context.Users.Any(u => u.Email == request.Email))
                return BadRequest(new { message = "Email đã được sử dụng" });

            // Create new user
            var user = new User
            {
                Username = request.Username,
                Password = request.Password, // Trong thực tế cần hash password
                FullName = request.FullName,
                Email = request.Email
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { 
                message = "Đăng ký thành công",
                UserId = user.Id
            });
        }
    }

    public class LoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class RegisterRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }
}