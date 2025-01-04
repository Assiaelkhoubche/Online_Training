
import axios from "axios";
import { useEffect, useState } from "react"



function RequestsAdmin()
{
    const [userRequestsInfo, setUserRequestsInfo] = useState([]);
    

    useEffect(() => {

        fetchRequest();
       
    }, []);


    const fetchRequest = async () => {
        try {
            const res = await axios.get("/getTrainers-request");

            if (res.status === 200) {

                console.log("requests: ", res.data.userRequests);
                setUserRequestsInfo(res.data.usersRequests);
            }

        } catch (err) {

            console.log("error geting request", err)
        }

    }

    const handleApproved =async (user) => {

        const payload = {
            userId:user.userId
        }
        try {

            const res = axios.post("/approve-request", payload, {
                headers: {
                    "Content-Type": "application/json"
                }
            });

            if (res.status === 200) {

                alert(" user approved successfuly");
            }

        } catch (err) {
            console.log("error approved user", err);
        }
        
    }
    const handleRejected = async (e) => {
        e.preventDefault();
    }

    return (
        <div>
            <h1>Requests page</h1>
            <div>
                <ul>
                    {
                        userRequestsInfo.map((user, index) => (
                            <li key={index}>
                                <p>{user.userName}</p>
                                <button onClick={()=>handleApproved(user)}>Approve</button>
                                <button onClick={handleRejected }>Rejecte</button>
                                
                            </li>
                        ))
                    }
                </ul>
            </div>
        </div>
    )
}

export default RequestsAdmin;