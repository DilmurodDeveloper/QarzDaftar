import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Layout from "../../../components/Layout/Layout";
import {
    FaUser,
    FaAt,
    FaPhone,
    FaStore,
    FaMapMarkerAlt,
    FaCalendarAlt,
    FaInfoCircle
} from "react-icons/fa";
import "./Profile.css";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function formatDateToDDMMYYYY(dateString) {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
}

function Profile() {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        const userData = JSON.parse(localStorage.getItem("user") || "{}");
        const userId = userData.userId;

        if (!userId) {
            alert("Foydalanuvchi aniqlanmadi. Iltimos, tizimga qayta kiring.");
            navigate("/login");
            return;
        }

        const fetchUserProfile = async () => {
            try {
                const token = localStorage.getItem("accessToken");
                if (!token) {
                    alert("Token topilmadi. Iltimos, tizimga qayta kiring.");
                    navigate("/login");
                    return;
                }

                const res = await fetch(`${API_BASE_URL}/Users/${userId}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });

                if (!res.ok) {
                    throw new Error("Profilni olishda xatolik yuz berdi");
                }

                const data = await res.json();
                setUser(data);
                setError(null);
            } catch (err) {
                setError(err.message);
                setUser(null);
            } finally {
                setLoading(false);
            }
        };

        fetchUserProfile();
    }, [navigate]);

    if (loading)
        return (
            <Layout>
                <p>Yuklanmoqda...</p>
            </Layout>
        );

    if (error)
        return (
            <Layout>
                <p style={{ color: "red" }}>{error}</p>
            </Layout>
        );

    return (
        <Layout>
            <h2 className="p-header">Shaxsiy ma'lumotlar:</h2>
            {user && (
                <div className="p-info">
                    <p><FaUser className="p-icon" /> <strong>Ism-Familiya:</strong> {user.fullName}</p>
                    <p><FaAt className="p-icon" /> <strong>Username:</strong> {user.username}</p>
                    <p><FaAt className="p-icon" /> <strong>Email:</strong> {user.email}</p>
                    <p><FaPhone className="p-icon" /> <strong>Telefon raqam:</strong> {user.phoneNumber}</p>
                    <p><FaStore className="p-icon" /> <strong>Do'kon nomi:</strong> {user.shopName}</p>
                    <p><FaMapMarkerAlt className="p-icon" /> <strong>Manzil:</strong> {user.address}</p>
                    <p><FaCalendarAlt className="p-icon" /> <strong>Ro'yxatdan o'tgan sana:</strong> {formatDateToDDMMYYYY(user.registeredAt)}</p>

                    <div className="profile-support">
                        <FaInfoCircle className="support-icon" />
                        <span>
                            Profil ma'lumotlarini o'zgartirish uchun{" "}
                            <a
                                href="https://t.me/DilmurodDeveloper"
                                target="_blank"
                                rel="noopener noreferrer"
                                style={{ color: "blue", textDecoration: "none" }}
                            >
                                Support
                            </a>{" "}
                            ga murojaat qiling.
                        </span>
                    </div>
                </div>
            )}
        </Layout>
    );
}

export default Profile;