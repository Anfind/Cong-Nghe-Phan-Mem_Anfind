// backend/JCertPreBackend/Services/IJwtService.cs
using JCertPreBackend.Models;

namespace JCertPreBackend.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateAdminToken(Admin admin);
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
        string? GetRoleFromToken(string token);
    }
}
