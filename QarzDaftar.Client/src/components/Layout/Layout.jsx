import React, { useState, useEffect } from "react";
import Header from "../Header/Header";
import Sidebar from "../Sidebar/Sidebar";
import "./Layout.css";

function Layout({ children, hideSidebar }) {
    const [sidebarOpen, setSidebarOpen] = useState(window.innerWidth > 768);
    const [isMobile, setIsMobile] = useState(window.innerWidth <= 768);
    const toggleSidebar = () => setSidebarOpen((prev) => !prev);

    const handleLogout = () => {
        localStorage.clear();
        window.location.href = "/login";
    };

    const storedUser = localStorage.getItem("user");
    const userName = storedUser ? JSON.parse(storedUser).fullName : "Foydalanuvchi";

    useEffect(() => {
        document.documentElement.classList.add("layout-scroll-lock");
        document.body.classList.add("layout-scroll-lock");
        document.getElementById("root")?.classList.add("layout-scroll-lock");

        const handleResize = () => {
            const mobile = window.innerWidth <= 768;
            setIsMobile(mobile);

            if (mobile) {
                setSidebarOpen(false);
            }
        };

        window.addEventListener("resize", handleResize);
        handleResize();

        return () => {
            document.documentElement.classList.remove("layout-scroll-lock");
            document.body.classList.remove("layout-scroll-lock");
            document.getElementById("root")?.classList.remove("layout-scroll-lock");

            window.removeEventListener("resize", handleResize);
        };
    }, []);

    return (
        <div className="layout-container">
            <Header userName={userName} onLogout={handleLogout} toggleSidebar={toggleSidebar} />

            <div className="main-content">
                {!hideSidebar && (
                    <aside className={`sidebar ${sidebarOpen ? "open" : ""}`}>
                        <Sidebar />
                    </aside>
                )}

                {isMobile && sidebarOpen && <div className="overlay-layout" onClick={toggleSidebar}></div>}

                <main className="content-area">{children}</main>
            </div>
        </div>
    );
}

export default Layout;