import { useState } from "react";
import { Link, useNavigate } from 'react-router-dom';

function Login() {

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const [message, setMessage] = useState('');
    const [error, setError] = useState('');

    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setMessage('');

        try {

            const response = await fetch(`/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    email,
                    password,
                   


                }),
            }).then(result => {

                setMessage("successful !");
                navigate("/");

            });

        } catch (err) {
            setError(err.message);
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
            <p> Didn't have an account <Link to="/register">register</Link></p>
        </div>
    );


}

export default Login;