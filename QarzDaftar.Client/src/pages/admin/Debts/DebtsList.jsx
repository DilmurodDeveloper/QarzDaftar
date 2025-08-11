import React, { useState, useEffect } from "react";
import Layout from "../../../components/Layout/Layout";
import { FaPlus, FaEdit, FaTrash, FaSearch, FaMoneyBillWave } from "react-icons/fa";
import DebtsFormModal from "./DebtsFormModal";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./DebtsList.css";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export function generateNewGuid() {
    return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, (c) => {
        const r = (Math.random() * 16) | 0;
        const v = c === "x" ? r : (r & 0x3) | 0x8;
        return v.toString(16);
    });
}

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

function DebtsList() {
    const [debts, setDebts] = useState([]);
    const [customers, setCustomers] = useState([]);
    const [filteredDebts, setFilteredDebts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState("");
    const [modalOpen, setModalOpen] = useState(false);
    const [editingDebt, setEditingDebt] = useState(null);
    const isMobile = useIsMobile();

    const handleSaveDebt = async (formData) => {
        const token = localStorage.getItem("accessToken");
        if (!token) {
            toast.error("Token topilmadi. Iltimos, qayta tizimga kiring.");
            return;
        }

        const now = new Date().toISOString();
        const payload = {
            id: editingDebt?.id || generateNewGuid(),
            amount: Number(formData.amount),
            remainingAmount: Number(formData.remainingAmount ?? formData.amount),
            description: formData.description || "",
            status: Number(formData.status ?? 0),
            dueDate: formData.dueDate || new Date().toISOString(),
            createdDate: editingDebt?.createdDate || new Date().toISOString(),
            updatedDate: new Date().toISOString(),
            customerId: formData.customerId
        };

        const method = editingDebt ? "PUT" : "POST";
        const url = `${API_BASE_URL}/Debts`;

        try {
            const res = await fetch(url, {
                method,
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify(payload),
            });

            if (!res.ok) {
                const errData = await res.json().catch(() => ({}));
                throw new Error(errData.title || "Saqlashda xatolik yuz berdi");
            }

            setModalOpen(false);
            toast.success("Muvaffaqiyatli saqlandi!");
            fetchDebts();
        } catch (err) {
            toast.error(err.message);
        }
    };

    const fetchCustomers = async () => {
        const token = localStorage.getItem("accessToken");
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

    const fetchDebts = async () => {
        const token = localStorage.getItem("accessToken");
        if (!token) {
            setError("Token topilmadi. Iltimos, qayta tizimga kiring.");
            return;
        }
        try {
            const res = await fetch(`${API_BASE_URL}/Debts/All`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error("Qarzlarni olishda xatolik");
            const data = await res.json();
            const list = Array.isArray(data) ? data : data.data || [];
            setDebts(list);
        } catch (err) {
            setError(err.message);
        }
    };

    useEffect(() => {
        setLoading(true);
        setError(null);
        Promise.all([fetchCustomers(), fetchDebts()])
            .then(() => setLoading(false))
            .catch((err) => {
                setError(err.message);
                setLoading(false);
            });
    }, []);

    useEffect(() => {
        const lowerSearch = searchTerm.toLowerCase();
        const filtered = debts.filter((debt) => {
            const customer = customers.find((c) => c.id === debt.customerId);
            const customerName = customer?.fullName?.toLowerCase() || "";
            const description = debt.description?.toLowerCase() || "";
            return description.includes(lowerSearch) || customerName.includes(lowerSearch);
        });
        setFilteredDebts(filtered);
    }, [searchTerm, debts, customers]);

    const handleSearch = (e) => {
        setSearchTerm(e.target.value);
    };

    const handleAddDebt = () => {
        setEditingDebt(null);
        setModalOpen(true);
    };

    const handleEditDebt = (debt) => {
        setEditingDebt(debt);
        setModalOpen(true);
    };

    const handleDeleteDebt = async (id) => {
        if (!window.confirm("Ushbu qarzni o'chirmoqchimisiz?")) return;
        const token = localStorage.getItem("accessToken");
        try {
            const res = await fetch(`${API_BASE_URL}/Debts?debtId=${id}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) {
                const errorData = await res.json();
                throw new Error(errorData.title || "Qarz o'chirishda xatolik");
            }
            toast.success("Qarz muvaffaqiyatli o'chirildi!");
            fetchDebts();
        } catch (err) {
            toast.error(err.message);
        }
    };

    if (loading) return <Layout><p>Yuklanmoqda...</p></Layout>;
    if (error) return <Layout><p style={{ color: "red" }}>{error}</p></Layout>;

    return (
        <Layout>
            <nav className="breadcrumb">
                <FaMoneyBillWave className="breadcrumb-icon" />
                <span>Qarzlar</span>
                <span className="breadcrumb-arrow"> &gt; </span>
                <span>Barcha qarzlar</span>
            </nav>
            <div className="debts-header">
                <h2>Qarzlar ro'yxati</h2>
                <div className="debts-actions">
                    <div className="search-box">
                        <FaSearch className="search-icon" />
                        <input
                            type="text"
                            placeholder="Qidirish..."
                            value={searchTerm}
                            onChange={handleSearch}
                            className="search-input"
                        />
                    </div>
                    <button className="add-btn" onClick={handleAddDebt}>
                        <FaPlus /> Qarz qo'shish
                    </button>
                </div>
            </div>

            {filteredDebts.length === 0 ? (
                <p>Qarzlar topilmadi</p>
            ) : (
                <table className="debts-table">
                    <thead>
                        <tr>
                            <th>№</th>
                            <th>Mijoz</th>
                            <th>Qarz summasi</th>
                            <th>Tavsif</th>
                            <th>To'lov muddati</th>
                            <th>Yaratilgan vaqt</th>
                            <th>Amallar</th>
                        </tr>
                    </thead>
                        <tbody>
                            {filteredDebts.map((debt, index) => {
                                const customer = customers.find((c) => c.id === debt.customerId);
                                return (
                                    <tr key={debt.id || index}>
                                        <td data-label="№">{index + 1}</td>
                                        <td data-label="Mijoz">{customer?.fullName || "Noma'lum"}</td>
                                        <td data-label="Qarz summasi">{debt.amount} so'm</td>
                                        <td data-label="Tavsif" title={!isMobile ? debt.description : undefined}>
                                            {isMobile ? (
                                                splitByWords(debt.description || "", 30).map((line, i) => (
                                                    <p key={i} style={{ margin: i === 0 ? "0" : "4px 0" }}>{line}</p>
                                                ))
                                            ) : (
                                                debt.description.length > 30
                                                    ? debt.description.slice(0, 30) + "..."
                                                    : debt.description || "—"
                                            )}
                                        </td>
                                        <td data-label="To'lov muddati">{formatDateToDDMMYYYY(debt.dueDate)}</td>
                                        <td data-label="Yaratilgan vaqt">{formatDateToDDMMYYYY(debt.createdDate)}</td>
                                        <td data-label="Amallar" className="action-buttons">
                                            <button
                                                className="edit-btn"
                                                onClick={() => handleEditDebt(debt)}
                                                aria-label="Tahrirlash"
                                            >
                                                <FaEdit />
                                            </button>
                                            <button
                                                className="delete-btn"
                                                onClick={() => handleDeleteDebt(debt.id)}
                                                aria-label="O'chirish"
                                            >
                                                <FaTrash />
                                            </button>
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                </table>
            )}

            <DebtsFormModal
                isOpen={modalOpen}
                onClose={() => setModalOpen(false)}
                onSave={handleSaveDebt}
                debt={editingDebt}
                customers={customers}
            />
        </Layout>
    );
}

export default DebtsList;