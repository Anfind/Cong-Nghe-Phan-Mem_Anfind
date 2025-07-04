import React, { useState } from 'react';
import axios from 'axios';

function Login({ onLogin }) {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);

    const handleLogin = async () => {
        if (!username || !password) {
            alert('Vui lòng nhập đầy đủ thông tin');
            return;
        }

        setLoading(true);
        try {
            const response = await axios.post('http://localhost:5032/api/auth/login', { 
                username, 
                password 
            });
            
            localStorage.setItem('user', JSON.stringify({
                id: response.data.userId,
                username: response.data.username || username,
                fullName: response.data.fullName
            }));
            
            alert('🎉 Đăng nhập thành công! Chào mừng bạn!');
            onLogin(response.data.userId);
        } catch (error) {
            alert('❌ Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ 
            maxWidth: '400px', 
            margin: '100px auto', 
            padding: '30px',
            border: '1px solid #ddd',
            borderRadius: '8px',
            boxShadow: '0 2px 10px rgba(0,0,0,0.1)'
        }}>
            <h2 style={{ textAlign: 'center', marginBottom: '30px', color: '#333' }}>
                Đăng nhập JCertPre
            </h2>
            
            <div style={{ marginBottom: '20px' }}>
                <input
                    type="text"
                    placeholder="Tên đăng nhập"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
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
                    placeholder="Mật khẩu"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
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
                onClick={handleLogin}
                disabled={loading}
                style={{ 
                    width: '100%',
                    padding: '12px', 
                    backgroundColor: loading ? '#6c757d' : '#007bff', 
                    color: 'white', 
                    border: 'none',
                    borderRadius: '4px',
                    fontSize: '16px',
                    fontWeight: 'bold',
                    cursor: loading ? 'not-allowed' : 'pointer',
                    transition: 'all 0.3s ease',
                    boxShadow: loading ? 'none' : '0 2px 4px rgba(0, 123, 255, 0.3)'
                }}
                onMouseEnter={(e) => {
                    if (!loading) {
                        e.target.style.backgroundColor = '#0056b3';
                        e.target.style.transform = 'translateY(-1px)';
                        e.target.style.boxShadow = '0 4px 8px rgba(0, 123, 255, 0.4)';
                    }
                }}
                onMouseLeave={(e) => {
                    if (!loading) {
                        e.target.style.backgroundColor = '#007bff';
                        e.target.style.transform = 'translateY(0)';
                        e.target.style.boxShadow = '0 2px 4px rgba(0, 123, 255, 0.3)';
                    }
                }}
            >
                {loading ? (
                    <span>
                        ⏳ Đang đăng nhập...
                    </span>
                ) : (
                    <span>
                        🔐 Đăng nhập
                    </span>
                )}
            </button>
            
            <div style={{ textAlign: 'center', marginTop: '20px' }}>
                <span style={{ color: '#666' }}>Chưa có tài khoản? </span>
                <a href="/register" style={{ color: '#007bff', textDecoration: 'none' }}>
                    Đăng ký ngay
                </a>
            </div>
            
            <div style={{ 
                marginTop: '20px', 
                padding: '10px', 
                backgroundColor: '#f8f9fa', 
                borderRadius: '4px',
                fontSize: '14px',
                color: '#666'
            }}>
                <strong>Tài khoản demo:</strong><br/>
                Username: test<br/>
                Password: 123456
            </div>
        </div>
    );
}

export default Login;
