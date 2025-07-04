import React, { useState, useEffect } from 'react';

function Results({ user }) {
    const [results, setResults] = useState([]);
    const [loading, setLoading] = useState(true);
    const [filter, setFilter] = useState('all'); // all, passed, failed

    useEffect(() => {
        const fetchResults = () => {
            try {
                // Lấy kết quả từ localStorage (trong thực tế sẽ gọi API)
                const savedResults = JSON.parse(localStorage.getItem('quizResults') || '[]');
                const userResults = savedResults.filter(result => result.userId === user.id);
                
                // Sắp xếp theo ngày mới nhất
                userResults.sort((a, b) => new Date(b.date) - new Date(a.date));
                
                setResults(userResults);
            } catch (error) {
                console.error('Error fetching results:', error);
                alert('Không thể tải kết quả học tập');
            } finally {
                setLoading(false);
            }
        };

        fetchResults();
    }, [user.id]);

    const getFilteredResults = () => {
        switch (filter) {
            case 'passed':
                return results.filter(result => result.score >= 70);
            case 'failed':
                return results.filter(result => result.score < 70);
            default:
                return results;
        }
    };

    const getScoreColor = (score) => {
        if (score >= 80) return '#28a745'; // green
        if (score >= 70) return '#ffc107'; // yellow
        return '#dc3545'; // red
    };

    const getGrade = (score) => {
        if (score >= 90) return 'A';
        if (score >= 80) return 'B';
        if (score >= 70) return 'C';
        if (score >= 60) return 'D';
        return 'F';
    };

    const calculateStats = () => {
        if (results.length === 0) return null;

        const totalTests = results.length;
        const passedTests = results.filter(r => r.score >= 70).length;
        const avgScore = Math.round(results.reduce((sum, r) => sum + r.score, 0) / totalTests);
        const bestScore = Math.max(...results.map(r => r.score));

        return {
            totalTests,
            passedTests,
            passRate: Math.round((passedTests / totalTests) * 100),
            avgScore,
            bestScore
        };
    };

    const stats = calculateStats();

    if (loading) {
        return (
            <div style={{ textAlign: 'center', marginTop: '100px' }}>
                <div>Đang tải...</div>
            </div>
        );
    }

    return (
        <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '20px' }}>
            {/* Header */}
            <div style={{ 
                display: 'flex', 
                justifyContent: 'space-between',
                alignItems: 'center',
                marginBottom: '30px'
            }}>
                <h1 style={{ color: '#333' }}>Kết quả học tập</h1>
                <button
                    onClick={() => window.location.href = '/'}
                    style={{
                        padding: '10px 20px',
                        backgroundColor: '#6c757d',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: 'pointer'
                    }}
                >
                    ← Về trang chủ
                </button>
            </div>

            {/* Statistics */}
            {stats && (
                <div style={{
                    display: 'grid',
                    gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
                    gap: '20px',
                    marginBottom: '30px'
                }}>
                    <div style={{
                        backgroundColor: '#e3f2fd',
                        padding: '20px',
                        borderRadius: '8px',
                        textAlign: 'center'
                    }}>
                        <h3 style={{ margin: '0 0 10px 0', color: '#1976d2' }}>
                            {stats.totalTests}
                        </h3>
                        <p style={{ margin: 0, color: '#666' }}>Tổng số bài thi</p>
                    </div>

                    <div style={{
                        backgroundColor: '#e8f5e8',
                        padding: '20px',
                        borderRadius: '8px',
                        textAlign: 'center'
                    }}>
                        <h3 style={{ margin: '0 0 10px 0', color: '#388e3c' }}>
                            {stats.passedTests}
                        </h3>
                        <p style={{ margin: 0, color: '#666' }}>Số bài đạt (≥70%)</p>
                    </div>

                    <div style={{
                        backgroundColor: '#fff3e0',
                        padding: '20px',
                        borderRadius: '8px',
                        textAlign: 'center'
                    }}>
                        <h3 style={{ margin: '0 0 10px 0', color: '#f57c00' }}>
                            {stats.passRate}%
                        </h3>
                        <p style={{ margin: 0, color: '#666' }}>Tỷ lệ đậu</p>
                    </div>

                    <div style={{
                        backgroundColor: '#fce4ec',
                        padding: '20px',
                        borderRadius: '8px',
                        textAlign: 'center'
                    }}>
                        <h3 style={{ margin: '0 0 10px 0', color: '#c2185b' }}>
                            {stats.avgScore}
                        </h3>
                        <p style={{ margin: 0, color: '#666' }}>Điểm trung bình</p>
                    </div>

                    <div style={{
                        backgroundColor: '#f3e5f5',
                        padding: '20px',
                        borderRadius: '8px',
                        textAlign: 'center'
                    }}>
                        <h3 style={{ margin: '0 0 10px 0', color: '#7b1fa2' }}>
                            {stats.bestScore}
                        </h3>
                        <p style={{ margin: 0, color: '#666' }}>Điểm cao nhất</p>
                    </div>
                </div>
            )}

            {/* Filter */}
            <div style={{
                display: 'flex',
                gap: '10px',
                marginBottom: '20px'
            }}>
                <button
                    onClick={() => setFilter('all')}
                    style={{
                        padding: '8px 16px',
                        backgroundColor: filter === 'all' ? '#007bff' : '#f8f9fa',
                        color: filter === 'all' ? 'white' : '#333',
                        border: '1px solid #ddd',
                        borderRadius: '4px',
                        cursor: 'pointer'
                    }}
                >
                    Tất cả ({results.length})
                </button>
                <button
                    onClick={() => setFilter('passed')}
                    style={{
                        padding: '8px 16px',
                        backgroundColor: filter === 'passed' ? '#28a745' : '#f8f9fa',
                        color: filter === 'passed' ? 'white' : '#333',
                        border: '1px solid #ddd',
                        borderRadius: '4px',
                        cursor: 'pointer'
                    }}
                >
                    Đạt ({results.filter(r => r.score >= 70).length})
                </button>
                <button
                    onClick={() => setFilter('failed')}
                    style={{
                        padding: '8px 16px',
                        backgroundColor: filter === 'failed' ? '#dc3545' : '#f8f9fa',
                        color: filter === 'failed' ? 'white' : '#333',
                        border: '1px solid #ddd',
                        borderRadius: '4px',
                        cursor: 'pointer'
                    }}
                >
                    Chưa đạt ({results.filter(r => r.score < 70).length})
                </button>
            </div>

            {/* Results List */}
            {getFilteredResults().length > 0 ? (
                <div style={{
                    display: 'grid',
                    gap: '15px'
                }}>
                    {getFilteredResults().map((result, index) => (
                        <div
                            key={index}
                            style={{
                                border: '1px solid #ddd',
                                borderRadius: '8px',
                                padding: '20px',
                                backgroundColor: 'white',
                                boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
                            }}
                        >
                            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                                <div style={{ flex: 1 }}>
                                    <h3 style={{ 
                                        margin: '0 0 10px 0', 
                                        color: '#333' 
                                    }}>
                                        {result.courseName}
                                    </h3>
                                    <div style={{ display: 'flex', gap: '20px', flexWrap: 'wrap' }}>
                                        <span style={{ color: '#666' }}>
                                            📅 {new Date(result.date).toLocaleDateString('vi-VN')}
                                        </span>
                                        <span style={{ color: '#666' }}>
                                            🕒 {new Date(result.date).toLocaleTimeString('vi-VN')}
                                        </span>
                                        <span style={{ color: '#666' }}>
                                            📝 {result.correctAnswers}/{result.totalQuestions} câu đúng
                                        </span>
                                    </div>
                                </div>

                                <div style={{ textAlign: 'right' }}>
                                    <div style={{
                                        fontSize: '24px',
                                        fontWeight: 'bold',
                                        color: getScoreColor(result.score),
                                        marginBottom: '5px'
                                    }}>
                                        {result.score}/100
                                    </div>
                                    <div style={{
                                        display: 'inline-block',
                                        padding: '4px 12px',
                                        borderRadius: '20px',
                                        backgroundColor: getScoreColor(result.score),
                                        color: 'white',
                                        fontSize: '14px',
                                        fontWeight: 'bold'
                                    }}>
                                        {getGrade(result.score)}
                                    </div>
                                </div>
                            </div>

                            {/* Progress Bar */}
                            <div style={{ marginTop: '15px' }}>
                                <div style={{
                                    width: '100%',
                                    backgroundColor: '#e9ecef',
                                    borderRadius: '10px',
                                    height: '8px'
                                }}>
                                    <div style={{
                                        width: `${result.score}%`,
                                        backgroundColor: getScoreColor(result.score),
                                        height: '100%',
                                        borderRadius: '10px',
                                        transition: 'width 0.3s'
                                    }}></div>
                                </div>
                            </div>

                            {/* Status */}
                            <div style={{ marginTop: '10px' }}>
                                {result.score >= 70 ? (
                                    <span style={{
                                        color: '#28a745',
                                        fontWeight: 'bold',
                                        fontSize: '14px'
                                    }}>
                                        ✅ ĐẠT - Chúc mừng!
                                    </span>
                                ) : (
                                    <span style={{
                                        color: '#dc3545',
                                        fontWeight: 'bold',
                                        fontSize: '14px'
                                    }}>
                                        ❌ CHƯA ĐẠT - Hãy cố gắng thêm!
                                    </span>
                                )}
                            </div>
                        </div>
                    ))}
                </div>
            ) : (
                <div style={{
                    textAlign: 'center',
                    padding: '50px',
                    backgroundColor: '#f8f9fa',
                    borderRadius: '8px',
                    color: '#666'
                }}>
                    {results.length === 0 ? (
                        <>
                            <h3>Chưa có kết quả nào</h3>
                            <p>Bạn chưa làm bài thi nào. Hãy bắt đầu học tập!</p>
                            <button
                                onClick={() => window.location.href = '/'}
                                style={{
                                    padding: '12px 24px',
                                    backgroundColor: '#007bff',
                                    color: 'white',
                                    border: 'none',
                                    borderRadius: '4px',
                                    cursor: 'pointer'
                                }}
                            >
                                Về trang chủ
                            </button>
                        </>
                    ) : (
                        <>
                            <h3>Không có kết quả phù hợp</h3>
                            <p>Thử thay đổi bộ lọc để xem kết quả khác.</p>
                        </>
                    )}
                </div>
            )}

            {/* Action Buttons */}
            {results.length > 0 && (
                <div style={{
                    textAlign: 'center',
                    marginTop: '30px',
                    padding: '20px',
                    backgroundColor: '#f8f9fa',
                    borderRadius: '8px'
                }}>
                    <h4 style={{ color: '#333', marginTop: 0 }}>Tiếp tục học tập</h4>
                    <p style={{ color: '#666', marginBottom: '20px' }}>
                        Luyện tập thêm để cải thiện kết quả của bạn!
                    </p>
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
                        Làm bài thi mới
                    </button>
                    <button
                        onClick={() => {
                            // eslint-disable-next-line no-restricted-globals
                            if (confirm('Bạn có muốn xóa toàn bộ lịch sử kết quả?')) {
                                localStorage.removeItem('quizResults');
                                setResults([]);
                            }
                        }}
                        style={{
                            padding: '12px 24px',
                            backgroundColor: '#dc3545',
                            color: 'white',
                            border: 'none',
                            borderRadius: '4px',
                            cursor: 'pointer'
                        }}
                    >
                        Xóa lịch sử
                    </button>
                </div>
            )}
        </div>
    );
}

export default Results;
