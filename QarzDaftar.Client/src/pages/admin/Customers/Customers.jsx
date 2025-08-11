import React, { useState, useEffect } from "react";
import Layout from "../../../components/Layout/Layout";
import { FaPlus, FaEdit, FaTrash, FaSearch, FaUsers } from "react-icons/fa";
import CustomerFormModal from "./CustomerFormModal";
import { toast, ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import "./Customers.css";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function generateNewGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
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

function getCurrentUserIdFromStorage() {
    const storedUser = localStorage.getItem("user");
    if (!storedUser) return null;
    try {
        return JSON.parse(storedUser).userId;
    } catch {
        return null;
    }
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

function Customers({ filter, breadcrumbTitle }) {
    const [customers, setCustomers] = useState([]);
    const [filteredCustomers, setFilteredCustomers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState("");
    const [modalOpen, setModalOpen] = useState(false);
    const [editingCustomer, setEditingCustomer] = useState(null);
    const [saveSuccess, setSaveSuccess] = useState(false);
    const isMobile = useIsMobile();

    useEffect(() => {
        fetchCustomers();
    }, []);

    useEffect(() => {
        let filtered = customers;

        if (filter === "active") {
            filtered = filtered.filter((c) => c.isActive);
        } else if (filter === "inactive") {
            filtered = filtered.filter((c) => !c.isActive);
        }

        const lowerSearch = searchTerm.toLowerCase();
        filtered = filtered.filter(
            (c) =>
                c.fullName?.toLowerCase().includes(lowerSearch) ||
                c.phoneNumber?.toLowerCase().includes(lowerSearch) ||
                c.address?.toLowerCase().includes(lowerSearch)
        );

        setFilteredCustomers(filtered);
    }, [searchTerm, customers, filter]);

    useEffect(() => {
        if (saveSuccess && !modalOpen) {
            toast.success("Mijoz muvaffaqiyatli saqlandi!");
            setSaveSuccess(false);
        }
    }, [saveSuccess, modalOpen]);

    const fetchCustomers = async () => {
        setLoading(true);
        setError(null);
        const token = localStorage.getItem("accessToken");

        if (!token) {
            setError("Token topilmadi. Iltimos, qayta tizimga kiring.");
            toast.error("Token topilmadi. Iltimos, qayta tizimga kiring.");
            setLoading(false);
            return;
        }

        try {
            const res = await fetch(`${API_BASE_URL}/Customers/All`, {
                headers: { Authorization: `Bearer ${token}` },
            });

            if (res.status === 401) {
                throw new Error("Avtorizatsiya xatosi (401). Qayta tizimga kiring.");
            }
            if (!res.ok) throw new Error("Mijozlarni olishda xatolik yuz berdi");

            const data = await res.json();
            const list = Array.isArray(data) ? data : data.data || [];

            setCustomers(list);
            setFilteredCustomers(list);
        } catch (err) {
            setError(err.message);
            toast.error(err.message);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = (e) => {
        setSearchTerm(e.target.value);
    };

    const handleAddCustomer = () => {
        setEditingCustomer(null);
        setModalOpen(true);
    };

    const handleEdit = (customer) => {
        setEditingCustomer(customer);
        setModalOpen(true);
    };

    const handleDelete = async (id) => {
        if (!window.confirm("Rostdan ham ushbu mijozni o‘chirmoqchimisiz?")) return;

        const token = localStorage.getItem("accessToken");

        try {
            const res = await fetch(`${API_BASE_URL}/Customers?customerId=${id}`, {
                method: "DELETE",
                headers: {
                    Authorization: `Bearer ${token}`,
                },
            });

            if (!res.ok) {
                const errorData = await res.json();

                if (
                    errorData.title &&
                    errorData.title.includes("Failed customer service error")
                ) {
                    toast.error(
                        "Bu mijozga tegishli qarz va to‘lov yozuvlarini o‘chiring va keyin takror urinib ko'ring."
                    );
                } else {
                    toast.error(errorData.title || "Mijozni o‘chirishda xatolik yuz berdi");
                }

                return;
            }

            toast.success("Mijoz muvaffaqiyatli o‘chirildi!");
            fetchCustomers();
        } catch (err) {
            toast.error("Tarmoqqa ulanishda xatolik yuz berdi: " + err.message);
        }
    };

    const handleSave = async (formData) => {
        const token = localStorage.getItem("accessToken");

        const method = editingCustomer ? "PUT" : "POST";
        const url = `${API_BASE_URL}/Customers`;

        const now = new Date().toISOString();

        const payload = {
            ...formData,
            id: editingCustomer?.id || generateNewGuid(),
            createdDate: editingCustomer?.createdDate || now,
            updatedDate: now,
            userId: getCurrentUserIdFromStorage(),
            isActive: formData.isActive !== undefined ? formData.isActive : true,
        };

        try {
            const res = await fetch(url, {
                method,
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify(payload),
            });

            if (!res.ok) throw new Error("Saqlashda xatolik yuz berdi");

            setModalOpen(false);
            setSaveSuccess(true);
            fetchCustomers();
        } catch (err) {
            toast.error(err.message);
        }
    };

    if (loading) return <Layout><p>Yuklanmoqda...</p></Layout>;
    if (error) return <Layout><p style={{ color: "red" }}>{error}</p></Layout>;

    return (
        <Layout>
            <ToastContainer
                position="top-right"
                autoClose={4000}
                hideProgressBar={false}
                newestOnTop={false}
                closeOnClick
                rtl={false}
                pauseOnFocusLoss
                draggable
                pauseOnHover
            />

            <nav className="breadcrumb">
                <FaUsers className="breadcrumb-icon" />
                <span>Mijozlar</span>
                <span className="breadcrumb-arrow"> &gt; </span>
                <span>{breadcrumbTitle}</span>
            </nav>

            <div className="customers-header">
                <h2>Mijozlar ro'yxati</h2>
                <div className="customer-actions">
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
                    <button className="add-btn" onClick={handleAddCustomer}>
                        <FaPlus /> Mijoz qo'shish
                    </button>
                </div>
            </div>

            {filteredCustomers.length === 0 ? (
                <p>Mijozlar topilmadi</p>
            ) : (
                <table className="customers-table">
                    <thead>
                        <tr>
                            <th>№</th>
                            <th>Ism</th>
                            <th>Telefon</th>
                            <th>Manzil</th>
                            <th>Faol</th>
                            <th>Yaratilgan</th>
                            <th>Yangilangan</th>
                            <th>Amallar</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filteredCustomers.map((customer, index) => (
                            <tr key={customer.id || index}>
                                <td data-label="№">{index + 1}</td>
                                <td data-label="Ism">{customer.fullName || "Noma'lum"}</td>
                                <td data-label="Telefon">{customer.phoneNumber || "—"}</td>
                                <td data-label="Manzil" title={!isMobile ? customer.address : undefined}>
                                    {isMobile ? (
                                        splitByWords(customer.address || "", 30).map((line, i) => (
                                            <p key={i} style={{ margin: i === 0 ? "0" : "4px 0" }}>{line}</p>
                                        ))
                                    ) : (
                                        customer.address.length > 30
                                            ? customer.address.slice(0, 30) + "..."
                                            : customer.address || "—"
                                    )}
                                </td>
                                <td data-label="Faol">
                                    <span
                                        className={customer.isActive ? "badge-active" : "badge-inactive"}
                                    >
                                        {customer.isActive ? "Ha" : "Yo‘q"}
                                    </span>
                                </td>
                                <td data-label="Yaratilgan">{formatDateToDDMMYYYY(customer.createdDate)}</td>
                                <td data-label="Yangilangan">{formatDateToDDMMYYYY(customer.updatedDate)}</td>
                                <td data-label="Amallar" className="action-buttons">
                                    <button className="edit-btn" onClick={() => handleEdit(customer)}>
                                        <FaEdit />
                                    </button>
                                    <button className="delete-btn" onClick={() => handleDelete(customer.id)}>
                                        <FaTrash />
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            <CustomerFormModal
                isOpen={modalOpen}
                onClose={() => setModalOpen(false)}
                onSave={handleSave}
                customer={editingCustomer}
            />
        </Layout>
    );
}

export default Customers;