import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { FaEye, FaEyeSlash } from "react-icons/fa";
import "./Auth.css";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function Register({ setUser }) {
    const [form, setForm] = useState({
        fullName: "",
        username: "",
        email: "",
        password: "",
        phoneNumber: "",
        shopname: "",
        address: ""
    });

    const [showPassword, setShowPassword] = useState(false);
    const [agreeTerms, setAgreeTerms] = useState(false);
    const navigate = useNavigate();

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleRegister = async (e) => {
        e.preventDefault();

        try {
            const response = await fetch(`${API_BASE_URL}/auth/register`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(form)
            });

            if (!response.ok) throw new Error("Ro'yxatdan o'tish xatoligi");

            const data = await response.json();
            alert(data.message);
            setUser({ username: form.username });
            navigate("/login");
        } catch (error) {
            alert("Xatolik: " + error.message);
        }
    };

    return (
        <div className="auth-container">
            <h2>Ro'yxatdan o'tish</h2>
            <form onSubmit={handleRegister} className="auth-form">
                <div className="input-row">
                    <input
                        name="fullName"
                        placeholder="Ism"
                        value={form.fullName}
                        onChange={handleChange}
                        required
                    />
                    <input
                        name="username"
                        placeholder="Username"
                        value={form.username}
                        onChange={handleChange}
                        required
                    />
                </div>
                <input
                    name="email"
                    type="email"
                    placeholder="Email"
                    value={form.email}
                    onChange={handleChange}
                    required
                />

                <div className="password-field">
                    <input
                        name="password"
                        type={showPassword ? "text" : "password"}
                        placeholder="Parol"
                        value={form.password}
                        onChange={handleChange}
                        required
                    />
                    <span
                        className="toggle-password"
                        onClick={() => setShowPassword(!showPassword)}
                    >
                        {showPassword ? <FaEye /> : <FaEyeSlash />}
                    </span>
                </div>

                <input
                    name="phoneNumber"
                    placeholder="Telefon raqam"
                    value={form.phoneNumber}
                    onChange={handleChange}
                    required
                />
                <div className="input-row">
                    <input
                        name="shopname"
                        placeholder="Do'kon nomi"
                        value={form.shopname}
                        onChange={handleChange}
                        required
                    />
                    <input
                        name="address"
                        placeholder="Manzil"
                        value={form.address}
                        onChange={handleChange}
                        required
                    />
                </div>

                <button type="submit" disabled={!agreeTerms}>
                    Ro'yxatdan o'tish
                </button>
            </form>
        </div>
    );
}

export default Register;
