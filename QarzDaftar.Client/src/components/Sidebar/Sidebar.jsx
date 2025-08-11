import React, { useState, useEffect } from "react";
import {
    FaHome,
    FaUsers,
    FaMoneyBillWave,
    FaCoins,
    FaCreditCard,
    FaChartBar,
    FaStickyNote,
    FaChevronRight,
    FaChevronDown,
    FaUserCheck,
    FaUserTimes,
    FaHeadset
} from "react-icons/fa";
import { NavLink, useLocation } from "react-router-dom";
import "../Layout/Layout.css";

function Sidebar() {
    const location = useLocation();

    const [customersMenuOpen, setCustomersMenuOpen] = useState(
        location.pathname.startsWith("/user/customers")
    );

    const [debtsMenuOpen, setDebtsMenuOpen] = useState(
        location.pathname.startsWith("/user/debts")
    );

    const toggleCustomersMenu = () => {
        setCustomersMenuOpen(prev => !prev);
    };

    const toggleDebtsMenu = () => {
        setDebtsMenuOpen(prev => !prev);
    };

    useEffect(() => {
        if (location.pathname.startsWith("/user/customers")) {
            setCustomersMenuOpen(true);
        }
        if (location.pathname.startsWith("/user/debts")) {
            setDebtsMenuOpen(true);
        }
    }, [location.pathname]);

    const menuItems = [
        { label: "To'lovlar", icon: <FaCreditCard />, path: "/user/payments" },
        { label: "Hisobotlar", icon: <FaChartBar />, path: "/user/reports" },
        { label: "Eslatmalar", icon: <FaStickyNote />, path: "/user/notes" },
        { label: "Support", icon: <FaHeadset />, path: "/support" },
    ];

    return (
        <nav>
            <NavLink
                to="/user/dashboard"
                className={({ isActive }) =>
                    `sidebar-item ${isActive ? "active" : ""}`
                }
            >
                <span className="icon"><FaHome /></span>
                <span className="label">Asosiy</span>
            </NavLink>

            <div
                className="sidebar-item customers-toggle"
                onClick={toggleCustomersMenu}
                role="button"
                tabIndex={0}
                onKeyPress={(e) => { if (e.key === 'Enter') toggleCustomersMenu(); }}
                style={{ userSelect: "none" }}
            >
                <span className="icon"><FaUsers /></span>
                <span className="label">Mijozlar</span>
                <span className="chevron-icon">
                    {customersMenuOpen ? <FaChevronDown /> : <FaChevronRight />}
                </span>
            </div>

            {customersMenuOpen && (
                <div className="submenu">
                    <NavLink
                        to="/user/customers/all"
                        className={({ isActive }) =>
                            `sidebar-subitem ${isActive ? "active" : ""}`
                        }
                    >
                        <FaUsers className="submenu-icon" /> Barcha mijozlar
                    </NavLink>
                    <NavLink
                        to="/user/customers/active"
                        className={({ isActive }) =>
                            `sidebar-subitem ${isActive ? "active" : ""}`
                        }
                    >
                        <FaUserCheck className="submenu-icon" /> Faol mijozlar
                    </NavLink>
                    <NavLink
                        to="/user/customers/inactive"
                        className={({ isActive }) =>
                            `sidebar-subitem ${isActive ? "active" : ""}`
                        }
                    >
                        <FaUserTimes className="submenu-icon" /> Nofaol mijozlar
                    </NavLink>
                </div>
            )}

            <div
                className="sidebar-item debts-toggle"
                onClick={toggleDebtsMenu}
                role="button"
                tabIndex={0}
                onKeyPress={(e) => { if (e.key === 'Enter') toggleDebtsMenu(); }}
                style={{ userSelect: "none" }}
            >
                <span className="icon"><FaMoneyBillWave /></span>
                <span className="label">Qarzlar</span>
                <span className="chevron-icon">
                    {debtsMenuOpen ? <FaChevronDown /> : <FaChevronRight />}
                </span>
            </div>

            {debtsMenuOpen && (
                <div className="submenu">
                    <NavLink
                        to="/user/debts/all"
                        className={({ isActive }) =>
                            `sidebar-subitem ${isActive ? "active" : ""}`
                        }
                    >
                        <FaCoins className="submenu-icon" /> Barcha qarzlar
                    </NavLink>
                    <NavLink
                        to="/user/debts/summary"
                        className={({ isActive }) =>
                            `sidebar-subitem ${isActive ? "active" : ""}`
                        }
                    >
                        <FaUsers className="submenu-icon" /> Qarzdorlar
                    </NavLink>
                </div>
            )}

            {menuItems.map((item, index) => (
                <NavLink
                    key={index}
                    to={item.path}
                    className={({ isActive }) =>
                        `sidebar-item ${isActive ? "active" : ""}`
                    }
                >
                    <span className="icon">{item.icon}</span>
                    <span className="label">{item.label}</span>
                </NavLink>
            ))}
        </nav>
    );
}

export default Sidebar;