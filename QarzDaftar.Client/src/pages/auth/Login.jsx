import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import { FaEye, FaEyeSlash } from "react-icons/fa";
import "./Auth.css";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function Login({ setUser }) {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch(`${API_BASE_URL}/Auth/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password })
            });

            if (!response.ok) throw new Error("Login failed");

            const data = await response.json();
            const token = data.accessToken;

            localStorage.setItem("accessToken", token);

            const decoded = jwtDecode(token);
            const role =
                decoded.role ||
                decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

            const userId =
                decoded.userId ||
                decoded.sub ||
                decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ||
                null;

            const fullName =
                decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]?.trim() ||
                decoded.name?.trim() ||
                decoded.fullName?.trim() ||
                decoded.username?.trim() ||
                username;

            const userData = { userId, fullName, role };
            localStorage.setItem("user", JSON.stringify(userData));

            setUser(userData);

            if (role === "SuperAdmin") {
                navigate("/ceo/dashboard");
            } else if (role === "Admin") {
                navigate("/user/dashboard");
            } else {
                navigate("/");
            }
        } catch (error) {
            alert("Login xatoligi: " + error.message);
        }
    };

    return (
        <div className="auth-container">
            <h2>Kirish</h2>
            <form onSubmit={handleLogin} className="auth-form">
                <input
                    type="text"
                    placeholder="Username"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                />

                <div className="password-field">
                    <input
                        type={showPassword ? "text" : "password"}
                        placeholder="Parol"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    />
                    <span
                        className="toggle-password"
                        onClick={() => setShowPassword(!showPassword)}
                    >
                        {showPassword ? <FaEye /> : <FaEyeSlash />}
                    </span>
                </div>

                <button type="submit">Kirish</button>
            </form>

            <p className="auth-footer">
                Hisobingiz yo'qmi? <Link to="/register">Ro'yxatdan o'tish</Link>
            </p>
        </div>
    );
}

export default Login;