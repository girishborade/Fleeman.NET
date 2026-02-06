import axios from 'axios';

const API_URL = 'http://localhost:5086/';

const AuthService = {
    login: async (username, password) => {
        const response = await axios.post(API_URL + 'login', { username, password });
        if (response.data.token) {
            sessionStorage.setItem('user', JSON.stringify(response.data));
        }
        return response.data;
    },

    googleLogin: async (token) => {
        const response = await axios.post(API_URL + 'api/v1/auth/google', { token });
        if (response.data.token) {
            sessionStorage.setItem('user', JSON.stringify(response.data));
        }
        return response.data;
    },

    register: async (userData) => {
        return axios.post(API_URL + 'register', userData);
    },

    logout: () => {
        sessionStorage.removeItem('user');
    },

    getCurrentUser: () => {
        return JSON.parse(sessionStorage.getItem('user'));
    },

    forgotPassword: async (email) => {
        return axios.post(API_URL + 'forgot-password', { email });
    },

    resetPassword: async (token, password) => {
        return axios.post(API_URL + 'reset-password', { token, password });
    }
};

export default AuthService;
