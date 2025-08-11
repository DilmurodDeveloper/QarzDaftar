import React from "react";
import { FaTelegramPlane, FaPhoneAlt, FaWhatsapp, FaHeadset } from "react-icons/fa";
import Layout from "../../../components/Layout/Layout";
import "./Support.css";

function Support() {
    const telegramLink = "https://t.me/DilmurodDeveloper";
    const phoneNumber = "+998991437101";
    const whatsappLink = `https://wa.me/${phoneNumber.replace(/\D/g, "")}`;

    return (
        <Layout>
            <nav className="breadcrumb">
                <FaHeadset className="breadcrumb-icon" />
                <span className="breadcrumb-arrow"> &gt; </span>
                <span>Support</span>
            </nav>
            <div className="support-container">
                <h2 className="support-header">Qo'llab-quvvatlash xizmati</h2>
                <p className="support-text">
                    Agar sizga yordam kerak bo'lsa, quyidagi yo'llardan biriga murojaat qilishingiz mumkin:
                </p>

                <div className="support-links">
                    <a
                        href={telegramLink}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="support-link telegram"
                    >
                        <FaTelegramPlane className="icon" />
                        Telegram orqali bog'lanish
                    </a>

                    <a href={`tel:${phoneNumber}`} className="support-link phone">
                        <FaPhoneAlt className="icon" />
                        Telefon: {phoneNumber}
                    </a>

                    <a
                        href={whatsappLink}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="support-link whatsapp"
                    >
                        <FaWhatsapp className="icon" />
                        WhatsApp orqali bog'lanish
                    </a>
                </div>

                <p className="support-footer">
                    Bizning jamoamiz sizga yordam berish uchun har doim tayyor!
                </p>
            </div>
        </Layout>
    );
}

export default Support;