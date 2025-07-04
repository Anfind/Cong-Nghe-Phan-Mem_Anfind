import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';

function Quiz({ user }) {
    const { courseId } = useParams();
    const [course, setCourse] = useState(null);
    const [questions, setQuestions] = useState([]);
    const [currentQuestion, setCurrentQuestion] = useState(0);
    const [answers, setAnswers] = useState({});
    const [showResult, setShowResult] = useState(false);
    const [score, setScore] = useState(0);
    const [loading, setLoading] = useState(true);
    const [timeLeft, setTimeLeft] = useState(1800); // 30 phút
    const [quizStarted, setQuizStarted] = useState(false);

    useEffect(() => {
        // Mock quiz data
        const mockQuestions = [
            {
                id: 1,
                question: "私は＿＿＿学生です。",
                options: ["の", "が", "を", "に"],
                correctAnswer: 0,
                explanation: "「の」được sử dụng để biểu thị sở hữu hoặc thuộc tính."
            },
            {
                id: 2,
                question: "今日は＿＿＿天気がいいです。",
                options: ["とても", "あまり", "ちょっと", "たぶん"],
                correctAnswer: 0,
                explanation: "「とても」có nghĩa là 'rất' và phù hợp với ngữ cảnh tích cực."
            },
            {
                id: 3,
                question: "昨日映画を＿＿＿ました。",
                options: ["見る", "見た", "見て", "見"],
                correctAnswer: 2,
                explanation: "「見て」là dạng te-form của động từ 見る, dùng khi kết hợp với ました."
            },
            {
                id: 4,
                question: "この本は＿＿＿ですか。",
                options: ["だれ", "どこ", "なに", "だれの"],
                correctAnswer: 3,
                explanation: "「だれの」nghĩa là 'của ai', dùng để hỏi về sở hữu."
            },
            {
                id: 5,
                question: "毎朝７時＿＿＿起きます。",
                options: ["に", "で", "を", "が"],
                correctAnswer: 0,
                explanation: "「に」được sử dụng để chỉ thời gian cụ thể."
            }
        ];

        const fetchCourseAndQuiz = async () => {
            try {
                const response = await axios.get('http://localhost:5032/api/course');
                const courseData = response.data.find(c => c.id === parseInt(courseId));
                setCourse(courseData);
                setQuestions(mockQuestions);
            } catch (error) {
                console.error('Error fetching course:', error);
                alert('Không thể tải thông tin bài thi');
            } finally {
                setLoading(false);
            }
        };

        fetchCourseAndQuiz();
    }, [courseId]);

    const startQuiz = () => {
        setQuizStarted(true);
        setTimeLeft(1800); // Reset timer
    };

    const handleAnswerSelect = (questionId, answerIndex) => {
        setAnswers({
            ...answers,
            [questionId]: answerIndex
        });
    };

    const handleNextQuestion = () => {
        if (currentQuestion < questions.length - 1) {
            setCurrentQuestion(currentQuestion + 1);
        }
    };

    const handlePrevQuestion = () => {
        if (currentQuestion > 0) {
            setCurrentQuestion(currentQuestion - 1);
        }
    };

    const handleSubmitQuiz = async () => {
        let correctAnswers = 0;
        questions.forEach(question => {
            if (answers[question.id] === question.correctAnswer) {
                correctAnswers++;
            }
        });

        const finalScore = Math.round((correctAnswers / questions.length) * 100);
        setScore(finalScore);
        setShowResult(true);

        // Lưu kết quả (tạm thời lưu vào localStorage)
        const result = {
            courseId: parseInt(courseId),
            courseName: course?.name,
            score: finalScore,
            totalQuestions: questions.length,
            correctAnswers: correctAnswers,
            date: new Date().toISOString(),
            userId: user.id
        };

        const savedResults = JSON.parse(localStorage.getItem('quizResults') || '[]');
        savedResults.push(result);
        localStorage.setItem('quizResults', JSON.stringify(savedResults));
    };

    const formatTime = (seconds) => {
        const mins = Math.floor(seconds / 60);
        const secs = seconds % 60;
        return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
    };

    if (loading) {
        return (
            <div style={{ textAlign: 'center', marginTop: '100px' }}>
                <div>Đang tải...</div>
            </div>
        );
    }

    if (!course) {
        return (
            <div style={{ textAlign: 'center', marginTop: '100px' }}>
                <div>Không tìm thấy khóa học</div>
            </div>
        );
    }

    if (!quizStarted) {
        return (
            <div style={{ maxWidth: '600px', margin: '100px auto', padding: '30px', textAlign: 'center' }}>
                <h1 style={{ color: '#333' }}>Bài thi thử - {course.name}</h1>
                <div style={{ 
                    backgroundColor: '#f8f9fa', 
                    padding: '20px', 
                    borderRadius: '8px',
                    marginBottom: '30px'
                }}>
                    <h3 style={{ color: '#333', marginTop: 0 }}>Thông tin bài thi</h3>
                    <p><strong>Số câu hỏi:</strong> {questions.length} câu</p>
                    <p><strong>Thời gian:</strong> 30 phút</p>
                    <p><strong>Điểm tối đa:</strong> 100 điểm</p>
                    <p style={{ color: '#dc3545' }}><strong>Lưu ý:</strong> Sau khi bắt đầu, bạn không thể tạm dừng!</p>
                </div>
                <button
                    onClick={startQuiz}
                    style={{
                        padding: '15px 30px',
                        backgroundColor: '#28a745',
                        color: 'white',
                        border: 'none',
                        borderRadius: '8px',
                        fontSize: '18px',
                        cursor: 'pointer'
                    }}
                >
                    Bắt đầu làm bài
                </button>
            </div>
        );
    }

    if (showResult) {
        return (
            <div style={{ maxWidth: '800px', margin: '50px auto', padding: '30px' }}>
                <div style={{ textAlign: 'center', marginBottom: '30px' }}>
                    <h1 style={{ color: '#333' }}>Kết quả bài thi</h1>
                    <div style={{
                        backgroundColor: score >= 70 ? '#d4edda' : '#f8d7da',
                        color: score >= 70 ? '#155724' : '#721c24',
                        padding: '20px',
                        borderRadius: '8px',
                        fontSize: '24px',
                        fontWeight: 'bold'
                    }}>
                        Điểm của bạn: {score}/100
                    </div>
                    <p style={{ color: '#666', marginTop: '10px' }}>
                        Số câu đúng: {Object.values(answers).filter((answer, index) => answer === questions[index]?.correctAnswer).length}/{questions.length}
                    </p>
                </div>

                <h3 style={{ color: '#333' }}>Chi tiết từng câu:</h3>
                {questions.map((question, index) => {
                    const userAnswer = answers[question.id];
                    const isCorrect = userAnswer === question.correctAnswer;
                    
                    return (
                        <div key={question.id} style={{
                            border: '1px solid #ddd',
                            borderRadius: '8px',
                            padding: '20px',
                            marginBottom: '15px',
                            backgroundColor: isCorrect ? '#d4edda' : '#f8d7da'
                        }}>
                            <p style={{ fontWeight: 'bold', color: '#333' }}>
                                Câu {index + 1}: {question.question}
                            </p>
                            <p style={{ color: '#666' }}>
                                <strong>Đáp án của bạn:</strong> {question.options[userAnswer] || 'Không trả lời'}
                            </p>
                            <p style={{ color: '#666' }}>
                                <strong>Đáp án đúng:</strong> {question.options[question.correctAnswer]}
                            </p>
                            <p style={{ color: '#666', fontStyle: 'italic' }}>
                                <strong>Giải thích:</strong> {question.explanation}
                            </p>
                        </div>
                    );
                })}

                <div style={{ textAlign: 'center', marginTop: '30px' }}>
                    <button
                        onClick={() => window.location.href = '/'}
                        style={{
                            padding: '12px 24px',
                            backgroundColor: '#007bff',
                            color: 'white',
                            border: 'none',
                            borderRadius: '4px',
                            marginRight: '10px',
                            cursor: 'pointer'
                        }}
                    >
                        Về trang chủ
                    </button>
                    <button
                        onClick={() => window.location.href = '/results'}
                        style={{
                            padding: '12px 24px',
                            backgroundColor: '#17a2b8',
                            color: 'white',
                            border: 'none',
                            borderRadius: '4px',
                            cursor: 'pointer'
                        }}
                    >
                        Xem tất cả kết quả
                    </button>
                </div>
            </div>
        );
    }

    const currentQ = questions[currentQuestion];

    return (
        <div style={{ maxWidth: '800px', margin: '50px auto', padding: '30px' }}>
            {/* Header */}
            <div style={{
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                marginBottom: '30px',
                padding: '20px',
                backgroundColor: '#f8f9fa',
                borderRadius: '8px'
            }}>
                <h2 style={{ margin: 0, color: '#333' }}>
                    Bài thi thử - {course.name}
                </h2>
                <div style={{
                    fontSize: '20px',
                    fontWeight: 'bold',
                    color: timeLeft < 300 ? '#dc3545' : '#333'
                }}>
                    ⏰ {formatTime(timeLeft)}
                </div>
            </div>

            {/* Progress */}
            <div style={{ marginBottom: '30px' }}>
                <div style={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    marginBottom: '10px'
                }}>
                    <span>Câu {currentQuestion + 1} / {questions.length}</span>
                    <span>{Math.round(((currentQuestion + 1) / questions.length) * 100)}%</span>
                </div>
                <div style={{
                    width: '100%',
                    backgroundColor: '#e9ecef',
                    borderRadius: '4px',
                    height: '8px'
                }}>
                    <div style={{
                        width: `${((currentQuestion + 1) / questions.length) * 100}%`,
                        backgroundColor: '#007bff',
                        height: '100%',
                        borderRadius: '4px',
                        transition: 'width 0.3s'
                    }}></div>
                </div>
            </div>

            {/* Question */}
            <div style={{
                border: '1px solid #ddd',
                borderRadius: '8px',
                padding: '30px',
                backgroundColor: 'white',
                marginBottom: '30px'
            }}>
                <h3 style={{ color: '#333', marginBottom: '20px' }}>
                    {currentQ.question}
                </h3>

                <div style={{ display: 'grid', gap: '15px' }}>
                    {currentQ.options.map((option, index) => (
                        <label
                            key={index}
                            style={{
                                display: 'flex',
                                alignItems: 'center',
                                padding: '15px',
                                border: '2px solid',
                                borderColor: answers[currentQ.id] === index ? '#007bff' : '#ddd',
                                borderRadius: '8px',
                                cursor: 'pointer',
                                backgroundColor: answers[currentQ.id] === index ? '#e3f2fd' : 'white',
                                transition: 'all 0.2s'
                            }}
                        >
                            <input
                                type="radio"
                                name={`question-${currentQ.id}`}
                                value={index}
                                checked={answers[currentQ.id] === index}
                                onChange={() => handleAnswerSelect(currentQ.id, index)}
                                style={{ marginRight: '10px' }}
                            />
                            <span style={{ fontSize: '16px' }}>{option}</span>
                        </label>
                    ))}
                </div>
            </div>

            {/* Navigation */}
            <div style={{
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center'
            }}>
                <button
                    onClick={handlePrevQuestion}
                    disabled={currentQuestion === 0}
                    style={{
                        padding: '12px 24px',
                        backgroundColor: currentQuestion === 0 ? '#ccc' : '#6c757d',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: currentQuestion === 0 ? 'not-allowed' : 'pointer'
                    }}
                >
                    ← Câu trước
                </button>

                <div style={{ display: 'flex', gap: '10px' }}>
                    {currentQuestion === questions.length - 1 ? (
                        <button
                            onClick={handleSubmitQuiz}
                            style={{
                                padding: '12px 24px',
                                backgroundColor: '#dc3545',
                                color: 'white',
                                border: 'none',
                                borderRadius: '4px',
                                cursor: 'pointer',
                                fontWeight: 'bold'
                            }}
                        >
                            Nộp bài
                        </button>
                    ) : (
                        <button
                            onClick={handleNextQuestion}
                            style={{
                                padding: '12px 24px',
                                backgroundColor: '#007bff',
                                color: 'white',
                                border: 'none',
                                borderRadius: '4px',
                                cursor: 'pointer'
                            }}
                        >
                            Câu tiếp →
                        </button>
                    )}
                </div>
            </div>

            {/* Question Navigator */}
            <div style={{
                marginTop: '30px',
                padding: '20px',
                backgroundColor: '#f8f9fa',
                borderRadius: '8px'
            }}>
                <h4 style={{ marginTop: 0, color: '#333' }}>Câu hỏi:</h4>
                <div style={{
                    display: 'grid',
                    gridTemplateColumns: 'repeat(10, 1fr)',
                    gap: '10px'
                }}>
                    {questions.map((_, index) => (
                        <button
                            key={index}
                            onClick={() => setCurrentQuestion(index)}
                            style={{
                                padding: '10px',
                                border: '2px solid',
                                borderColor: currentQuestion === index ? '#007bff' : 
                                           answers[questions[index].id] !== undefined ? '#28a745' : '#ddd',
                                backgroundColor: currentQuestion === index ? '#007bff' : 
                                               answers[questions[index].id] !== undefined ? '#28a745' : 'white',
                                color: currentQuestion === index || answers[questions[index].id] !== undefined ? 'white' : '#333',
                                borderRadius: '4px',
                                cursor: 'pointer',
                                fontWeight: 'bold'
                            }}
                        >
                            {index + 1}
                        </button>
                    ))}
                </div>
            </div>
        </div>
    );
}

export default Quiz;
