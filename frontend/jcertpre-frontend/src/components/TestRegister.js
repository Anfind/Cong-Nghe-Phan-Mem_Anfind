import React, { useState } from 'react';

function TestRegister() {
    const [formData, setFormData] = useState({
        username: '',
        password: '',
        confirmPassword: '',
        fullName: '',
        email: ''
    });
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState('');

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const handleRegister = async () => {
        console.log('🚀 Test Register button clicked!');
        setMessage('Button clicked! Check console for logs.');
        
        const { username, password, confirmPassword, fullName, email } = formData;
        console.log('📝 Form data:', formData);

        // Basic validation
        if (!username || !password || !confirmPassword || !fullName || !email) {
            setMessage('❌ Vui lòng điền đầy đủ thông tin');
            return;
        }

        if (password !== confirmPassword) {
            setMessage('❌ Mật khẩu xác nhận không khớp');
            return;
        }

        if (password.length < 6) {
            setMessage('❌ Mật khẩu phải có ít nhất 6 ký tự');
            return;
        }

        setLoading(true);
        setMessage('⏳ Đang xử lý đăng ký...');
        
        try {
            console.log('🌐 Sending POST request to backend...');
            
            const response = await fetch('http://localhost:5032/api/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    username,
                    password,
                    fullName,
                    email
                })
            });

            console.log('📡 Response status:', response.status);
            console.log('📡 Response ok:', response.ok);

            if (!response.ok) {
                const errorText = await response.text();
                console.error('❌ Error response:', errorText);
                throw new Error(`HTTP ${response.status}: ${errorText}`);
            }

            const result = await response.json();
            console.log('✅ Success response:', result);
            
            setMessage('✅ Đăng ký thành công! ' + JSON.stringify(result));
            
            // Reset form
            setFormData({
                username: '',
                password: '',
                confirmPassword: '',
                fullName: '',
                email: ''
            });
            
        } catch (error) {
            console.error('💥 Fetch error:', error);
            setMessage(`💥 Lỗi: ${error.message}`);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ 
            maxWidth: '500px', 
            margin: '50px auto', 
            padding: '30px',
            border: '2px solid #007bff',
            borderRadius: '10px',
            backgroundColor: '#f8f9fa'
        }}>
            <h2 style={{ textAlign: 'center', color: '#007bff' }}>
                🧪 Test Register Component
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
                    transition: 'all 0.3s ease'
                }}
            >
                {loading ? '⏳ Đang đăng ký...' : '🧪 Test Đăng ký'}
            </button>
            
            {message && (
                <div style={{ 
                    marginTop: '20px', 
                    padding: '15px',
                    backgroundColor: message.includes('✅') ? '#d4edda' : 
                                    message.includes('❌') || message.includes('💥') ? '#f8d7da' : '#fff3cd',
                    border: '1px solid ' + (message.includes('✅') ? '#c3e6cb' : 
                                           message.includes('❌') || message.includes('💥') ? '#f5c6cb' : '#ffeaa7'),
                    borderRadius: '4px',
                    fontSize: '14px',
                    wordBreak: 'break-word'
                }}>
                    {message}
                </div>
            )}
            
            <div style={{ marginTop: '20px', fontSize: '12px', color: '#666' }}>
                <strong>Debug Info:</strong>
                <ul>
                    <li>Kiểm tra Console (F12) để xem logs chi tiết</li>
                    <li>Backend cần chạy tại: http://localhost:5032</li>
                    <li>Test health: <a href="http://localhost:5032/api/health" target="_blank" rel="noopener noreferrer">http://localhost:5032/api/health</a></li>
                </ul>
            </div>
        </div>
    );
}

export default TestRegister;
