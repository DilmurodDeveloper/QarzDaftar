import React, { useState, useEffect } from "react";
import Layout from "../../../components/Layout/Layout";
import { FaSearch, FaMoneyBillWave } from "react-icons/fa";
import "./DebtorsSummary.css";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function DebtorsSummary() {
    const [customers, setCustomers] = useState([]);
    const [summaryData, setSummaryData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState("");

    const token = localStorage.getItem("accessToken");

    const fetchCustomers = async () => {
        if (!token) {
            setError("Token topilmadi. Iltimos, qayta tizimga kiring.");
            return [];
        }
        try {
            const res = await fetch(`${API_BASE_URL}/Customers/All`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error("Mijozlarni olishda xatolik");
            const data = await res.json();
            return Array.isArray(data) ? data : data.data || [];
        } catch (err) {
            setError(err.message);
            return [];
        }
    };

    const fetchSummaryForCustomer = async (customerId) => {
        if (!token) return null;
        try {
            const [debtRes, paidRes, remainingRes] = await Promise.all([
                fetch(`${API_BASE_URL}/Debts/total/${customerId}`, {
                    headers: { Authorization: `Bearer ${token}` },
                }),
                fetch(`${API_BASE_URL}/Payments/customers/${customerId}/total-paid`, {
                    headers: { Authorization: `Bearer ${token}` },
                }),
                fetch(`${API_BASE_URL}/Payments/customers/${customerId}/remaining-debt`, {
                    headers: { Authorization: `Bearer ${token}` },
                }),
            ]);

            if (!debtRes.ok || !paidRes.ok || !remainingRes.ok) {
                throw new Error("Ma'lumotlarni olishda xatolik");
            }

            const totalDebt = await debtRes.json();
            const totalPaid = await paidRes.json();
            const remainingDebt = await remainingRes.json();

            let status = "To'lanmagan";
            if (totalDebt === totalPaid) status = "To'langan";
            else if (totalPaid > totalDebt) status = "Ortiqcha to'lov";

            return { totalDebt, totalPaid, remainingDebt, status };
        } catch (err) {
            setError(err.message);
            return null;
        }
    };

    useEffect(() => {
        const loadData = async () => {
            setLoading(true);
            setError(null);

            const customerList = await fetchCustomers();

            const promises = customerList.map(async (customer) => {
                const summary = await fetchSummaryForCustomer(customer.id);
                return {
                    customer,
                    ...summary,
                };
            });

            const results = await Promise.all(promises);

            const filteredResults = results.filter((r) => r !== null);

            setCustomers(customerList);
            setSummaryData(filteredResults);
            setLoading(false);
        };

        loadData();
    }, []);

    const filteredSummary = summaryData.filter(({ customer }) =>
        customer.fullName.toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (loading) return <Layout><p>Yuklanmoqda...</p></Layout>;
    if (error) return <Layout><p style={{ color: "red" }}>{error}</p></Layout>;

    return (
        <Layout>
            <nav className="breadcrumb">
                <FaMoneyBillWave className="breadcrumb-icon" />
                <span>Qarzlar</span>
                <span className="breadcrumb-arrow"> &gt; </span>
                <span>Qarzdorlar</span>
            </nav>
            <div className="debtors-summary-container">
                <h2>Mijozlarning qarzlar bo'yicha umumiy ko'rinishi</h2>
                <div className="search-box">
                    <FaSearch className="search-icon" />
                    <input
                        type="text"
                        placeholder="Qidirish..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="search-input"
                    />
                </div>
                <br></br>
                <table className="summary-table">
                    <thead>
                        <tr>
                            <th>№</th>
                            <th>Mijoz</th>
                            <th>Jami qarzlar</th>
                            <th>To'langan summa</th>
                            <th>Qolgan qarz</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filteredSummary.length ? (
                            filteredSummary.map(({ customer, totalDebt, totalPaid, remainingDebt, status }, index) => (
                                <tr key={customer.id || index}>
                                    <td data-label="№">{index + 1}</td>
                                    <td data-label="Mijoz">{customer.fullName}</td>
                                    <td data-label="Jami qarzlar">{totalDebt.toLocaleString()} so'm</td>
                                    <td data-label="To'langan summa">{totalPaid.toLocaleString()} so'm</td>
                                    <td data-label="Qolgan qarz">{remainingDebt.toLocaleString()} so'm</td>
                                    <td data-label="Status">{status}</td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="5" style={{ textAlign: "center" }}>Mijozlar topilmadi</td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </Layout>
    );
}

export default DebtorsSummary;