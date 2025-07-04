import React, { useState, useEffect } from 'react';
import axios from 'axios';

function Home({ user, onLogout }) {
    const [courses, setCourses] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetchCourses();
    }, []);

    const fetchCourses = async () => {
        try {
            const response = await axios.get('http://localhost:5032/api/course');
            setCourses(response.data);
        } catch (error) {
            console.error('Error fetching courses:', error);
            alert('Không thể tải danh sách khóa học');
        } finally {
            setLoading(false);
        }
    };

    const handleEnrollCourse = async (courseId) => {
        try {
            await axios.post('http://localhost:5032/api/course/enroll', {
                userId: user.id,
                courseId: courseId
            });
            alert('Đăng ký khóa học thành công!');
        } catch (error) {
            alert('Đăng ký khóa học thất bại');
        }
    };

    const handleLogout = () => {
        localStorage.removeItem('user');
        onLogout();
    };

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
                marginBottom: '30px',
                padding: '20px',
                backgroundColor: '#f8f9fa',
                borderRadius: '8px'
            }}>
                <div>
                    <h1 style={{ margin: 0, color: '#333' }}>JCertPre</h1>
                    <p style={{ margin: '5px 0 0 0', color: '#666' }}>
                        Xin chào, {user.username}!
                    </p>
                </div>
                <div>
                    <button
                        onClick={() => window.location.href = '/results'}
                        style={{
                            padding: '10px 20px',
                            marginRight: '10px',
                            backgroundColor: '#17a2b8',
                            color: 'white',
                            border: 'none',
                            borderRadius: '4px',
                            cursor: 'pointer'
                        }}
                    >
                        Kết quả học tập
                    </button>
                    <button
                        onClick={handleLogout}
                        style={{
                            padding: '10px 20px',
                            backgroundColor: '#dc3545',
                            color: 'white',
                            border: 'none',
                            borderRadius: '4px',
                            cursor: 'pointer'
                        }}
                    >
                        Đăng xuất
                    </button>
                </div>
            </div>

            {/* Course List */}
            <h2 style={{ color: '#333', marginBottom: '20px' }}>Danh sách khóa học</h2>
            
            <div style={{ 
                display: 'grid', 
                gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))',
                gap: '20px'
            }}>
                {courses.map(course => (
                    <div 
                        key={course.id} 
                        style={{
                            border: '1px solid #ddd',
                            borderRadius: '8px',
                            padding: '20px',
                            backgroundColor: 'white',
                            boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
                        }}
                    >
                        <h3 style={{ color: '#333', marginTop: 0 }}>{course.name}</h3>
                        <p style={{ color: '#666', marginBottom: '15px' }}>
                            Loại: {course.type}
                        </p>
                        <p style={{ color: '#666', marginBottom: '20px' }}>
                            Khóa học luyện thi chứng chỉ tiếng Nhật với nội dung được cập nhật thường xuyên.
                        </p>
                        
                        <div style={{ display: 'flex', gap: '10px' }}>
                            <button
                                onClick={() => window.location.href = `/course/${course.id}`}
                                style={{
                                    flex: 1,
                                    padding: '10px',
                                    backgroundColor: '#007bff',
                                    color: 'white',
                                    border: 'none',
                                    borderRadius: '4px',
                                    cursor: 'pointer'
                                }}
                            >
                                Xem chi tiết
                            </button>
                            <button
                                onClick={() => handleEnrollCourse(course.id)}
                                style={{
                                    flex: 1,
                                    padding: '10px',
                                    backgroundColor: '#28a745',
                                    color: 'white',
                                    border: 'none',
                                    borderRadius: '4px',
                                    cursor: 'pointer'
                                }}
                            >
                                Đăng ký
                            </button>
                        </div>
                        
                        <button
                            onClick={() => window.location.href = `/quiz/${course.id}`}
                            style={{
                                width: '100%',
                                marginTop: '10px',
                                padding: '10px',
                                backgroundColor: '#ffc107',
                                color: '#212529',
                                border: 'none',
                                borderRadius: '4px',
                                cursor: 'pointer'
                            }}
                        >
                            Thi thử
                        </button>
                    </div>
                ))}
            </div>

            {courses.length === 0 && (
                <div style={{ 
                    textAlign: 'center', 
                    padding: '50px',
                    color: '#666'
                }}>
                    Chưa có khóa học nào
                </div>
            )}
        </div>
    );
}

export default Home;
