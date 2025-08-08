import React from "react";
import "./Welcome.css";
import { FaHandshake, FaUsers, FaMoneyBillWave, FaPhoneAlt, FaTelegramPlane } from "react-icons/fa";
import welcomeImage from "../../assets/welcome.svg";

function Welcome() {
    return (
        <>
            <section className="welcome-section">
                <div className="welcome-content">
                    <div className="text-column">
                        <div className="welcome-header">
                            <h2>Xush Kelibsiz!</h2>
                        </div>
                        <p className="welcome-description">
                            <strong>QarzDaftarCRM</strong> - Ushbu loyiha <strong>do'kon egalari</strong> va <strong>mijozlar</strong> o'rtasidagi qarz va to'lovlarni samarali boshqarish uchun mo‘ljallangan.
                            Foydalanuvchilar ro'yxatdan o'tib, mijoz qo'shish, qarz yozish, to'lovni kiritish va eslatmalar yaratish imkoniyatiga ega bo'lishadi.
                        </p>
                        <div className="features">
                            <div className="feature-item">
                                <FaUsers className="feature-icon" />
                                <span>Mijozlarni boshqarish</span>
                            </div>
                            <div className="feature-item">
                                <FaMoneyBillWave className="feature-icon" />
                                <span>Qarz va to‘lovlarni yozish</span>
                            </div>
                            <div className="feature-item">
                                <FaHandshake className="feature-icon" />
                                <span>Eslatmalar yaratish</span>
                            </div>
                        </div>
                        <div className="contact-info">
                            <h3>Onlayn Yordam:</h3>
                            <div className="contact-item">
                                <FaPhoneAlt className="contact-icon" />
                                <a href="tel:+998991437101" className="contact-link">
                                    +998 99 143 71 01
                                </a>
                            </div>
                            <div className="contact-item">
                                <FaTelegramPlane className="contact-icon" />
                                <a
                                    href="https://t.me/DilmurodDeveloper"
                                    target="_blank"
                                    rel="noopener noreferrer"
                                    className="contact-link"
                                >
                                    Telegram
                                </a>
                            </div>
                        </div>
                    </div>

                    <div className="icon-column">
                        <img src={welcomeImage} alt="Welcome" className="finance-image" />
                    </div>
                </div>
            </section>

            <footer className="footer">
                <p>
                    © 2025 Barcha huquqlar{" "}
                    <a
                        href="https://dilmuroddev.uz"
                        target="_blank"
                        rel="noopener noreferrer"
                        className="footer-link"
                    >
                        Dilmurod
                    </a>{" "}
                    tomonidan himoyalangan
                </p>
            </footer>
        </>
    );
}

export default Welcome;
