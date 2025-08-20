import React, { useState, useEffect } from "react";
import { FaArrowUp } from "react-icons/fa";
import "./ScrollToTop.css";

function ScrollToTop() {
    const [visible, setVisible] = useState(false);

    useEffect(() => {
        const root = document.getElementById("root");

        const toggleVisibility = () => {
            if (root.scrollTop > 200) {
                setVisible(true);
            } else {
                setVisible(false);
            }
        };

        root.addEventListener("scroll", toggleVisibility);
        return () => root.removeEventListener("scroll", toggleVisibility);
    }, []);

    const scrollToTop = () => {
        const root = document.getElementById("root");
        root.scrollTo({ top: 0, behavior: "smooth" });
    };

    if (!visible) return null;

    return (
        <div className="scroll-to-top" onClick={scrollToTop}>
            <FaArrowUp />
        </div>
    );
}

export default ScrollToTop;
