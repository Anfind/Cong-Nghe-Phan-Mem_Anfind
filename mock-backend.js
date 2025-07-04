const express = require('express');
const cors = require('cors');
const app = express();
const PORT = 5032;

// Middleware
app.use(cors());
app.use(express.json());

// Mock data
let users = [
    {
        id: 1,
        username: 'testuser',
        password: 'password123',
        fullName: 'Test User',
        email: 'test@example.com'
    }
];

// Register endpoint
app.post('/api/auth/register', (req, res) => {
    console.log('Register request received:', req.body);
    
    const { username, password, fullName, email } = req.body;
    
    // Validation
    if (!username || !password || !fullName || !email) {
        return res.status(400).json({ message: 'Vui lòng điền đầy đủ thông tin' });
    }
    
    // Check if user exists
    const existingUser = users.find(u => u.username === username || u.email === email);
    if (existingUser) {
        return res.status(400).json({ message: 'Tên đăng nhập hoặc email đã tồn tại' });
    }
    
    // Create new user
    const newUser = {
        id: users.length + 1,
        username,
        password,
        fullName,
        email
    };
    
    users.push(newUser);
    
    console.log('User registered successfully:', newUser);
    
    res.status(201).json({ 
        message: 'Đăng ký thành công',
        user: {
            id: newUser.id,
            username: newUser.username,
            fullName: newUser.fullName,
            email: newUser.email
        }
    });
});

// Login endpoint
app.post('/api/auth/login', (req, res) => {
    console.log('Login request received:', req.body);
    
    const { username, password } = req.body;
    
    const user = users.find(u => u.username === username && u.password === password);
    
    if (!user) {
        return res.status(401).json({ message: 'Tên đăng nhập hoặc mật khẩu không đúng' });
    }
    
    res.json({
        message: 'Đăng nhập thành công',
        user: {
            id: user.id,
            username: user.username,
            fullName: user.fullName,
            email: user.email
        }
    });
});

// Health check
app.get('/api/health', (req, res) => {
    res.json({ status: 'OK', message: 'Mock backend is running' });
});

// Get all users (for testing)
app.get('/api/users', (req, res) => {
    res.json(users.map(u => ({
        id: u.id,
        username: u.username,
        fullName: u.fullName,
        email: u.email
    })));
});

app.listen(PORT, () => {
    console.log(`Mock backend server is running on http://localhost:${PORT}`);
    console.log('Available endpoints:');
    console.log('- POST /api/auth/register');
    console.log('- POST /api/auth/login');
    console.log('- GET /api/health');
    console.log('- GET /api/users');
});
