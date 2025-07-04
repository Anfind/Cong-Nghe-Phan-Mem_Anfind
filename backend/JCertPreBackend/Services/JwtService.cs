// backend/JCertPreBackend/Services/JwtService.cs
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JCertPreBackend.Models;

namespace JCertPreBackend.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "Student"),
                new Claim("FullName", user.FullName)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(int.Parse(jwtSettings["ExpiryInHours"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateAdminToken(Admin admin)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Role, admin.Role),
                new Claim("FullName", admin.FullName),
                new Claim("IsSuper", admin.IsSuper.ToString()),
                new Claim("CanManageCourses", admin.CanManageCourses.ToString()),
                new Claim("CanManageUsers", admin.CanManageUsers.ToString()),
                new Claim("CanManageQuizzes", admin.CanManageQuizzes.ToString()),
                new Claim("CanViewReports", admin.CanViewReports.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(int.Parse(jwtSettings["ExpiryInHours"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = secretKey,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int? GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);
                var userIdClaim = jwt?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
            }
            catch
            {
                return null;
            }
        }

        public string? GetRoleFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token);
                var roleClaim = jwt?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                return roleClaim?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
