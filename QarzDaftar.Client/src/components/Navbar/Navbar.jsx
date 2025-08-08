import React, { useState, useEffect } from "react";
import { useNavigate, Link, useLocation } from "react-router-dom";
import "./Navbar.css";

function Navbar({ user, setUser }) {
    const navigate = useNavigate();
    const location = useLocation();

    const [menuOpen, setMenuOpen] = useState(false);
    const [isMobile, setIsMobile] = useState(window.innerWidth <= 768);

    useEffect(() => {
        const handleResize = () => {
            setIsMobile(window.innerWidth <= 768);
            if (window.innerWidth > 768) setMenuOpen(false);
        };
        window.addEventListener("resize", handleResize);
        return () => window.removeEventListener("resize", handleResize);
    }, []);

    const isLoginPage = location.pathname === "/login";
    const isRegisterPage = location.pathname === "/register";

    return (
        <nav className="navbar">
            <div className="navbar-header">
                <h1 className="navbar-logo">
                    <Link to="/" className="navbar-logo-link">
                        QarzDaftar
                    </Link>
                </h1>

                {isMobile && (
                    <button className="menu-toggle" onClick={() => setMenuOpen(!menuOpen)}>
                        ☰
                    </button>
                )}
            </div>

            <ul className={`navbar-ul ${menuOpen || !isMobile ? "open" : ""}`}>
                {!isLoginPage && (
                    <li>
                        <button onClick={() => navigate("/login")} className="navbar-button">
                            Kirish
                        </button>
                    </li>
                )}

                {!isRegisterPage && (
                    <li>
                        <button onClick={() => navigate("/register")} className="navbar-button">
                            Ro'yxatdan o'tish
                        </button>
                    </li>
                )}
            </ul>
        </nav>
    );
}

export default Navbar;
