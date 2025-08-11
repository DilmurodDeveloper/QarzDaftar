import React, { useState, useEffect } from "react";
import "./Customers.css";

function CustomerFormModal({ isOpen, onClose, onSave, customer }) {
    const [formData, setFormData] = useState({
        fullName: "",
        phoneNumber: "",
        address: "",
        isActive: true
    });

    useEffect(() => {
        if (customer) {
            setFormData({
                fullName: customer.fullName || "",
                phoneNumber: customer.phoneNumber || "",
                address: customer.address || "",
                isActive: customer.isActive ?? true
            });
        } else {
            setFormData({
                fullName: "",
                phoneNumber: "",
                address: "",
                isActive: true
            });
        }
    }, [customer]);

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: type === "checkbox" ? checked : value
        }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        onSave(formData);
    };

    if (!isOpen) return null;

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <h3>{customer ? "Mijozni tahrirlash" : "Yangi mijoz qo‘shish"}</h3>
                <form onSubmit={handleSubmit}>
                    <label>Ism</label>
                    <input
                        type="text"
                        name="fullName"
                        value={formData.fullName}
                        onChange={handleChange}
                        required
                    />

                    <label>Telefon</label>
                    <input
                        type="text"
                        name="phoneNumber"
                        value={formData.phoneNumber}
                        onChange={handleChange}
                    />

                    <label>Manzil</label>
                    <input
                        type="text"
                        name="address"
                        value={formData.address}
                        onChange={handleChange}
                    />

                    <label className="checkbox">
                        <input
                            type="checkbox"
                            name="isActive"
                            checked={formData.isActive}
                            onChange={handleChange}
                        /> Faol mijoz
                    </label>

                    <div className="modal-actions">
                        <button type="submit" className="save-btn">Saqlash</button>
                        <button type="button" className="cancel-btn" onClick={onClose}>Bekor qilish</button>
                    </div>
                </form>
            </div>
        </div>
    );
}

export default CustomerFormModal;