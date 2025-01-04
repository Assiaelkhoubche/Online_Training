
import { useState } from "react";
import { useNavigate } from "react-router-dom"
import axios from "axios"
import {useAuth } from "../Components/Authentication/AuthContext"
function EditProfile() {

    const navigate = useNavigate();
    const [userName, setUserName] = useState("");
    const { setUser } = useAuth();

    const handleSubmit =async (e) => {

        e.preventDefault();

        const payload = {

            userName:userName
        }
        alert(payload);
        try {

            const res = await axios.post("/edite-profile", payload, {
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (res.status === 200) {
                console.log(res.data.user);
                setUser(res.data.user);
                navigate("/profile");
            }

        } catch (err) {
            console.log("error edite profile", err.message);
        }

    }

    return (

        <div>
            <form onSubmit={handleSubmit}>
                <div>

                    <label> UserName</label>  
                    <input
                        type="text"
                        id="userName"
                        value={userName}
                        onChange={(e) => { setUserName(e.target.value) }}
                        required
                    />

                </div>
                <button type="submit">Submit</button>
            </form>
        </div>

    )
}

export default EditProfile;