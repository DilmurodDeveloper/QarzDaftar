import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { FaBars, FaUserCircle, FaChevronDown } from "react-icons/fa";
import "../Layout/Layout.css";

function Header({ onLogout, toggleSidebar, userName }) {
    const [menuOpen, setMenuOpen] = useState(false);
    const navigate = useNavigate();

    const handleMenuToggle = () => {
        setMenuOpen(!menuOpen);
    };

    const goToProfile = () => {
        navigate("/user/profile");
        setMenuOpen(false);
    };

    return (
        <header className="header">
            <div className="header-left">
                <button className="hamburger-btn" onClick={toggleSidebar}>
                    <FaBars size={15} />
                </button>
                <h1 className="logo">QarzDaftar CRM</h1>
            </div>
            <div className="header-right">
                <div className="profile-menu" onClick={handleMenuToggle}>
                    <FaUserCircle size={25} className="profile-icon" />
                    <span className="username">{userName}</span>
                    <FaChevronDown size={12} className={`dropdown-icon ${menuOpen ? "open" : ""}`} />
                    {menuOpen && (
                        <div className="dropdown-menu">
                            <button onClick={goToProfile}>Profil</button>
                            <button onClick={onLogout}>Chiqish</button>
                        </div>
                    )}
                </div>
            </div>
        </header>
    );
}

export default Header;