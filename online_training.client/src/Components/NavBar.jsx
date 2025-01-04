
import { Link, useNavigate } from 'react-router-dom';
import {useAuth } from "./Authentication/AuthContext"
import axios from "axios"
import { useState } from 'react';

const NavBar = () => {

    const { isAuthenticated, user, logout } = useAuth();

    
    

    const navigate = useNavigate();

    const handleLogout = async (e) => {
        e.preventDefault();
        try {

            const res = await axios.post("/logout");

            if (res.status === 200) {

                logout();
                navigate("/login");                
            }
            

        } catch (err) {

            console.log("Error Logout", err.message);
        }
    }


    const handleTrainerRequest = async (e) => {

        e.preventDefault();

        try {

            const res = await axios.post("/trainer-request");
           

        } catch (err) {
            console.log("error trainer reques", err)
        }
        
    }
    
   
    return (
        <nav className="flex items-center justify-between bg-gray-800 p-4">
            
            <ul className="flex space-x-4">
                <li><Link to="/" className="">Home</Link></li>
                {isAuthenticated ?
                   ( <>
                        <li><Link to="/profile">Profile</Link></li>
                        <li><button onClick={handleLogout} >Logout</button></li>

                        {user.roles == "User" &&
                                <li>
                                         <button onClick={handleTrainerRequest} >
                                               {
                                                  user.haspendingRequest ? "Pending":"Become a Trainer"

                                                }
                                        </button>
                                </li>
                        }
                        {
                            user.roles == "Administrator" &&
                            <li><Link to="/requests-Admin">requests</Link></li>
                        }
                   </>)
                    :
                    (<li><Link to="/login" className="">Login</Link></li>)
                }
                <li><Link to="/register" className="">Sign Up</Link></li>
                {isAuthenticated && user != null && <li> <p> Hello @{user.userName} you are { user.roles}</p></li>  }
            </ul>
        </nav>
    );
};

export default NavBar;
