import React, { useState, useEffect } from "react";
import "./DebtsFormModal.css";

function DebtsFormModal({ isOpen, onClose, onSave, debt, customers }) {
    const [formData, setFormData] = useState({
        customerId: "",
        amount: "",
        description: "",
        dueDate: "",
    });

    useEffect(() => {
        if (debt) {
            setFormData({
                customerId: debt.customerId || "",
                amount: debt.amount || "",
                description: debt.description || "",
                dueDate: debt.dueDate ? debt.dueDate.substring(0, 10) : "",
            });
        } else {
            setFormData({
                customerId: "",
                amount: "",
                description: "",
                dueDate: "",
            });
        }
    }, [debt]);

    if (!isOpen) return null;

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        onSave(formData);
    };

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <h3>{debt ? "Qarzni yangilash" : "Yangi qarz qo'shish"}</h3>
                <form onSubmit={handleSubmit}>
                    <label>Mijoz</label>
                    <select name="customerId" value={formData.customerId} onChange={handleChange} required>
                        <option value="">Mijozni tanlang</option>
                        {customers.map((c) => (
                            <option key={c.id} value={c.id}>{c.fullName}</option>
                        ))}
                    </select>

                    <label>Qarz summasi</label>
                    <input
                        type="number"
                        name="amount"
                        value={formData.amount}
                        onChange={handleChange}
                        required
                        min="0"
                    />

                    <label>Tavsif</label>
                    <input
                        type="text"
                        name="description"
                        value={formData.description}
                        onChange={handleChange}
                    />

                    <label>To'lov muddati</label>
                    <input
                        type="date"
                        name="dueDate"
                        value={formData.dueDate}
                        onChange={handleChange}
                        required
                    />

                    <div className="modal-actions">
                        <button type="submit" className="save-btn">
                            Saqlash
                        </button>
                        <button type="button" className="cancel-btn" onClick={onClose}>
                            Bekor qilish
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default DebtsFormModal;