import  { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from "axios"

const Register = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [userName, setUserName] = useState('');
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setMessage('');

        const payload = {
            email: email,
            userName: userName,
            password: password
        };

        try {

            const res = await axios.post("/register", payload, {
                headers: {
                    'Content-Type': 'application/json', 
                },
            });

            if (res.status === 200) {
                setMessage("Successful creation!");
                navigate("/login");
            }
            
        } catch (err) {
            setError(err.message || 'An error occurred');
            console.log(err.response);
        }
    };

    return (
        <div>
            <h2>Register</h2>
            <form onSubmit={handleSubmit}>
                
                <div>
                    <label>userName</label>
                    <input
                        type="text"
                        value={userName}
                        onChange={(e) => setUserName(e.target.value)}
                        
                    />
                </div>
                <div>
                    <label>Email</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    />
                </div>
                <div>
                    <label>Password</label>
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                </div>
                <button type="submit">Register</button>
            </form>

            {error && <div className="error">{error}</div>}
            {message && <div className="success">{message}</div>}
            <p>Already have an account <Link to="/login">Login</Link></p>
        </div>
    );
};

export default Register;
