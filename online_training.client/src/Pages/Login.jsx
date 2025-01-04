import { useState } from "react";
import { Link, useNavigate } from 'react-router-dom';
import axios from "axios"

import {useAuth}  from "../Components/Authentication/AuthContext"

function Login() {

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const navigate = useNavigate();

    const { login } = useAuth();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setMessage('');

        const payload = {
            email:email,
            password:password
        }

        try {

            const res = await axios.post("/login", payload, {
                headers: {
                    'Content-Type': 'application/json',
                },
            })

            if (res.status === 200) {

                
                console.log(res.data);
                login(res.data.userInfo);
                setMessage("You are successely login!");
                navigate("/");
            }

        } catch (err) {
            setError("Failed To login",err);
        }
    }
    
    

    return (
        <div>
            <h2>Login</h2>
            <form onSubmit={handleSubmit}>

              
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
                <button type="submit">Login</button>
            </form>

            {error && <div className="error">{error}</div>}
            {message && <div className="success">{message}</div>}
            <p> Didn have an account <Link to="/register">register</Link></p>
        </div>
    );


}

export default Login;