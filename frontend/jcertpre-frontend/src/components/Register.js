import React, { useState } from 'react';
import axios from 'axios';

function Register({ onRegister }) {
    const [formData, setFormData] = useState({
        username: '',
        password: '',
        confirmPassword: '',
        fullName: '',
        email: ''
    });
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const handleRegister = async () => {
        console.log('Register button clicked!'); // Debug log
        
        const { username, password, confirmPassword, fullName, email } = formData;

        console.log('Form data:', formData); // Debug log

        // Validation
        if (!username || !password || !confirmPassword || !fullName || !email) {
            alert('Vui lòng điền đầy đủ thông tin');
            return;
        }

        if (password !== confirmPassword) {
            alert('Mật khẩu xác nhận không khớp');
            return;
        }

        if (password.length < 6) {
            alert('Mật khẩu phải có ít nhất 6 ký tự');
            return;
        }

        setLoading(true);
        console.log('Sending register request...'); // Debug log
        
        try {
            const response = await axios.post('http://localhost:5032/api/auth/register', {
                username,
                password,
                fullName,
                email
            });
            
            console.log('Register response:', response.data); // Debug log
            
            // Show success state
            setSuccess(true);
            
            // Show success message
            alert('🎉 Đăng ký thành công! Chuyển về trang đăng nhập...');
            
            // Reset form
            setFormData({
                username: '',
                password: '',
                confirmPassword: '',
                fullName: '',
                email: ''
            });
            
            // Delay to show success state then redirect
            setTimeout(() => {
                window.location.href = '/login';
            }, 1000);
            
        } catch (error) {
            console.error('Register error:', error); // Debug log
            console.error('Error response:', error.response); // Debug log
            
            let errorMessage = 'Đăng ký thất bại. Vui lòng thử lại.';
            
            if (error.response?.data?.message) {
                errorMessage = error.response.data.message;
            } else if (error.response?.data) {
                errorMessage = error.response.data;
            } else if (error.message) {
                errorMessage = `Lỗi kết nối: ${error.message}`;
            }
            
            alert(errorMessage);
            setSuccess(false);
        } finally {
            setLoading(false);
        }
    };

    // Success state UI
    if (success) {
        return (
            <div style={{ 
                maxWidth: '400px', 
                margin: '100px auto', 
                padding: '30px',
                border: '2px solid #28a745',
                borderRadius: '8px',
                backgroundColor: '#d4edda',
                textAlign: 'center'
            }}>
                <div style={{ fontSize: '48px', marginBottom: '20px' }}>✅</div>
                <h2 style={{ color: '#155724', marginBottom: '15px' }}>
                    Đăng ký thành công!
                </h2>
                <p style={{ color: '#155724', marginBottom: '20px' }}>
                    Đang chuyển về trang đăng nhập...
                </p>
                <div style={{
                    width: '100%',
                    height: '4px',
                    backgroundColor: '#c3e6cb',
                    borderRadius: '2px',
                    overflow: 'hidden'
                }}>
                    <div style={{
                        width: '100%',
                        height: '100%',
                        backgroundColor: '#28a745',
                        animation: 'progress 1s ease-in-out'
                    }}></div>
                </div>
                <style>{`
                    @keyframes progress {
                        from { width: 0%; }
                        to { width: 100%; }
                    }
                `}</style>
            </div>
        );
    }

    return (
        <div style={{ 
            maxWidth: '400px', 
            margin: '50px auto', 
            padding: '30px',
            border: '1px solid #ddd',
            borderRadius: '8px',
            boxShadow: '0 2px 10px rgba(0,0,0,0.1)'
        }}>
            <h2 style={{ textAlign: 'center', marginBottom: '30px', color: '#333' }}>
                Đăng ký JCertPre
            </h2>
            
            <div style={{ marginBottom: '15px' }}>
                <input
                    type="text"
                    name="fullName"
                    placeholder="Họ và tên"
                    value={formData.fullName}
                    onChange={handleChange}
                    style={{ 
                        width: '100%', 
                        padding: '12px', 
                        border: '1px solid #ddd',
                        borderRadius: '4px',
                        fontSize: '16px',
                        boxSizing: 'border-box'
                    }}
                />
            </div>

            <div style={{ marginBottom: '15px' }}>
                <input
                    type="email"
                    name="email"
                    placeholder="Email"
                    value={formData.email}
                    onChange={handleChange}
                    style={{ 
                        width: '100%', 
                        padding: '12px', 
                        border: '1px solid #ddd',
                        borderRadius: '4px',
                        fontSize: '16px',
                        boxSizing: 'border-box'
                    }}
                />
            </div>
            
            <div style={{ marginBottom: '15px' }}>
                <input
                    type="text"
                    name="username"
                    placeholder="Tên đăng nhập"
                    value={formData.username}
                    onChange={handleChange}
                    style={{ 
                        width: '100%', 
                        padding: '12px', 
                        border: '1px solid #ddd',
                        borderRadius: '4px',
                        fontSize: '16px',
                        boxSizing: 'border-box'
                    }}
                />
            </div>
            
            <div style={{ marginBottom: '15px' }}>
                <input
                    type="password"
                    name="password"
                    placeholder="Mật khẩu (ít nhất 6 ký tự)"
                    value={formData.password}
                    onChange={handleChange}
                    style={{ 
                        width: '100%', 
                        padding: '12px', 
                        border: '1px solid #ddd',
                        borderRadius: '4px',
                        fontSize: '16px',
                        boxSizing: 'border-box'
                    }}
                />
            </div>

            <div style={{ marginBottom: '20px' }}>
                <input
                    type="password"
                    name="confirmPassword"
                    placeholder="Xác nhận mật khẩu"
                    value={formData.confirmPassword}
                    onChange={handleChange}
                    style={{ 
                        width: '100%', 
                        padding: '12px', 
                        border: '1px solid #ddd',
                        borderRadius: '4px',
                        fontSize: '16px',
                        boxSizing: 'border-box'
                    }}
                />
            </div>
            
            <button
                onClick={handleRegister}
                disabled={loading}
                style={{ 
                    width: '100%',
                    padding: '14px', 
                    backgroundColor: loading ? '#6c757d' : '#28a745', 
                    color: 'white', 
                    border: 'none',
                    borderRadius: '6px',
                    fontSize: '16px',
                    fontWeight: 'bold',
                    cursor: loading ? 'not-allowed' : 'pointer',
                    transition: 'all 0.3s ease',
                    transform: loading ? 'none' : 'translateY(0)',
                    boxShadow: loading ? 'none' : '0 2px 4px rgba(40, 167, 69, 0.3)',
                    outline: 'none',
                    position: 'relative',
                    overflow: 'hidden'
                }}
                onMouseEnter={(e) => {
                    if (!loading) {
                        e.target.style.backgroundColor = '#218838';
                        e.target.style.transform = 'translateY(-1px)';
                        e.target.style.boxShadow = '0 4px 8px rgba(40, 167, 69, 0.4)';
                    }
                }}
                onMouseLeave={(e) => {
                    if (!loading) {
                        e.target.style.backgroundColor = '#28a745';
                        e.target.style.transform = 'translateY(0)';
                        e.target.style.boxShadow = '0 2px 4px rgba(40, 167, 69, 0.3)';
                    }
                }}
                onMouseDown={(e) => {
                    if (!loading) {
                        e.target.style.transform = 'translateY(1px)';
                    }
                }}
                onMouseUp={(e) => {
                    if (!loading) {
                        e.target.style.transform = 'translateY(-1px)';
                    }
                }}
            >
                {loading ? (
                    <span style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                        <span style={{ 
                            animation: 'spin 1s linear infinite',
                            marginRight: '8px',
                            display: 'inline-block'
                        }}>⏳</span>
                        Đang đăng ký...
                    </span>
                ) : (
                    <span style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                        <span style={{ marginRight: '8px' }}>📝</span>
                        Đăng ký tài khoản
                    </span>
                )}
            </button>
            
            <style>{`
                @keyframes spin {
                    from { transform: rotate(0deg); }
                    to { transform: rotate(360deg); }
                }
                @keyframes progress {
                    from { width: 0%; }
                    to { width: 100%; }
                }
            `}</style>
            
            <div style={{ textAlign: 'center', marginTop: '20px' }}>
                <span style={{ color: '#666' }}>Đã có tài khoản? </span>
                <a href="/login" style={{ color: '#007bff', textDecoration: 'none' }}>
                    Đăng nhập ngay
                </a>
            </div>
        </div>
    );
}

export default Register;
