
import { Link } from 'react-router-dom';

const NavBar = () => {
    return (
        <nav className="flex items-center justify-between bg-gray-800 p-4">
            
            <ul className="flex space-x-4">
                <li><Link to="/" className="">Home</Link></li>
                <li><Link to="/login" className="">Login</Link></li>
                <li><Link to="/register" className="">Sign Up</Link></li>
            </ul>
        </nav>
    );
};

export default NavBar;
