
import { Link } from "react-router-dom"
import { useAuth } from "../Components/Authentication/AuthContext";
function Profile() {

    const { user } = useAuth();

    if (!user) {
        return <div> <p>Loading...</p></div>
    }

    return (
        <div>
            <h1>Profile</h1>
            <p>Email: {user.email}</p>
            <p>User Name: {user.userName}</p>
            <p>Role: {user.roles}</p>

            <Link to="/edite-profile">Edite</Link>
        </div>
    )
}

export default Profile;