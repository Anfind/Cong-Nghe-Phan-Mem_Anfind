using Microsoft.EntityFrameworkCore;
using JCertPreBackend.Data;
using JCertPreBackend.Models;
using JCertPreBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Entity Framework with MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<JCertPreContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register JWT Service
builder.Services.AddScoped<IJwtService, JwtService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
        };
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure database is created and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<JCertPreContext>();
    
    try
    {
        // Ensure database is created
        context.Database.EnsureCreated();
        
        // Seed data if no users exist
        if (!context.Users.Any())
        {
            // Add sample users with plain text passwords
            context.Users.AddRange(
                new User 
                { 
                    Username = "testuser", 
                    Password = "password123", // Plain text password
                    FullName = "Test User", 
                    Email = "test@jcertpre.com", 
                    Role = "Student",
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.Now
                },
                new User 
                { 
                    Username = "admin", 
                    Password = "admin123", // Plain text password
                    FullName = "Admin User", 
                    Email = "admin@jcertpre.com", 
                    Role = "Admin",
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.Now
                }
            );
        }

        // Seed courses if none exist
        if (!context.Courses.Any())
        {
            context.Courses.AddRange(
                new Course
                {
                    Name = "JLPT N5 - Cơ bản",
                    Type = "JLPT",
                    Description = "Khóa học tiếng Nhật cơ bản cho người mới bắt đầu. Học từ vựng, ngữ pháp và kanji cơ bản.",
                    VideoUrl = "https://example.com/video1.mp4",
                    IsActive = true,
                    IsPublished = true,
                    ViewCount = 0,
                    Price = 0
                },
                new Course
                {
                    Name = "JLPT N4 - Trung cấp",
                    Type = "JLPT",
                    Description = "Khóa học tiếng Nhật trung cấp. Mở rộng từ vựng và ngữ pháp phức tạp hơn.",
                    VideoUrl = "https://example.com/video2.mp4",
                    IsActive = true,
                    IsPublished = true,
                    ViewCount = 0,
                    Price = 0
                },
                new Course
                {
                    Name = "NAT-TEST 5Q",
                    Type = "NAT-TEST",
                    Description = "Khóa học luyện thi NAT-TEST mức độ 5Q tương đương JLPT N5.",
                    VideoUrl = "https://example.com/video3.mp4",
                    IsActive = true,
                    IsPublished = true,
                    ViewCount = 0,
                    Price = 0
                }
            );
        }

        context.SaveChanges();
        Console.WriteLine("Database seeded successfully with plain text passwords!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowReactApp");

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Console.WriteLine("JCertPre Backend is starting...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("Database: MySQL (Plain text passwords enabled)");

app.Run();