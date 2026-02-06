import React, { useState, useEffect, Suspense, lazy } from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './components/layout/Navbar';
import Footer from './components/layout/Footer';
import { Loader2 } from 'lucide-react';
import { jwtDecode } from "jwt-decode";
import AuthService from './services/authService';

// Lazy imports
const Home = lazy(() => import('./pages/client/Home'));
const About = lazy(() => import('./pages/client/About'));
const Login = lazy(() => import('./pages/auth/Login'));
const Register = lazy(() => import('./pages/auth/Register'));
const Booking = lazy(() => import('./pages/client/Booking'));
const MyBookings = lazy(() => import('./pages/client/MyBookings'));
const CarSelection = lazy(() => import('./pages/client/CarSelection'));
const HubSelection = lazy(() => import('./pages/client/HubSelection'));
const StaffDashboard = lazy(() => import('./pages/staff/StaffDashboard'));
const AdminDashboard = lazy(() => import('./pages/admin/AdminDashboard'));
const AdminBookings = lazy(() => import('./pages/admin/AdminBookings'));
const AdminVehicles = lazy(() => import('./pages/admin/AdminVehicles'));
const StaffManagement = lazy(() => import('./pages/admin/StaffManagement'));
const ManageBooking = lazy(() => import('./pages/client/ManageBooking'));
const CustomerCare = lazy(() => import('./pages/client/CustomerCare'));
const ExploreVehicles = lazy(() => import('./pages/client/ExploreVehicles'));


// Import Protected Route
import ProtectedRoute from './components/auth/ProtectedRoute';

// Loading Component
const PageLoader = () => (
  <div className="flex h-screen w-full items-center justify-center bg-background">
    <Loader2 className="h-10 w-10 animate-spin text-primary" />
  </div>
);

function App() {
  const [theme, setTheme] = useState(localStorage.getItem('theme') || 'dark');

  useEffect(() => {
    // Theme logic
    localStorage.setItem('theme', theme);
    const root = window.document.documentElement;
    root.classList.remove('light', 'dark');
    root.classList.add(theme);

    // Session Management Logic
    const validateSession = () => {
      const user = AuthService.getCurrentUser();
      if (user && user.token) {
        try {
          const decoded = jwtDecode(user.token);
          const currentTime = Date.now() / 1000;
          if (decoded.exp < currentTime) {
            console.warn("Session expired. Logging out.");
            AuthService.logout();
            window.location.reload(); // Force UI update
          }
        } catch (error) {
          console.error("Invalid token. Logging out.");
          AuthService.logout();
        }
      }
    };
    validateSession();
  }, [theme]);

  const toggleTheme = () => {
    setTheme(prev => prev === 'dark' ? 'light' : 'dark');
  };

  return (
    <Router>
      <div className="flex flex-col min-h-screen bg-background text-foreground">
        <Navbar theme={theme} toggleTheme={toggleTheme} />
        <main className="flex-grow">
          <Suspense fallback={<PageLoader />}>
            <Routes>
              {/* Public Routes */}
              <Route path="/" element={<Home />} />
              <Route path="/about" element={<About />} />
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              <Route path="/customer-care" element={<CustomerCare />} />
              <Route path="/explore-vehicles" element={<ExploreVehicles />} />
              <Route path="/booking" element={<Booking />} />
              <Route path="/select-car" element={<CarSelection />} />
              <Route path="/select-hub" element={<HubSelection />} />

              {/* Client Protected Routes */}
              <Route element={<ProtectedRoute allowedRoles={['CUSTOMER', 'ADMIN', 'STAFF']} />}>
                <Route path="/manage-booking" element={<ManageBooking />} />
                <Route path="/my-bookings" element={<MyBookings />} />
              </Route>

              {/* Staff Protected Routes */}
              <Route element={<ProtectedRoute allowedRoles={['STAFF', 'ADMIN']} />}>
                <Route path="/staff/dashboard" element={<StaffDashboard />} />
                <Route path="/staff/handover" element={<StaffDashboard />} />
                <Route path="/staff/return" element={<StaffDashboard />} />
              </Route>

              {/* Admin Protected Routes */}
              <Route element={<ProtectedRoute allowedRoles={['ADMIN']} />}>
                <Route path="/admin/dashboard" element={<AdminDashboard />} />
                <Route path="/admin/bookings" element={<AdminBookings />} />
                <Route path="/admin/vehicles" element={<AdminVehicles />} />
                <Route path="/admin/staff" element={<StaffManagement />} />
              </Route>

              {/* 404 Route */}

            </Routes>
          </Suspense>
        </main>
        <Footer />
      </div>
    </Router >
  );
}

export default App;
