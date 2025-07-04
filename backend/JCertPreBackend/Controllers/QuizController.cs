// backend/JCertPreBackend/Controllers/QuizController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JCertPreBackend.Data;
using JCertPreBackend.Models;
using System.Text.Json;

namespace JCertPreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly JCertPreContext _context;

        public QuizController(JCertPreContext context)
        {
            _context = context;
            SeedQuizData();
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<Quiz>> GetQuizByCourse(int courseId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.CourseId == courseId && q.IsActive);

            if (quiz == null)
                return NotFound(new { message = "Không tìm thấy bài thi cho khóa học này" });

            return Ok(quiz);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizRequest request)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == request.QuizId);

            if (quiz == null)
                return NotFound(new { message = "Không tìm thấy bài thi" });

            // Calculate score
            int correctAnswers = 0;
            foreach (var answer in request.Answers)
            {
                var question = quiz.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question != null && question.CorrectAnswer == answer.SelectedAnswer)
                {
                    correctAnswers++;
                }
            }

            int score = (int)Math.Round((double)correctAnswers / quiz.Questions.Count * 100);

            // Save result
            var testResult = new TestResult
            {
                UserId = request.UserId,
                QuizId = request.QuizId,
                Score = score,
                CorrectAnswers = correctAnswers,
                TotalQuestions = quiz.Questions.Count,
                TimeSpent = request.TimeSpent,
                StartTime = request.StartTime,
                EndTime = DateTime.Now,
                UserAnswers = JsonSerializer.Serialize(request.Answers)
            };

            _context.TestResults.Add(testResult);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                score = score,
                correctAnswers = correctAnswers,
                totalQuestions = quiz.Questions.Count,
                passed = score >= 70,
                resultId = testResult.Id
            });
        }

        [HttpGet("results/user/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserResults(int userId)
        {
            var results = await _context.TestResults
                .Include(tr => tr.Quiz)
                .ThenInclude(q => q!.Course)
                .Where(tr => tr.UserId == userId)
                .OrderByDescending(tr => tr.EndTime)
                .Select(tr => new
                {
                    tr.Id,
                    tr.Score,
                    tr.CorrectAnswers,
                    tr.TotalQuestions,
                    tr.TimeSpent,
                    tr.StartTime,
                    tr.EndTime,
                    QuizTitle = tr.Quiz!.Title,
                    CourseName = tr.Quiz.Course!.Name,
                    CourseId = tr.Quiz.CourseId
                })
                .ToListAsync();

            return Ok(results);
        }

        private void SeedQuizData()
        {
            if (!_context.Quizzes.Any())
            {
                // JLPT N5 Quiz
                var jlptQuiz = new Quiz
                {
                    Id = 1,
                    CourseId = 1,
                    Title = "JLPT N5 Practice Test",
                    Description = "Bài thi thử JLPT N5",
                    TimeLimit = 30,
                    IsActive = true
                };

                var jlptQuestions = new List<Question>
                {
                    new Question
                    {
                        Id = 1,
                        QuizId = 1,
                        Text = "私は＿＿＿学生です。",
                        Option1 = "の",
                        Option2 = "が",
                        Option3 = "を",
                        Option4 = "に",
                        CorrectAnswer = 0,
                        Explanation = "「の」được sử dụng để biểu thị sở hữu hoặc thuộc tính.",
                        Order = 1
                    },
                    new Question
                    {
                        Id = 2,
                        QuizId = 1,
                        Text = "今日は＿＿＿天気がいいです。",
                        Option1 = "とても",
                        Option2 = "あまり",
                        Option3 = "ちょっと",
                        Option4 = "たぶん",
                        CorrectAnswer = 0,
                        Explanation = "「とても」có nghĩa là 'rất' và phù hợp với ngữ cảnh tích cực.",
                        Order = 2
                    },
                    new Question
                    {
                        Id = 3,
                        QuizId = 1,
                        Text = "昨日映画を＿＿＿ました。",
                        Option1 = "見る",
                        Option2 = "見た",
                        Option3 = "見て",
                        Option4 = "見",
                        CorrectAnswer = 2,
                        Explanation = "「見て」là dạng te-form của động từ 見る, dùng khi kết hợp với ました.",
                        Order = 3
                    },
                    new Question
                    {
                        Id = 4,
                        QuizId = 1,
                        Text = "この本は＿＿＿ですか。",
                        Option1 = "だれ",
                        Option2 = "どこ",
                        Option3 = "なに",
                        Option4 = "だれの",
                        CorrectAnswer = 3,
                        Explanation = "「だれの」nghĩa là 'của ai', dùng để hỏi về sở hữu.",
                        Order = 4
                    },
                    new Question
                    {
                        Id = 5,
                        QuizId = 1,
                        Text = "毎朝７時＿＿＿起きます。",
                        Option1 = "に",
                        Option2 = "で",
                        Option3 = "を",
                        Option4 = "が",
                        CorrectAnswer = 0,
                        Explanation = "「に」được sử dụng để chỉ thời gian cụ thể.",
                        Order = 5
                    }
                };

                // NAT-TEST N4 Quiz
                var natQuiz = new Quiz
                {
                    Id = 2,
                    CourseId = 2,
                    Title = "NAT-TEST N4 Practice Test",
                    Description = "Bài thi thử NAT-TEST N4",
                    TimeLimit = 30,
                    IsActive = true
                };

                var natQuestions = new List<Question>
                {
                    new Question
                    {
                        Id = 6,
                        QuizId = 2,
                        Text = "友達と一緒に映画を＿＿＿に行きました。",
                        Option1 = "見る",
                        Option2 = "見",
                        Option3 = "見て",
                        Option4 = "見た",
                        CorrectAnswer = 0,
                        Explanation = "「見る」是动词原形，与「に行く」搭配使用。",
                        Order = 1
                    },
                    new Question
                    {
                        Id = 7,
                        QuizId = 2,
                        Text = "この料理は＿＿＿おいしいです。",
                        Option1 = "とても",
                        Option2 = "あまり",
                        Option3 = "全然",
                        Option4 = "ちっとも",
                        CorrectAnswer = 0,
                        Explanation = "「とても」表示程度很高，用于肯定句。",
                        Order = 2
                    }
                };

                _context.Quizzes.AddRange(jlptQuiz, natQuiz);
                _context.Questions.AddRange(jlptQuestions);
                _context.Questions.AddRange(natQuestions);
                _context.SaveChanges();
            }
        }
    }

    public class SubmitQuizRequest
    {
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public int TimeSpent { get; set; } // seconds
        public DateTime StartTime { get; set; }
        public List<UserAnswer> Answers { get; set; } = new List<UserAnswer>();
    }

    public class UserAnswer
    {
        public int QuestionId { get; set; }
        public int SelectedAnswer { get; set; } // 0-3
    }
}
