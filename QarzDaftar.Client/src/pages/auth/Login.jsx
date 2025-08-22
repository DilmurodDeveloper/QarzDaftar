import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import { FaEye, FaEyeSlash } from "react-icons/fa";
import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./Auth.css";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;
const SUPERADMIN_USERNAME = import.meta.env.VITE_SUPERADMIN_USERNAME;

function Login({ setUser }) {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [showPassword, setShowPassword] = useState(false);
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();

        try {
            const roleToSend = username === SUPERADMIN_USERNAME ? "SuperAdmin" : "Admin";

            const response = await fetch(`${API_BASE_URL}/Auth/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password, role: roleToSend })
            });

            if (!response.ok) {
                throw new Error("Username yoki Parol xato");
            }

            const data = await response.json();
            const token = data.accessToken;
            localStorage.setItem("accessToken", token);

            const decoded = jwtDecode(token);
            const userRole =
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

            const userData = { userId, fullName, role: userRole };
            localStorage.setItem("user", JSON.stringify(userData));
            setUser(userData);

            toast.success("Muvaffaqiyatli hisobga kirildi", {
                position: "top-right",
                autoClose: 3000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                theme: "colored",
            });

            setTimeout(() => {
                if (userRole === "SuperAdmin") navigate("/ceo/dashboard");
                else if (userRole === "Admin") navigate("/user/dashboard");
                else navigate("/");
            }, 2000);

        } catch (error) {
            console.error("Login error:", error);
            toast.error(error.message, {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                theme: "colored",
            });
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
                        {showPassword ? <FaEyeSlash /> : <FaEye />}
                    </span>
                </div>

                <button type="submit">Kirish</button>
            </form>

            <p className="auth-footer">
                Hisobingiz yo'qmi? <a href="tel:+998991437101">Biz bilan bog'lanish</a>
            </p>

            <ToastContainer />
        </div>
    );
}

export default Login;
