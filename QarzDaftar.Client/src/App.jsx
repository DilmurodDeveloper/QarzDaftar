import { useState } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Navbar from "./components/Navbar/Navbar";
import Welcome from "./pages/welcome/Welcome";
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import './App.css';

function App() {
    const [user, setUser] = useState(null);

    return (
        <>
            <Navbar user={user} setUser={setUser} />
            <main>
                <Routes>
                    <Route path="/" element={<Welcome />} />
                    <Route path="/login" element={<Login setUser={setUser} />} />
                    <Route path="/register" element={<Register setUser={setUser} />} />
                </Routes>
            </main>
        </>
    );
}

export default App;
