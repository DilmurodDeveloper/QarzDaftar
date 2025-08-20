import React, { useState } from "react";
import { FaPlayCircle, FaPhoneAlt } from "react-icons/fa";
import welcomeImage from "../../assets/welcome.jpg";
import ScrollToTop from "../../components/ScrollToTop/ScrollToTop";
import "./Welcome.css";

function Welcome({ videoRef, registrationRef }) {
    const [name, setName] = useState("");
    const [phone, setPhone] = useState("");
    const [submitted, setSubmitted] = useState(false);

    const handleSubmit = (e) => {
        e.preventDefault();
        setSubmitted(true);
        setName("");
        setPhone("");
    };

    const handleScrollToVideo = () => {
        videoRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    const handleScrollToRegistration = (e) => {
        e.preventDefault();
        registrationRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    const [activeIndex, setActiveIndex] = useState(null);

    const toggleFAQ = (index) => {
        setActiveIndex(activeIndex === index ? null : index);
    };

    const faqData = [
        { q: "QarzDaftar nima?", a: "QarzDaftarCRM - do‘kon egalari va mijozlar o‘rtasidagi qarz va to‘lovlarni samarali boshqarish uchun mo‘ljallangan tizim." },
        { q: "Bu kim uchun?", a: "Asosan kichik va o‘sib borayotgan biznes egalari, do‘konlar va tadbirkorlar uchun." },
        { q: "QarzDaftar qanday ishlaydi?", a: "Foydalanuvchi ro‘yxatdan o‘tadi, mijozlarni qo‘shadi, qarz va to‘lovlarni kiritadi va tizim avtomatik hisob-kitob qiladi." },
        { q: "QarzDaftar xavfsizmi?", a: "Ha, barcha ma’lumotlar xavfsiz serverlarda saqlanadi va foydalanuvchi maxfiyligi himoyalanadi." },
    ];

    return (
        <>
            <section className="welcome-section">
                <div className="welcome-content">
                    <div className="text-column">
                        <div className="welcome-header">
                            <h1>Xush Kelibsiz!</h1>
                        </div>
                        <p className="welcome-description">
                            QarzDaftar CRM – Do‘kon egalari uchun zamonaviy raqamli yordamchi.
                            Qarzdorlik va to‘lovlarni aniq hisoblash, mijozlar ro‘yxatini yuritish, qarz yozish, to‘lovlarni kiritish hamda eslatmalar qo‘shish imkonini beradi.
                        </p>
                        <div className="howitworks-wrap">
                            <button className="howitworks-btn" onClick={handleScrollToVideo}>
                                <FaPlayCircle className="howitworks-icon" />
                                Bu qanday ishlaydi?
                            </button>
                            <a href="tel:+998991437101" className="phone-support">
                                <FaPhoneAlt className="phone-icon" />
                                <div className="phone-text">
                                    <span className="phone-title">ONLINE SUPPORT</span>
                                    <span className="phone-number">+998 99 143 71 01</span>
                                </div>
                            </a>
                        </div>
                    </div>

                    <div className="icon-column">
                        <div className="images-stack framed-image">
                            <img src={welcomeImage} alt="Desktop rasm" className="image-desktop" />
                        </div>
                    </div>
                </div>
            </section>

            <section className="offer-section">
                <h2 className="offer-title">Biz nimalarni taklif qilamiz?</h2>
                <p className="offer-subtitle">
                    Biz do‘kon egalari va mijozlar o‘rtasidagi qarz va to‘lovlarni boshqarishni
                    raqamlashtirish orqali qulay va samarali tizim yaratmoqdamiz.
                </p>

                <div className="offer-grid">
                    <div className="offer-card">
                        <h3>Qarz va To‘lovlarni Boshqarish</h3>
                        <p>
                            Har bir mijoz uchun qarz yozish, to‘lovlarni qayd etish va qarzdorlikni
                            kuzatib borish imkoniyati.
                        </p>
                    </div>

                    <div className="offer-card">
                        <h3>Mijozlar Ro‘yxati</h3>
                        <p>
                            Mijozlaringizni yagona platformada saqlang, ularning qarz tarixi va
                            to‘lov ma’lumotlarini oson kuzating.
                        </p>
                    </div>

                    <div className="offer-card">
                        <h3>Hisobot va Tahlil</h3>
                        <p>
                            Daromad, qarzdorlik va to‘lovlar bo‘yicha avtomatik hisobotlar hamda
                            vizual grafikalar.
                        </p>
                    </div>

                    <div className="offer-card">
                        <h3>Eslatmalar va Bildirishnomalar</h3>
                        <p>
                            Mijozlarga qarz haqida eslatma yuborish va o‘z vaqtida ogohlantirish
                            orqali samaradorlikni oshiring.
                        </p>
                    </div>
                </div>
            </section>

            <section className="video-section" ref={videoRef}>
                <h2>QarzDaftar ilovasining demo ko‘rinishi</h2>
                <div className="video-container">
                    <iframe
                        width="100%"
                        height="500"
                        src="https://www.youtube.com/embed/YOUR_VIDEO_ID"
                        title="Loyihani ishlatish video"
                        frameBorder="0"
                        allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                        allowFullScreen
                    ></iframe>
                </div>
            </section>

            <section className="pricing-section">
                <h2>Ta'riflar</h2>
                <p className="offer-subtitle">
                    O'zingizga mos keladigan tarifni tanlang va ilovani sinab ko'ring.
                    Bizning tariflarimiz sizning biznesingiz uchun qulay va samarali yechimlarni taklif etadi.
                </p>
                <div className="pricing-cards">
                    <div className="pricing-card">
                        <h3>7 kunlik sinov</h3>
                        <p className="price">Bepul</p>
                        <a href="#registration" className="activate-btn" onClick={handleScrollToRegistration}>
                            Faollashtirish
                        </a>
                    </div>
                    <div className="pricing-card">
                        <h3>1 oylik obuna</h3>
                        <p className="price">50,000 UZS</p>
                        <a href="#registration" className="activate-btn" onClick={handleScrollToRegistration}>
                            Faollashtirish
                        </a>
                    </div>
                    <div className="pricing-card">
                        <h3>3 oylik obuna</h3>
                        <p className="price">140,000 UZS</p>
                        <a href="#registration" className="activate-btn" onClick={handleScrollToRegistration}>
                            Faollashtirish
                        </a>
                    </div>
                    <div className="pricing-card">
                        <h3>1 yillik obuna</h3>
                        <p className="price">500,000 UZS</p>
                        <a href="#registration" className="activate-btn" onClick={handleScrollToRegistration}>
                            Faollashtirish
                        </a>
                    </div>
                </div>
            </section>

            <section className="faq-section">
                <h2>Ko'p so‘raladigan savollar(FAQ)</h2>
                {faqData.map((item, index) => (
                    <div
                        key={index}
                        className={`faq-item ${activeIndex === index ? "active" : ""}`}
                        onClick={() => toggleFAQ(index)}
                    >
                        <h4>{item.q}</h4>
                        <p>{item.a}</p>
                    </div>
                ))}
            </section>

            <section className="registration-section" ref={registrationRef} id="registration">
                <h2>Ilovani sinab ko'rish</h2>
                <p>Ismingiz va telefon raqamingizni qoldiring, biz siz bilan bog‘lanamiz:</p>
                <form className="registration-form" onSubmit={handleSubmit}>
                    <input
                        type="text"
                        placeholder="Ismingiz"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        required
                    />
                    <input
                        type="tel"
                        placeholder="Telefon raqamingiz"
                        value={phone}
                        onChange={(e) => setPhone(e.target.value)}
                        required
                    />
                    <button type="submit">Yuborish</button>
                </form>
                {submitted && <p className="form-success">Ma’lumot yuborildi, tez orada siz bilan bog‘lanamiz!</p>}
            </section>

            <ScrollToTop />
            
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
