import React, { useState, useEffect } from "react";
import {
    FaUsers,
    FaMoneyBillWave,
    FaStickyNote,
    FaCoins,
    FaCheckCircle,
    FaTimesCircle,
    FaHome
} from "react-icons/fa";
import Layout from "../../../components/Layout/Layout";
import "./Dashboard.css";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function Dashboard() {
    const [customersCount, setCustomersCount] = useState(0);
    const [debtsCount, setDebtsCount] = useState(0);
    const [totalDebtAmount, setTotalDebtAmount] = useState(0);
    const [totalPaidDebt, setTotalPaidDebt] = useState(0);
    const [remainingDebt, setRemainingDebt] = useState(0);
    const [notesCount, setNotesCount] = useState(0);

    useEffect(() => {
        const token = localStorage.getItem("accessToken");
        const storedUser = localStorage.getItem("user");

        if (!token || !storedUser) {
            window.location.href = "/login";
            return;
        }

        const parsedUser = JSON.parse(storedUser);

        fetchCustomers(token);
        fetchDebts(token);
        fetchTotalPaidDebt(token, parsedUser.userId);
        fetchRemainingDebt(token, parsedUser.userId);
        fetchUserNotes(token);
    }, []);

    const fetchCustomers = async (token) => {
        try {
            const res = await fetch(`${API_BASE_URL}/Customers/All`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            const data = await res.json();
            if (Array.isArray(data)) setCustomersCount(data.length);
            else if (Array.isArray(data.data)) setCustomersCount(data.data.length);
        } catch (error) {
            console.error("Mijozlarni olishda xatolik:", error);
        }
    };

    const fetchDebts = async (token) => {
        try {
            const res = await fetch(`${API_BASE_URL}/Debts/All`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            const data = await res.json();
            const list = Array.isArray(data) ? data : data.data || [];

            setDebtsCount(list.length);

            const totalSum = list.reduce((sum, debt) => sum + (debt.amount || 0), 0);
            setTotalDebtAmount(totalSum);
        } catch (error) {
            console.error("Qarzdorlarni olishda xatolik:", error);
        }
    };

    const fetchTotalPaidDebt = async (token, userId) => {
        try {
            const res = await fetch(`${API_BASE_URL}/Payments/total-paid/user/${userId}`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            const data = await res.json();
            const paidAmount = typeof data === "number"
                ? data
                : data.totalPaid ?? data.amount ?? 0;
            setTotalPaidDebt(paidAmount);
        } catch (error) {
            console.error("To‘langan qarzni olishda xatolik:", error);
        }
    };

    const fetchRemainingDebt = async (token, userId) => {
        try {
            const res = await fetch(`${API_BASE_URL}/Payments/remaining-debt/user/${userId}`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            const data = await res.json();
            const debtAmount = typeof data === "number"
                ? data
                : data.remainingDebt ?? data.amount ?? 0;
            setRemainingDebt(debtAmount);
        } catch (error) {
            console.error("Qolgan qarzni olishda xatolik:", error);
        }
    };

    const fetchUserNotes = async (token) => {
        try {
            const res = await fetch(`${API_BASE_URL}/UserNotes/All`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            const data = await res.json();
            const list = Array.isArray(data) ? data : data.data || [];
            setNotesCount(list.length);
        } catch (error) {
            console.error("Eslatmalarni olishda xatolik:", error);
        }
    };

    return (
        <Layout>
            <nav className="breadcrumb">
                <FaHome className="breadcrumb-icon" />
                <span className="breadcrumb-arrow"> &gt; </span>
                <span>Asosiy</span>
            </nav>
            <WelcomeCard />
            <div className="stats-grid">
                <StatCard title="Barcha mijozlar" value={customersCount} icon={FaUsers} color="#3B82F6" />
                <StatCard title="Barcha qarzlar soni" value={debtsCount} icon={FaCoins} color="#2563EB" />
                <StatCard title="Umumiy qarz summasi" value={`${totalDebtAmount.toLocaleString()} so'm`} icon={FaMoneyBillWave} color="#F59E0B" />
                <StatCard title="To‘langan qarzlar" value={`${totalPaidDebt.toLocaleString()} so'm`} icon={FaCheckCircle} color="#10B981" />
                <StatCard title="To‘lanmagan qarzlar" value={`${remainingDebt.toLocaleString()} so'm`} icon={FaTimesCircle} color="#EF4444" />
                <StatCard title="Barcha eslatmalar soni" value={notesCount} icon={FaStickyNote} color="#8B5CF6" />
            </div>
        </Layout>
    );
}

function WelcomeCard() {
    const storedUser = localStorage.getItem("user");

    const fullName = storedUser ? JSON.parse(storedUser).fullName : "Foydalanuvchi";
    return (
        <div className="welcome-card">
            <div>
                <h2>Assalomu alaykum, {fullName} 👋</h2>
                <p>Boshqaruv paneliga xush kelibsiz</p>
            </div>
        </div>
    );
}

function StatCard({ title, value, icon: Icon, color }) {
    return (
        <div className="stat-card">
            <div className="stat-content">
                <div className="stat-icon" style={{ color }}>
                    <Icon />
                </div>
                <div className="stat-info">
                    <h4>{title}</h4>
                    <p>{value}</p>
                </div>
            </div>
        </div>
    );
}

export default Dashboard;