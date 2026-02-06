import axios from 'axios';

import Swal from 'sweetalert2';

const API_URL = 'http://localhost:5086';

const instance = axios.create({
    baseURL: API_URL,
});

// Add a request interceptor to append the token if present
instance.interceptors.request.use(
    config => {
        const user = JSON.parse(sessionStorage.getItem('user'));
        if (user && user.token) {
            config.headers['Authorization'] = 'Bearer ' + user.token;
        }
        return config;
    },
    error => Promise.reject(error)
);

// Add a response interceptor to handle global errors
instance.interceptors.response.use(
    response => response,
    error => {
        // Allow requests to suppress global error handling
        if (error.config && error.config.suppressGlobalErrors) {
            return Promise.reject(error);
        }

        let title = 'Error';
        let text = 'Something went wrong!';

        if (error.response) {
            const status = error.response.status;
            const data = error.response.data;
            // Extract message from backend response if available (Backend often sends string or {message: "..."})
            const backendMessage = typeof data === 'string' ? data : (data?.message || data?.title || 'Unknown error');

            if (status === 401) {
                // Determine if it is a login failure or session expiry
                // We rely on App.jsx for session expiry, but this catches immediate 401s
                title = 'Unauthorized';
                text = backendMessage || 'You need to login to access this resource.';
            } else if (status === 403) {
                title = 'Access Denied';
                text = 'You do not have permission to perform this action.';
            } else if (status === 404) {
                title = 'Not Found';
                text = 'The requested resource was not found.';
            } else if (status >= 500) {
                title = 'Server Error';
                text = backendMessage || 'Our servers are facing issues. Please try again later.';
            } else {
                text = backendMessage;
            }
        } else if (error.request) {
            title = 'Network Error';
            text = 'Unable to connect to the server. Please check your internet connection.';
        }

        Swal.fire({
            icon: 'error',
            title: title,
            text: text,
            confirmButtonColor: '#d33'
        });

        return Promise.reject(error);
    }
);

const ApiService = {
    // Hubs
    getHubs: (stateName, cityName, cityId) => {
        if (cityId) return instance.get(`/api/v1/hubs/city/${cityId}`).then(res => res.data);
        return instance.get('/api/v1/hubs').then(res => res.data);
    },
    searchLocations: (query) => instance.get(`/api/v1/locations/search?query=${query}`).then(res => res.data), // Feature missing in BE

    // States & Cities
    getAllStates: () => instance.get('/api/v1/states').then(res => res.data),
    getCitiesByState: (stateId) => instance.get(`/api/v1/cities/state/${stateId}`).then(res => res.data),

    // Cars
    getCarTypes: () => instance.get('/api/v1/car-types').then(res => res.data),
    getAvailableCars: (hubId, startDate, endDate, carTypeId) => {
        let url = `/api/v1/cars/available?hubId=${hubId}&startDate=${startDate}&endDate=${endDate}`;
        if (carTypeId) url += `&carTypeId=${carTypeId}`;
        return instance.get(url).then(res => res.data);
    },
    uploadCars: (file) => {
        const formData = new FormData();
        formData.append('file', file);
        return instance.post('/api/v1/cars/upload', formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        });
    },

    // Add-ons
    getAddOns: () => instance.get('/api/v1/addons').then(res => res.data),

    // Bookings
    createBooking: (bookingRequest) => instance.post('/booking/create', bookingRequest).then(res => res.data),
    getBooking: (id) => instance.get(`/booking/get/${id}`).then(res => res.data),
    getBookingsByUser: (email) => instance.get(`/booking/user/${email}`).then(res => res.data),
    getBookingsByHub: (hubId) => instance.get(`/booking/hub/${hubId}`).then(res => res.data),
    processHandover: (request) => instance.post('/booking/process-handover', request).then(res => res.data),
    returnCar: (request) => instance.post('/booking/return', request).then(res => res.data),
    cancelBooking: (id) => instance.post(`/booking/cancel/${id}`).then(res => res.data),

    // Customer
    findCustomer: (email) => instance.get(`/api/v1/customers/${email}`, { suppressGlobalErrors: true }).then(res => res.data),
    saveCustomer: (customer) => instance.post('/api/v1/customers', customer).then(res => res.data),

    // Vendors
    getAllVendors: () => instance.get('/api/v1/vendors').then(res => res.data),
    addVendor: (vendor) => instance.post('/api/v1/vendors', vendor).then(res => res.data),
    testVendorConnection: (id) => instance.delete(`/api/v1/vendors/${id}`).then(res => res.data), // Using delete for test? No, sticking to original likely/stub

    // Admin Rates
    uploadRates: (file) => {
        const formData = new FormData();
        formData.append('file', file);
        return instance.post('/api/admin/upload-rates', formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        });
    },

    // Admin Dashboard
    getAllBookings: () => instance.get('/booking/all').then(res => res.data),
    getFleetOverview: (startDate, endDate) => {
        let url = '/api/admin/fleet-overview';
        if (startDate && endDate) {
            url += `?startDate=${startDate}&endDate=${endDate}`;
        }
        return instance.get(url).then(res => res.data);
    },

    // Staff Management (Admin)
    getAdminStaff: () => instance.get('/api/admin/staff').then(res => res.data),
    getAdminHubs: () => instance.get('/api/v1/hubs').then(res => res.data), // Reusing general hubs
    registerStaff: (userData) => instance.post('/api/admin/register-staff', userData).then(res => res.data),
    deleteStaff: (id) => instance.delete(`/api/admin/staff/${id}`).then(res => res.data)
};

export default ApiService;
