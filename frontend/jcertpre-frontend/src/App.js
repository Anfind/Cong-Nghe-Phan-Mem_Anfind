import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import TestRegister from './components/TestRegister';
import Home from './components/Home';
import CourseDetail from './components/CourseDetail';
import Quiz from './components/Quiz';
import Results from './components/Results';

function App() {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        // Kiểm tra user đã đăng nhập từ localStorage
        const savedUser = localStorage.getItem('user');
        if (savedUser) {
            try {
                setUser(JSON.parse(savedUser));
            } catch (error) {
                localStorage.removeItem('user');
            }
        }
        setLoading(false);
    }, []);

    const handleLogin = (userId) => {
        const userData = { id: userId };
        setUser(userData);
    };

    const handleLogout = () => {
        setUser(null);
        localStorage.removeItem('user');
    };

    const handleRegister = () => {
        // Chuyển hướng về trang login sau khi đăng ký
        window.location.href = '/login';
    };

    if (loading) {
        return (
            <div style={{ 
                display: 'flex', 
                justifyContent: 'center', 
                alignItems: 'center', 
                height: '100vh',
                fontSize: '18px' 
            }}>
                Đang tải...
            </div>
        );
    }

    return (
        <Router>
            <div style={{ minHeight: '100vh', backgroundColor: '#f5f5f5' }}>
                <Routes>
                    {/* Public Routes */}
                    <Route 
                        path="/login" 
                        element={
                            user ? <Navigate to="/" replace /> : 
                            <Login onLogin={handleLogin} />
                        } 
                    />
                    <Route 
                        path="/register" 
                        element={
                            user ? <Navigate to="/" replace /> : 
                            <Register onRegister={handleRegister} />
                        } 
                    />
                    <Route 
                        path="/test-register" 
                        element={<TestRegister />} 
                    />

                    {/* Protected Routes */}
                    <Route 
                        path="/" 
                        element={
                            user ? <Home user={user} onLogout={handleLogout} /> : 
                            <Navigate to="/login" replace />
                        } 
                    />
                    <Route 
                        path="/course/:courseId" 
                        element={
                            user ? <CourseDetail user={user} /> : 
                            <Navigate to="/login" replace />
                        } 
                    />
                    <Route 
                        path="/quiz/:courseId" 
                        element={
                            user ? <Quiz user={user} /> : 
                            <Navigate to="/login" replace />
                        } 
                    />
                    <Route 
                        path="/results" 
                        element={
                            user ? <Results user={user} /> : 
                            <Navigate to="/login" replace />
                        } 
                    />

                    {/* Fallback Route */}
                    <Route 
                        path="*" 
                        element={<Navigate to={user ? "/" : "/login"} replace />} 
                    />
                </Routes>
            </div>
        </Router>
    );
}

export default App;