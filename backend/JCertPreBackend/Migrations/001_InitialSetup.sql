-- MySQL Database Setup Script for JCertPre
-- Run this script to create the database and tables

-- Create production database
CREATE DATABASE IF NOT EXISTS jcertpre_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Create development database  
CREATE DATABASE IF NOT EXISTS jcertpre_dev_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Use development database for setup
USE jcertpre_dev_db;

-- Create users table
CREATE TABLE IF NOT EXISTS users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    Role VARCHAR(20) DEFAULT 'Student',
    IsActive BOOLEAN DEFAULT TRUE,
    IsEmailVerified BOOLEAN DEFAULT FALSE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    LastLoginAt DATETIME NULL,
    UpdatedAt DATETIME NULL,
    INDEX idx_username (Username),
    INDEX idx_email (Email),
    INDEX idx_role (Role)
);

-- Create admins table
CREATE TABLE IF NOT EXISTS admins (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE,
    Role VARCHAR(20) DEFAULT 'Admin',
    IsActive BOOLEAN DEFAULT TRUE,
    IsSuper BOOLEAN DEFAULT FALSE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    LastLoginAt DATETIME NULL,
    UpdatedAt DATETIME NULL,
    CanManageCourses BOOLEAN DEFAULT TRUE,
    CanManageUsers BOOLEAN DEFAULT TRUE,
    CanManageQuizzes BOOLEAN DEFAULT TRUE,
    CanViewReports BOOLEAN DEFAULT TRUE,
    INDEX idx_username (Username),
    INDEX idx_email (Email)
);

-- Create courses table
CREATE TABLE IF NOT EXISTS courses (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Type VARCHAR(50) NOT NULL,
    Description TEXT NULL,
    ImageUrl VARCHAR(500) NULL,
    VideoUrl VARCHAR(500) NULL,
    Price DECIMAL(10,2) DEFAULT 0.00,
    IsActive BOOLEAN DEFAULT TRUE,
    IsPublished BOOLEAN DEFAULT FALSE,
    ViewCount INT DEFAULT 0,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_type (Type),
    INDEX idx_active_published (IsActive, IsPublished),
    INDEX idx_created_at (CreatedAt)
);

-- Create quizzes table
CREATE TABLE IF NOT EXISTS quizzes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CourseId INT NOT NULL,
    Title VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    TimeLimit INT DEFAULT 30,
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (CourseId) REFERENCES courses(Id) ON DELETE CASCADE,
    INDEX idx_course_id (CourseId),
    INDEX idx_active (IsActive)
);

-- Create questions table
CREATE TABLE IF NOT EXISTS questions (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    QuizId INT NOT NULL,
    Text TEXT NOT NULL,
    Option1 VARCHAR(500) NULL,
    Option2 VARCHAR(500) NULL,
    Option3 VARCHAR(500) NULL,
    Option4 VARCHAR(500) NULL,
    Options JSON NULL,
    CorrectAnswer VARCHAR(255) NOT NULL,
    Explanation TEXT NULL,
    `Order` INT DEFAULT 0,
    Points INT DEFAULT 1,
    FOREIGN KEY (QuizId) REFERENCES quizzes(Id) ON DELETE CASCADE,
    INDEX idx_quiz_id (QuizId),
    INDEX idx_order (`Order`)
);

-- Create test_results table
CREATE TABLE IF NOT EXISTS test_results (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    QuizId INT NOT NULL,
    Score INT DEFAULT 0,
    CorrectAnswers INT DEFAULT 0,
    TotalQuestions INT DEFAULT 0,
    TimeSpent INT DEFAULT 0,
    StartTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    EndTime DATETIME DEFAULT CURRENT_TIMESTAMP,
    CompletedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UserAnswers TEXT NULL,
    Answers JSON NULL,
    IsCompleted BOOLEAN DEFAULT FALSE,
    Notes TEXT NULL,
    FOREIGN KEY (UserId) REFERENCES users(Id) ON DELETE CASCADE,
    FOREIGN KEY (QuizId) REFERENCES quizzes(Id) ON DELETE CASCADE,
    INDEX idx_user_id (UserId),
    INDEX idx_quiz_id (QuizId),
    INDEX idx_completed_at (CompletedAt),
    INDEX idx_score (Score)
);

-- Create user_courses table
CREATE TABLE IF NOT EXISTS user_courses (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    CourseId INT NOT NULL,
    Progress INT DEFAULT 0,
    EnrolledAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    LastAccessedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES users(Id) ON DELETE CASCADE,
    FOREIGN KEY (CourseId) REFERENCES courses(Id) ON DELETE CASCADE,
    UNIQUE KEY unique_enrollment (UserId, CourseId),
    INDEX idx_user_id (UserId),
    INDEX idx_course_id (CourseId),
    INDEX idx_enrolled_at (EnrolledAt)
);

-- Insert default admin user (password: admin123)
INSERT IGNORE INTO admins (Username, Password, FullName, Email, IsSuper, CreatedAt) 
VALUES ('admin', 'admin123', 'System Administrator', 'admin@jcertpre.com', TRUE, NOW());

-- Insert sample test user (password: password123)
INSERT IGNORE INTO users (Username, Password, FullName, Email, Role, CreatedAt) 
VALUES ('testuser', 'password123', 'Test User', 'test@jcertpre.com', 'Student', NOW());

-- Insert sample courses
INSERT IGNORE INTO courses (Id, Name, Type, Description, VideoUrl, IsActive, IsPublished, CreatedAt) VALUES
(1, 'JLPT N5 - Cơ bản', 'JLPT', 'Khóa học tiếng Nhật cơ bản cho người mới bắt đầu. Học từ vựng, ngữ pháp và kanji cơ bản.', 'https://example.com/video1.mp4', TRUE, TRUE, NOW()),
(2, 'JLPT N4 - Trung cấp', 'JLPT', 'Khóa học tiếng Nhật trung cấp. Mở rộng từ vựng và ngữ pháp phức tạp hơn.', 'https://example.com/video2.mp4', TRUE, TRUE, NOW()),
(3, 'NAT-TEST 5Q', 'NAT-TEST', 'Khóa học luyện thi NAT-TEST mức độ 5Q tương đương JLPT N5.', 'https://example.com/video3.mp4', TRUE, TRUE, NOW());

-- Insert sample quizzes
INSERT IGNORE INTO quizzes (Id, CourseId, Title, Description, TimeLimit, CreatedAt) VALUES
(1, 1, 'Bài kiểm tra N5 - Phần 1', 'Kiểm tra từ vựng và ngữ pháp cơ bản N5', 30, NOW()),
(2, 1, 'Bài kiểm tra N5 - Phần 2', 'Kiểm tra kanji và đọc hiểu N5', 25, NOW()),
(3, 2, 'Bài kiểm tra N4 - Phần 1', 'Kiểm tra từ vựng và ngữ pháp N4', 35, NOW());

-- Insert sample questions
INSERT IGNORE INTO questions (Id, QuizId, Text, Option1, Option2, Option3, Option4, CorrectAnswer, Explanation, `Order`) VALUES
(1, 1, 'Chọn nghĩa đúng của từ "がっこう"', 'Trường học', 'Bệnh viện', 'Cửa hàng', 'Nhà hàng', 'Trường học', 'がっこう (gakkou) có nghĩa là trường học', 1),
(2, 1, '"わたし___がくせいです。" Điền từ thích hợp vào chỗ trống.', 'は', 'が', 'を', 'に', 'は', 'Trợ từ は (wa) được dùng để chỉ chủ đề của câu', 2),
(3, 1, 'Cách đọc của kanji "本" trong từ "本を読む" là gì?', 'ほん', 'もと', 'ぼん', 'ぽん', 'ほん', 'Trong "本を読む" (đọc sách), 本 được đọc là ほん (hon)', 3);

COMMIT;
