import React, { useState, useEffect } from "react";
import Layout from "../../../components/Layout/Layout";
import { FaPlus, FaEdit, FaTrash, FaStickyNote } from "react-icons/fa";
import "./Notes.css";

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

const statusLabels = {
    0: "Kutilmoqda",
    1: "Tugallangan",
    2: "Muddati o'tgan",
};

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

function Notes() {
    const [notes, setNotes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [currentNote, setCurrentNote] = useState(null);
    const token = localStorage.getItem("accessToken");
    const isMobile = useIsMobile();

    const fetchNotes = async () => {
        if (!token) {
            setError("Token topilmadi. Iltimos, qayta tizimga kiring.");
            setLoading(false);
            return;
        }
        try {
            const res = await fetch(`${API_BASE_URL}/UserNotes/All`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) throw new Error("Eslatmalarni olishda xatolik yuz berdi.");
            const data = await res.json();
            const list = Array.isArray(data) ? data : data.data ?? [];
            setNotes(list);
            setError(null);
        } catch (err) {
            setError(err.message);
            setNotes([]);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchNotes();
    }, []);

    const openAddModal = () => {
        setCurrentNote(null);
        setIsModalOpen(true);
    };

    const openEditModal = (note) => {
        setCurrentNote(note);
        setIsModalOpen(true);
    };

    const closeModal = () => {
        setCurrentNote(null);
        setIsModalOpen(false);
    };

    const deleteNote = async (id) => {
        if (!window.confirm("Ushbu eslatmani o'chirmoqchimisiz?")) return;

        try {
            const res = await fetch(`${API_BASE_URL}/UserNotes?userNoteId=${id}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${token}` },
            });
            if (!res.ok) {
                const errData = await res.json().catch(() => ({}));
                throw new Error(errData.title || "O'chirishda xatolik yuz berdi");
            }
            await fetchNotes();
        } catch (err) {
            alert(err.message);
        }
    };

    const handleFormSubmit = async (e) => {
        e.preventDefault();

        const form = e.target;
        const content = form.content.value.trim();
        const reminderDateRaw = form.reminderDate.value;
        const status = parseInt(form.status.value, 10);

        if (!content || !reminderDateRaw) {
            alert("Majburiy maydonlarni to'ldiring");
            return;
        }

        const user = JSON.parse(localStorage.getItem("user"));
        if (!user?.userId) {
            alert("Foydalanuvchi aniqlanmadi. Iltimos, tizimga qayta kiring.");
            return;
        }

        const userId = user.userId;
        const createdAt = currentNote?.createdAt || new Date().toISOString();
        const reminderDateISO = new Date(reminderDateRaw).toISOString();

        const payload = {
            id: currentNote ? currentNote.id : generateNewGuid(),
            content,
            reminderDate: reminderDateISO,
            createdAt,
            status,
            userId,
        };

        if (currentNote) {
            payload.id = currentNote.id;
        }

        try {
            const res = await fetch(`${API_BASE_URL}/UserNotes`, {
                method: currentNote ? "PUT" : "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${localStorage.getItem("accessToken")}`,
                },
                body: JSON.stringify(payload),
            });

            if (!res.ok) {
                const errData = await res.json();
                console.error("Server error:", errData);
                throw new Error(errData.title || "Xatolik yuz berdi");
            }

            await fetchNotes();
            closeModal();
        } catch (err) {
            alert(err.message);
        }
    };

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
            <nav className="breadcrumb">
                <FaStickyNote className="breadcrumb-icon" />
                <span className="breadcrumb-arrow"> &gt; </span>
                <span>Eslatmalar</span>
            </nav>
            <div className="usernotes-container">
                <div className="usernotes-header">
                    <h2>Eslatmalar ro'yxati</h2>
                    <button className="add-btn" onClick={openAddModal}>
                        <FaPlus /> Eslatma qo'shish
                    </button>
                </div>

                <table className="usernotes-table">
                    <thead>
                        <tr>
                            <th>№</th>
                            <th>Matn</th>
                            <th>Eslatma sanasi</th>
                            <th>Status</th>
                            <th>Amallar</th>
                        </tr>
                    </thead>
                    <tbody>
                        {notes.length ? (
                            notes.map((note, index) => (
                                <tr key={note.id || index}>
                                    <td data-label="№">{index + 1}</td>
                                    <td data-label="Matn" title={!isMobile ? note.content : undefined}>
                                        {isMobile ? (
                                            splitByWords(note.content || "", 30).map((line, i) => (
                                                <p key={i} style={{ margin: i === 0 ? "0" : "4px 0" }}>{line}</p>
                                            ))
                                        ) : (
                                            note.content.length > 30
                                                ? note.content.slice(0, 30) + "..."
                                                : note.content || "—"
                                        )}
                                    </td>
                                    <td data-label="Eslatma sanasi">{formatDateToDDMMYYYY(note.reminderDate)}</td>
                                    <td data-label="Status">{statusLabels[note.status]}</td>
                                    <td data-label="Amallar">
                                        <button
                                            className="edit-btn"
                                            onClick={() => openEditModal(note)}
                                            title="Tahrirlash"
                                        >
                                            <FaEdit />
                                        </button>
                                        <button
                                            className="delete-btn"
                                            onClick={() => deleteNote(note.id)}
                                            title="O'chirish"
                                        >
                                            <FaTrash />
                                        </button>
                                    </td>
                                </tr>
                            ))
                        ) : (
                            <tr>
                                <td colSpan="4" style={{ textAlign: "center" }}>
                                    Eslatmalar mavjud emas
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>

                {isModalOpen && (
                    <div className="modal-overlay">
                        <div className="modal-content">
                            <h3>{currentNote ? "Eslatmani tahrirlash" : "Eslatma qo'shish"}</h3>
                            <form onSubmit={handleFormSubmit}>
                                <label style={{ display: "block", marginBottom: "1rem" }}>
                                    <span style={{ display: "block", marginBottom: "0.5rem", fontWeight: "600", color: "#334155" }}>
                                        Matn:
                                    </span>
                                    <textarea
                                        name="content"
                                        defaultValue={currentNote?.content ?? ""}
                                        required
                                        rows={3}
                                        style={{
                                            width: "100%",
                                            fontSize: "1rem",
                                            border: "1px solid #cbd5e1",
                                            borderRadius: "8px",
                                            padding: "0.5rem",
                                            backgroundColor: "#f8fafc",
                                            outline: "none"
                                        }}
                                    />
                                </label>

                                <label>
                                    Eslatma sanasi:
                                    <input
                                        type="date"
                                        name="reminderDate"
                                        defaultValue={
                                            currentNote
                                                ? new Date(currentNote.reminderDate)
                                                    .toISOString()
                                                    .split("T")[0]
                                                : new Date().toISOString().split("T")[0]
                                        }
                                        required
                                    />
                                </label>

                                <label>
                                    Status:
                                    <select
                                        name="status"
                                        defaultValue={currentNote?.status ?? 0}
                                        required
                                    >
                                        <option value={0}>Kutilmoqda</option>
                                        <option value={1}>Tugallangan</option>
                                        <option value={2}>Muddati o'tgan</option>
                                    </select>
                                </label>

                                <div className="modal-actions">
                                    <button type="submit" className="save-btn">
                                        Saqlash
                                    </button>
                                    <button
                                        type="button"
                                        className="cancel-btn"
                                        onClick={closeModal}
                                    >
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

export default Notes;