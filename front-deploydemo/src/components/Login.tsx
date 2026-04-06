import { useState } from "react";
import { logins } from "../services/axios";
import { useNavigate } from "react-router-dom";

function Login() {

    const navigate = useNavigate();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");

    const login = async () => {

        try {
            await logins({
                username: username,
                password: password
            });

            // Navigate after successful login
            navigate("/demo");

        } catch (error) {
            console.log(error);
        }
    };

    return (
        <div style={{ border: "2px solid red", padding: "20px", width: "300px", margin: "50px auto" }}>
            <div><h1>Login</h1></div>

            <div>
                <input
                    placeholder="username"
                    onChange={(e) => setUsername(e.target.value)}
                />
            </div>

            <div>
                <input
                    placeholder="password"
                    type="password"
                    onChange={(e) => setPassword(e.target.value)}
                />
            </div>

            <div>
                <button onClick={login}>
                    Login
                </button>
            </div>
        </div>
    );
}

export default Login;