import React, { useEffect, useState } from "react";
import { toast, ToastContainer } from "react-toastify";
import { FaPlus, FaEdit, FaTrash, FaSearch, FaFilter } from "react-icons/fa";
import Layout from "../../../components/Layout/Layout";
import "react-toastify/dist/ReactToastify.css";
import UserForm from "./UserForm";
import "./Dashboard.css";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

function SuperAdminDashboard() {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [search, setSearch] = useState("");
    const [showModal, setShowModal] = useState(false);
    const [showColumnMenu, setShowColumnMenu] = useState(false);
    const [editingUser, setEditingUser] = useState(null);

    const [visibleColumns, setVisibleColumns] = useState({
        fullName: true,
        username: true,
        email: true,
        phoneNumber: true,
        shopName: true,
        address: true,
        registeredAt: false,
        subscriptionExpiresAt: false,
        isActivatedByAdmin: false,
        isBlocked: false,
        createdDate: true,
        updatedDate: true,
        role: true,
    });

    const token = localStorage.getItem("accessToken");

    const fetchUsers = async () => {
        try {
            const res = await fetch(`${API_BASE_URL}/Users/All`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error("Foydalanuvchilarni olishda xatolik");
            const data = await res.json();
            setUsers(data);
        } catch (err) {
            toast.error(err.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchUsers();
    }, []);

    const handleDelete = async (userId) => {
        if (!window.confirm("Rostdan foydalanuvchini o'chirmoqchimisiz?")) return;
        try {
            const res = await fetch(`${API_BASE_URL}/Users?userId=${userId}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error("O'chirishda xatolik");
            toast.success("Foydalanuvchi o'chirildi");
            fetchUsers();
        } catch (err) {
            toast.error(err.message);
        }
    };

    const filteredUsers = users.filter(
        (user) =>
            user.fullName.toLowerCase().includes(search.toLowerCase()) ||
            user.username.toLowerCase().includes(search.toLowerCase()) ||
            user.email.toLowerCase().includes(search.toLowerCase())
    );

    return (
        <Layout hideSidebar={true}>
            <div className="superadmin-dashboard-container">
                <h2>SuperAdmin Dashboard</h2>
                <ToastContainer />

                <div className="superadmin-dashboard-controls">
                    <div className="superadmin-search-wrapper">
                        <FaSearch className="superadmin-search-icon" />
                        <input
                            type="text"
                            placeholder="Foydalanuvchi qidiring..."
                            value={search}
                            onChange={(e) => setSearch(e.target.value)}
                            className="search-input"
                        />
                    </div>

                    <div className="actions-group">
                        <div className="column-toggle-wrapper">
                            <button
                                className="filter-user-btn"
                                onClick={() => setShowColumnMenu(!showColumnMenu)}
                            >
                                <FaFilter /> Filtr
                            </button>
                            {showColumnMenu && (
                                <div className="column-toggle-menu">
                                    {Object.keys(visibleColumns).map((col) => (
                                        <label key={col} className="column-toggle-label">
                                            <input
                                                type="checkbox"
                                                checked={visibleColumns[col]}
                                                onChange={() =>
                                                    setVisibleColumns((prev) => ({
                                                        ...prev,
                                                        [col]: !prev[col],
                                                    }))
                                                }
                                            />
                                            {col}
                                        </label>
                                    ))}
                                </div>
                            )}
                        </div>

                        <button
                            className="add-user-btn"
                            onClick={() => {
                                setEditingUser(null);
                                setShowModal(true);
                            }}
                        >
                            <FaPlus /> Yangi foydalanuvchi yaratish
                        </button>
                    </div>
                </div>

                {showModal && (
                    <div className="superadmin-modal-overlay" onClick={() => setShowModal(false)}>
                        <div className="superadmin-modal-content" onClick={(e) => e.stopPropagation()}>
                            <UserForm
                                onSuccess={() => {
                                    fetchUsers();
                                    setShowModal(false);
                                }}
                                token={token}
                                editingUser={editingUser}
                            />
                        </div>
                    </div>
                )}

                {loading ? (
                    <p>Yuklanmoqda...</p>
                ) : (
                    <table className="superadmin-users-table">
                        <thead>
                            <tr>
                                <th>№</th>
                                {visibleColumns.fullName && <th>Full Name</th>}
                                {visibleColumns.username && <th>Username</th>}
                                {visibleColumns.email && <th>Email</th>}
                                {visibleColumns.phoneNumber && <th>Phone</th>}
                                {visibleColumns.shopName && <th>Shop Name</th>}
                                {visibleColumns.address && <th>Address</th>}
                                {visibleColumns.registeredAt && <th>Registered At</th>}
                                {visibleColumns.subscriptionExpiresAt && <th>Subscription Expires At</th>}
                                {visibleColumns.isActivatedByAdmin && <th>Activated</th>}
                                {visibleColumns.isBlocked && <th>Blocked</th>}
                                {visibleColumns.createdDate && <th>Created Date</th>}
                                {visibleColumns.updatedDate && <th>Updated Date</th>}
                                {visibleColumns.role && <th>Role</th>}
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {filteredUsers.map((user, index) => (
                                <tr key={user.id || index}>
                                    <td>{index + 1}</td>
                                    {visibleColumns.fullName && <td>{user.fullName}</td>}
                                    {visibleColumns.username && <td>{user.username}</td>}
                                    {visibleColumns.email && <td>{user.email}</td>}
                                    {visibleColumns.phoneNumber && <td>{user.phoneNumber}</td>}
                                    {visibleColumns.shopName && <td>{user.shopName}</td>}
                                    {visibleColumns.address && <td>{user.address}</td>}
                                    {visibleColumns.registeredAt && <td>{new Date(user.registeredAt).toLocaleString()}</td>}
                                    {visibleColumns.subscriptionExpiresAt && <td>{new Date(user.subscriptionExpiresAt).toLocaleString()}</td>}
                                    {visibleColumns.isActivatedByAdmin && <td>{user.isActivatedByAdmin ? "Ha" : "Yo'q"}</td>}
                                    {visibleColumns.isBlocked && <td>{user.isBlocked ? "Ha" : "Yo'q"}</td>}
                                    {visibleColumns.createdDate && <td>{new Date(user.createdDate).toLocaleString()}</td>}
                                    {visibleColumns.updatedDate && <td>{new Date(user.updatedDate).toLocaleString()}</td>}
                                    {visibleColumns.role && <td>{user.role}</td>}
                                    <td>
                                        <button
                                            onClick={() => {
                                                setEditingUser(user);
                                                setShowModal(true);
                                            }}
                                        >
                                            <FaEdit />
                                        </button>
                                        <button onClick={() => handleDelete(user.id)}>
                                            <FaTrash />
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>
        </Layout>
    );
}

export default SuperAdminDashboard;
