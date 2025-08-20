import React, { useState, useEffect } from "react";
import { FaEye, FaEyeSlash } from "react-icons/fa";
import "./UserForm.css";

function UserForm({ onSuccess, token, editingUser }) {
    const [user, setUser] = useState({
        fullName: "",
        username: "",
        email: "",
        password: "",
        phoneNumber: "",
        shopName: "",
        address: "",
        role: "Admin",
    });

    const [showPassword, setShowPassword] = useState(false);

    useEffect(() => {
        if (editingUser) {
            setUser({
                ...editingUser,
                password: "",
            });
        }
    }, [editingUser]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setUser((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        const requiredFields = ["fullName", "username", "email", "phoneNumber", "shopName", "address"];
        for (const key of requiredFields) {
            if (!user[key]) {
                alert(`${key} maydoni to‘ldirilishi shart!`);
                return;
            }
        }

        if (!editingUser && !user.password) {
            alert("Password maydoni to‘ldirilishi shart!");
            return;
        }

        try {
            const endpoint = editingUser
                ? `${import.meta.env.VITE_API_BASE_URL}/Auth/update`
                : `${import.meta.env.VITE_API_BASE_URL}/Auth/register`;

            const bodyData = {
                fullName: user.fullName,
                username: user.username,
                email: user.email,
                phoneNumber: user.phoneNumber,
                shopName: user.shopName,
                address: user.address,
                role: user.role,
                ...(editingUser ? { id: user.id } : {}),
                ...(user.password ? { password: user.password } : {}),
            };

            const res = await fetch(endpoint, {
                method: editingUser ? "PUT" : "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify(bodyData),
            });

            if (!res.ok) {
                const errorData = await res.json();
                alert(errorData.message || "Xatolik yuz berdi");
                return;
            }

            onSuccess();
        } catch (err) {
            alert(err.message);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="user-form">
            <div className="form-row">
                <label>
                    Full Name*:
                    <input type="text" name="fullName" value={user.fullName} onChange={handleChange} required />
                </label>
                <label>
                    Username*:
                    <input type="text" name="username" value={user.username} onChange={handleChange} required />
                </label>
            </div>

            <label>
                Email*:
                <input type="email" name="email" value={user.email} onChange={handleChange} required />
            </label>

            <label>
                Phone Number*:
                <input type="text" name="phoneNumber" value={user.phoneNumber} onChange={handleChange} required />
            </label>

            <label className="password-wrapper">
                Password{editingUser ? " (yangilash ixtiyoriy)" : "*"}:
                <div className="password-input-wrapper">
                    <input
                        type={showPassword ? "text" : "password"}
                        name="password"
                        value={user.password}
                        onChange={handleChange}
                        placeholder={editingUser ? "Yangi parol" : ""}
                    />
                    <span className="toggle-password-superadmin" onClick={() => setShowPassword(!showPassword)}>
                        {showPassword ? <FaEye /> : <FaEyeSlash />}
                    </span>
                </div>
            </label>

            <div className="form-row">
                <label>
                    Shop Name*:
                    <input type="text" name="shopName" value={user.shopName} onChange={handleChange} required />
                </label>
                <label>
                    Address*:
                    <input type="text" name="address" value={user.address} onChange={handleChange} required />
                </label>
            </div>

            <button type="submit">{editingUser ? "Yangilash" : "Saqlash"}</button>
        </form>
    );
}

export default UserForm;
