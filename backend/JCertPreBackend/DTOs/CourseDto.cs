// backend/JCertPreBackend/DTOs/CourseCreateDto.cs
using System.ComponentModel.DataAnnotations;

namespace JCertPreBackend.DTOs
{
    public class CourseCreateDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        [MaxLength(500)]
        public string? VideoUrl { get; set; }
        
        public decimal Price { get; set; } = 0;
        public bool IsPublished { get; set; } = false;
    }

    public class CourseUpdateDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }
        
        [MaxLength(50)]
        public string? Type { get; set; }
        
        public string? Description { get; set; }
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        [MaxLength(500)]
        public string? VideoUrl { get; set; }
        
        public decimal? Price { get; set; }
        public bool? IsPublished { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CourseResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublished { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int EnrolledUsersCount { get; set; }
        public int QuizzesCount { get; set; }
    }
}
