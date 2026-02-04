import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import AuthService from '../../services/authService';
import { jwtDecode } from "jwt-decode";

const ProtectedRoute = ({ allowedRoles = [] }) => {
    const user = AuthService.getCurrentUser();
    const location = useLocation();

    if (!user || !user.token) {
        // Not logged in
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    try {
        const decoded = jwtDecode(user.token);
        const currentTime = Date.now() / 1000;

        // Check expiry
        if (decoded.exp < currentTime) {
            AuthService.logout();
            return <Navigate to="/login" state={{ from: location }} replace />;
        }

        // Check role (if allowedRoles are provided)
        // Adjust 'role' property based on your JWT structure (e.g., user.role or decoded.role)
        // Assuming user object has role, or decoded token has it.
        const userRole = user.role || decoded.role || decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

        if (allowedRoles.length > 0 && !allowedRoles.some(role => userRole?.toLowerCase() === role.toLowerCase())) {
            // Role not authorized, redirect to home or unauthorized page
            return <Navigate to="/" replace />;
        }

        return <Outlet />;

    } catch (error) {
        console.error("Invalid token in protected route", error);
        AuthService.logout();
        return <Navigate to="/login" state={{ from: location }} replace />;
    }
};

export default ProtectedRoute;
