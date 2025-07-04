import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';

function CourseDetail({ user }) {
    const { courseId } = useParams();
    const [course, setCourse] = useState(null);
    const [loading, setLoading] = useState(true);
    const [currentVideo, setCurrentVideo] = useState(0);

    // Mock video data (trong thực tế sẽ lấy từ API)
    const mockVideos = [
        {
            id: 1,
            title: 'Bài 1: Giới thiệu về JLPT/NAT-TEST',
            url: 'https://www.youtube.com/embed/dQw4w9WgXcQ',
            duration: '15:30'
        },
        {
            id: 2,
            title: 'Bài 2: Ngữ pháp cơ bản',
            url: 'https://www.youtube.com/embed/dQw4w9WgXcQ',
            duration: '22:45'
        },
        {
            id: 3,
            title: 'Bài 3: Từ vựng thường gặp',
            url: 'https://www.youtube.com/embed/dQw4w9WgXcQ',
            duration: '18:20'
        }
    ];

    useEffect(() => {
        const fetchCourseDetail = async () => {
            try {
                const response = await axios.get('http://localhost:5032/api/course');
                const courseData = response.data.find(c => c.id === parseInt(courseId));
                setCourse(courseData);
            } catch (error) {
                console.error('Error fetching course:', error);
                alert('Không thể tải thông tin khóa học');
            } finally {
                setLoading(false);
            }
        };

        fetchCourseDetail();
    }, [courseId]);

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
                <button
                    onClick={() => window.location.href = '/'}
                    style={{
                        marginTop: '20px',
                        padding: '10px 20px',
                        backgroundColor: '#007bff',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: 'pointer'
                    }}
                >
                    Về trang chủ
                </button>
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
                marginBottom: '20px'
            }}>
                <h1 style={{ color: '#333' }}>{course.name}</h1>
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

            <div style={{ display: 'flex', gap: '20px' }}>
                {/* Video Player */}
                <div style={{ flex: 2 }}>
                    <div style={{
                        backgroundColor: '#000',
                        borderRadius: '8px',
                        overflow: 'hidden',
                        marginBottom: '20px'
                    }}>
                        <iframe
                            width="100%"
                            height="400"
                            src={mockVideos[currentVideo].url}
                            title={mockVideos[currentVideo].title}
                            frameBorder="0"
                            allowFullScreen
                            style={{ display: 'block' }}
                        ></iframe>
                    </div>

                    <h3 style={{ color: '#333' }}>
                        {mockVideos[currentVideo].title}
                    </h3>
                    <p style={{ color: '#666' }}>
                        Thời lượng: {mockVideos[currentVideo].duration}
                    </p>

                    {/* Course Description */}
                    <div style={{
                        backgroundColor: '#f8f9fa',
                        padding: '20px',
                        borderRadius: '8px',
                        marginTop: '20px'
                    }}>
                        <h4 style={{ color: '#333', marginTop: 0 }}>Mô tả khóa học</h4>
                        <p style={{ color: '#666', lineHeight: '1.6' }}>
                            Khóa học {course.name} được thiết kế đặc biệt cho những người muốn luyện thi 
                            chứng chỉ tiếng Nhật. Nội dung bao gồm ngữ pháp, từ vựng, đọc hiểu và nghe hiểu 
                            theo chuẩn {course.type === 'Video' ? 'JLPT' : 'NAT-TEST'}.
                        </p>
                        <ul style={{ color: '#666' }}>
                            <li>Video bài giảng chi tiết</li>
                            <li>Bài tập thực hành</li>
                            <li>Đề thi thử</li>
                            <li>Hỗ trợ trực tuyến</li>
                        </ul>
                    </div>
                </div>

                {/* Video List */}
                <div style={{ flex: 1 }}>
                    <h3 style={{ color: '#333' }}>Danh sách bài học</h3>
                    <div style={{ border: '1px solid #ddd', borderRadius: '8px' }}>
                        {mockVideos.map((video, index) => (
                            <div
                                key={video.id}
                                onClick={() => setCurrentVideo(index)}
                                style={{
                                    padding: '15px',
                                    borderBottom: index < mockVideos.length - 1 ? '1px solid #eee' : 'none',
                                    cursor: 'pointer',
                                    backgroundColor: currentVideo === index ? '#e3f2fd' : 'white',
                                    transition: 'background-color 0.2s'
                                }}
                                onMouseEnter={(e) => {
                                    if (currentVideo !== index) {
                                        e.target.style.backgroundColor = '#f5f5f5';
                                    }
                                }}
                                onMouseLeave={(e) => {
                                    if (currentVideo !== index) {
                                        e.target.style.backgroundColor = 'white';
                                    }
                                }}
                            >
                                <div style={{ 
                                    fontWeight: currentVideo === index ? 'bold' : 'normal',
                                    color: '#333',
                                    marginBottom: '5px'
                                }}>
                                    {video.title}
                                </div>
                                <div style={{ 
                                    fontSize: '14px', 
                                    color: '#666' 
                                }}>
                                    {video.duration}
                                </div>
                            </div>
                        ))}
                    </div>

                    {/* Action Buttons */}
                    <div style={{ marginTop: '20px' }}>
                        <button
                            onClick={() => window.location.href = `/quiz/${courseId}`}
                            style={{
                                width: '100%',
                                padding: '12px',
                                backgroundColor: '#ffc107',
                                color: '#212529',
                                border: 'none',
                                borderRadius: '4px',
                                fontSize: '16px',
                                fontWeight: 'bold',
                                cursor: 'pointer',
                                marginBottom: '10px'
                            }}
                        >
                            🎯 Làm bài thi thử
                        </button>

                        <button
                            onClick={() => window.location.href = '/results'}
                            style={{
                                width: '100%',
                                padding: '12px',
                                backgroundColor: '#17a2b8',
                                color: 'white',
                                border: 'none',
                                borderRadius: '4px',
                                fontSize: '16px',
                                cursor: 'pointer'
                            }}
                        >
                            📊 Xem kết quả học tập
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default CourseDetail;
