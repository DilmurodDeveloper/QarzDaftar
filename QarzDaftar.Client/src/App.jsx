import { useState } from "react";
import { Routes, Route, useLocation } from "react-router-dom";
import Navbar from "./components/Navbar/Navbar";
import Welcome from "./pages/welcome/Welcome";
import Login from "./pages/auth/Login";
import Register from "./pages/auth/Register";
import Dashboard from "./pages/admin/Dashboard/Dashboard";
import AllCustomers from "./pages/admin/Customers/AllCustomers";
import ActiveCustomers from "./pages/admin/Customers/ActiveCustomers";
import InactiveCustomers from "./pages/admin/Customers/InactiveCustomers";
import DebtsList from "./pages/admin/Debts/DebtsList";
import DebtorsSummary from "./pages/admin/Debts/DebtorsSummary";
import Payments from "./pages/admin/Payments/Payments";
import Reports from "./pages/admin/Reports/Reports";
import Notes from "./pages/admin/Notes/Notes";
import Profile from "./pages/admin/Profile/Profile";
import Support from "./pages/admin/Support/Support";
import './App.css';

function App() {
    const [user, setUser] = useState(null);
    const location = useLocation();

    const hideNavbarOnPaths = [
        "/user/dashboard",
        "/user/customers",
        "/user/customers/all",
        "/user/customers/active",
        "/user/customers/inactive",
        "/user/debts",
        "/user/debts/all",
        "/user/debts/summary",
        "/user/payments",
        "/user/reports",
        "/user/notes",
        "/user/profile",
        "/support"
    ];
    const shouldHideNavbar = hideNavbarOnPaths.includes(location.pathname);

    return (
        <>
            {!shouldHideNavbar && <Navbar user={user} setUser={setUser} />}
            <main>
                <Routes>
                    <Route path="/" element={<Welcome />} />
                    <Route path="/login" element={<Login setUser={setUser} />} />
                    <Route path="/register" element={<Register setUser={setUser} />} />
                    <Route path="/user/dashboard" element={<Dashboard />} />
                    <Route path="/user/customers/all" element={<AllCustomers />} />
                    <Route path="/user/customers/active" element={<ActiveCustomers />} />
                    <Route path="/user/customers/inactive" element={<InactiveCustomers />} />
                    <Route path="/user/debts/all" element={<DebtsList />} />
                    <Route path="/user/debts/summary" element={<DebtorsSummary />} />
                    <Route path="/user/payments" element={<Payments />} />
                    <Route path="/user/reports" element={<Reports />} />
                    <Route path="/user/notes" element={<Notes />} />
                    <Route path="/support" element={<Support />} />
                    <Route path="/user/profile" element={<Profile />} />
                </Routes>
            </main>
        </>
    );
}

export default App;