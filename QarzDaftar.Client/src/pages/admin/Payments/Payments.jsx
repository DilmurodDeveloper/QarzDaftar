import React, { useState, useEffect } from "react";
import Layout from "../../../components/Layout/Layout";
import { FaPlus, FaSearch, FaEdit, FaTrash, FaCreditCard } from "react-icons/fa";
import "./Payments.css";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

const paymentMethodLabels = {
    0: "Naqd",
    1: "Karta",
    2: "Click",
    3: "Bank o'tkazmasi",
    4: "Payme",
};

function formatDateToDDMMYYYY(dateString) {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
}

function useIsMobile() {
    const [isMobile, setIsMobile] = useState(window.innerWidth <= 768);

    useEffect(() => {
        const handleResize = () => setIsMobile(window.innerWidth <= 768);
        window.addEventListener("resize", handleResize);
        return () => window.removeEventListener("resize", handleResize);
    }, []);

    return isMobile;
}

function splitByWords(text, maxLength = 30) {
    const words = text.split(" ");
    const lines = [];
    let currentLine = "";

    for (const word of words) {
        if ((currentLine + word).length <= maxLength) {
            currentLine += (currentLine ? " " : "") + word;
        } else {
            lines.push(currentLine);
            currentLine = word;
        }
    }

    if (currentLine) {
        lines.push(currentLine);
    }

    return lines;
}

function Payments() {
    const [payments, setPayments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState("");
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [currentPayment, setCurrentPayment] = useState(null);
    const [customers, setCustomers] = useState([]);
    const isMobile = useIsMobile();
    const token = localStorage.getItem("accessToken");

    const fetchCustomers = async () => {
        if (!token) {
            setError("Token topilmadi. Iltimos, qayta tizimga kiring.");
            return;
        }
        try {
            const res = await fetch(`${API_BASE_URL}/Customers/All`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error("Mijozlarni olishda xatolik");
            const data = await res.json();
            const list = Array.isArray(data) ? data : data.data || [];
            setCustomers(list);
        } catch (err) {
            setError(err.message);
        }
    };

    const fetchPayments = async () => {
        if (!token) {
            setError("Token topilmadi. Iltimos, qayta tizimga kiring.");
            return;
        }
        try {
            const res = await fetch(`${API_BASE_URL}/Payments/All`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error("To‘lovlarni olishda xatolik");
            const data = await res.json();
            const list = Array.isArray(data) ? data : data.data || [];
            setPayments(list);
        } catch (err) {
            setError(err.message);
        }
    };

    useEffect(() => {
        setLoading(true);
        setError(null);
        Promise.all([fetchCustomers(), fetchPayments()])
            .then(() => setLoading(false))
            .catch((err) => {
                setError(err.message);
                setLoading(false);
            });
    }, []);

    const filteredPayments = payments.filter((payment) => {
        const customer = customers.find((c) => c.id === payment.customerId);
        const customerName = customer?.fullName?.toLowerCase() || "";
        const desc = payment.description?.toLowerCase() || "";
        return (
            customerName.includes(searchTerm.toLowerCase()) ||
            desc.includes(searchTerm.toLowerCase())
        );
    });

    const openAddModal = () => {
        setCurrentPayment(null);
        setIsModalOpen(true);
    };

    const openEditModal = (payment) => {
        setCurrentPayment(payment);
        setIsModalOpen(true);
    };

    const closeModal = () => {
        setIsModalOpen(false);
        setCurrentPayment(null);
    };

    const deletePayment = async (id) => {
        if (!window.confirm("Ushbu to‘lovni o‘chirmoqchimisiz?")) return;

        try {
            const res = await fetch(`${API_BASE_URL}/Payments?paymentId=${id}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) {
                const errData = await res.json();
                throw new Error(errData.title || "To‘lovni o‘chirishda xatolik");
            }
            fetchPayments();
        } catch (err) {
            alert(err.message);
        }
    };

    const handleFormSubmit = async (e) => {
        e.preventDefault();

        const form = e.target;
        const payload = {
            id: currentPayment?.id || undefined,
            amount: parseFloat(form.amount.value),
            description: form.description.value,
            method: parseInt(form.method.value, 10),
            paymentDate: form.paymentDate.value,
            customerId: form.customerId.value,
            createdDate: currentPayment?.createdDate || new Date().toISOString(),
            updatedDate: new Date().toISOString(),
        };

        if (!payload.customerId || !payload.amount || !payload.paymentDate) {
            alert("Iltimos, majburiy maydonlarni to'ldiring");
            return;
        }

        try {
            let res;
            if (currentPayment) {
                res = await fetch(`${API_BASE_URL}/Payments`, {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                    body: JSON.stringify(payload),
                });
            } else {
                res = await fetch(`${API_BASE_URL}/Payments`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                    body: JSON.stringify(payload),
                });
            }

            if (!res.ok) {
                const errData = await res.json();
                throw new Error(errData.title || "Xatolik yuz berdi");
            }

            await fetchPayments();
            closeModal();
        } catch (err) {
            alert(err.message);
        }
    };

    if (loading) return <Layout><p>Yuklanmoqda...</p></Layout>;
    if (error) return <Layout><p style={{ color: "red" }}>{error}</p></Layout>;

    return (
        <Layout>
            <nav className="breadcrumb">
                <FaCreditCard className="breadcrumb-icon" />
                <span className="breadcrumb-arrow"> &gt; </span>
                <span>Tolovlar</span>
            </nav>
            <div className="payments-container">
                <div className="payments-header">
                    <h2>To'lovlar ro'yxati</h2>
                    <div className="payments-actions">
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
                        <button className="add-btn" onClick={openAddModal}>
                            <FaPlus /> To'lov qo'shish
                        </button>
                    </div>
                </div>

                <table className="payments-table">
                    <thead>
                        <tr>
                            <th>№</th>
                            <th>Mijoz</th>
                            <th>Summa</th>
                            <th>Tavsif</th>
                            <th>To'lov turi</th>
                            <th>To'lov sanasi</th>
                            <th>Amallar</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filteredPayments.length > 0 ? (
                            filteredPayments.map((payment, index) => {
                                const customer = customers.find((c) => c.id === payment.customerId);
                                return (
                                    <tr key={payment.id || index}>
                                        <td data-label="№">{index + 1}</td>
                                        <td data-label="Mijoz">{customer?.fullName || "Noma'lum"}</td>
                                        <td data-label="Summa">{payment.amount.toLocaleString()} so'm</td>
                                        <td data-label="Tavsif" title={!isMobile ? payment.description : undefined}>
                                            {isMobile ? (
                                                splitByWords(payment.description || "", 30).map((line, i) => (
                                                    <p key={i} style={{ margin: i === 0 ? "0" : "4px 0" }}>{line}</p>
                                                ))
                                            ) : (
                                                payment.description.length > 30
                                                    ? payment.description.slice(0, 30) + "..."
                                                    : payment.description || "—"
                                            )}
                                        </td>
                                        <td data-label="To'lov turi">{paymentMethodLabels[payment.method]}</td>
                                        <td data-label="To'lov sanasi">{formatDateToDDMMYYYY(payment.paymentDate)}</td>
                                        <td data-label="Amallar">
                                            <button className="edit-btn" onClick={() => openEditModal(payment)}>
                                                <FaEdit />
                                            </button>
                                            <button className="delete-btn" onClick={() => deletePayment(payment.id)}>
                                                <FaTrash />
                                            </button>
                                        </td>
                                    </tr>
                                );
                            })
                        ) : (
                            <tr>
                                <td colSpan="6" style={{ textAlign: "center" }}>
                                    To'lovlar topilmadi
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>

                {isModalOpen && (
                    <div className="modal-overlay">
                        <div className="modal-content">
                            <h3>{currentPayment ? "To'lovni tahrirlash" : "Yangi to'lov qo'shish"}</h3>
                            <form onSubmit={handleFormSubmit}>
                                <label>
                                    Mijoz:
                                    <select
                                        name="customerId"
                                        defaultValue={currentPayment?.customerId || ""}
                                        required
                                    >
                                        <option value="" disabled>
                                            Mijozni tanlang
                                        </option>
                                        {customers.map((c) => (
                                            <option key={c.id} value={c.id}>
                                                {c.fullName}
                                            </option>
                                        ))}
                                    </select>
                                </label>

                                <label>
                                    Summa:
                                    <input
                                        type="number"
                                        name="amount"
                                        defaultValue={currentPayment?.amount || ""}
                                        required
                                        min={0.01}
                                        step="0.01"
                                    />
                                </label>

                                <label>
                                    Tavsif:
                                    <input
                                        type="text"
                                        name="description"
                                        defaultValue={currentPayment?.description || ""}
                                    />
                                </label>

                                <label>
                                    To'lov turi:
                                    <select
                                        name="method"
                                        defaultValue={currentPayment?.method ?? 0}
                                        required
                                    >
                                        {Object.entries(paymentMethodLabels).map(([key, label]) => (
                                            <option key={key} value={key}>
                                                {label}
                                            </option>
                                        ))}
                                    </select>
                                </label>

                                <label>
                                    To'lov sanasi:
                                    <input
                                        type="date"
                                        name="paymentDate"
                                        defaultValue={
                                            currentPayment
                                                ? new Date(currentPayment.paymentDate).toISOString().split("T")[0]
                                                : new Date().toISOString().split("T")[0]
                                        }
                                        required
                                    />
                                </label>

                                <div className="modal-actions">
                                    <button type="submit" className="save-btn">
                                        Saqlash
                                    </button>
                                    <button type="button" className="cancel-btn" onClick={closeModal}>
                                        Bekor qilish
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                )}
            </div>
        </Layout>
    );
}

export default Payments;