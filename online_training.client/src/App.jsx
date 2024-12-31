
import './index.css';

import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Home from './Pages/Home'

import NavBar from './Components/NavBar';
import Login from './Pages/Login';
import Register from './Pages/Register';


function App() {
    
    return (
        <div>
            <h1 className="bg-blue-500">Hello assia</h1>
            
            <Router>
                <NavBar />
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                </Routes>
            </Router>
            
        </div>
    );
    
    
}

export default App;