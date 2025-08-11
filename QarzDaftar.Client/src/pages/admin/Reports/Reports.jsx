import React, { useState, useEffect } from "react";
import { FaChartBar } from "react-icons/fa";
import Layout from "../../../components/Layout/Layout";
import "./Reports.css";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function Reports() {
    const [totals, setTotals] = useState({
        totalDebt: 0,
        totalPaid: 0,
        remainingDebt: 0,
    });
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const userData = JSON.parse(localStorage.getItem("user") || "{}");
    const userId = userData.userId;
    const token = localStorage.getItem("accessToken");

    useEffect(() => {
        if (!token || !userId) {
            setError("Foydalanuvchi aniqlanmadi yoki token yo'q.");
            setLoading(false);
            return;
        }

        const fetchTotals = async () => {
            try {
                const [debtRes, paidRes, remainingRes] = await Promise.all([
                    fetch(`${API_BASE_URL}/Debts/total-debt/user/${userId}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                    fetch(`${API_BASE_URL}/Payments/total-paid/user/${userId}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                    fetch(`${API_BASE_URL}/Payments/remaining-debt/user/${userId}`, {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                ]);

                if (!debtRes.ok || !paidRes.ok || !remainingRes.ok) {
                    throw new Error("Ma'lumotlarni olishda xatolik yuz berdi");
                }

                const debtData = await debtRes.json();
                const paidData = await paidRes.json();
                const remainingData = await remainingRes.json();

                setTotals({
                    totalDebt: debtData || 0,
                    totalPaid: paidData || 0,
                    remainingDebt: remainingData || 0,
                });
                setError(null);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchTotals();
    }, [token, userId]);

    if (loading) return <Layout><p>Yuklanmoqda...</p></Layout>;
    if (error) return <Layout><p style={{ color: "red" }}>{error}</p></Layout>;

    return (
        <Layout>
            <nav className="breadcrumb">
                <FaChartBar className="breadcrumb-icon" />
                <span className="breadcrumb-arrow"> &gt; </span>
                <span>Hisobotlar</span>
            </nav>
            <div className="reports-container">
                <h2>Hisobotlar</h2>
                <ul>
                    <li>
                        <strong>Umumiy qarz:</strong> {totals.totalDebt.toLocaleString()} so'm
                    </li>
                    <li>
                        <strong>To'langan summa:</strong> {totals.totalPaid.toLocaleString()} so'm
                    </li>
                    <li>
                        <strong>Qolgan qarz:</strong> {totals.remainingDebt.toLocaleString()} so'm
                    </li>
                </ul>
            </div>
        </Layout>
    );
}

export default Reports;