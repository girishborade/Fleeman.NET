import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import ApiService from '../../services/api';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import {
    LayoutDashboard,
    Upload,
    IndianRupee,
    Car,
    CalendarCheck,
    Users,
    Link,
    Plus,
    X,
    Activity,
    CheckCircle2,
    AlertCircle,
    ArrowRight,
    Terminal,
    Wrench,
    Eraser,
    Search,
    Loader2
} from "lucide-react";
import Swal from 'sweetalert2';

const AdminDashboard = () => {
    const navigate = useNavigate();
    const [file, setFile] = useState(null);
    const [message, setMessage] = useState({ type: '', text: '' });
    const [loading, setLoading] = useState(false);
    const [uploadKey, setUploadKey] = useState(Date.now());
    const [vendors, setVendors] = useState([]);
    const [newVendor, setNewVendor] = useState({ name: '', type: 'Maintenance', email: '', apiUrl: 'https://api.example.com/v1' });
    const [showVendorForm, setShowVendorForm] = useState(false);


    // Auto-clear message after 5 seconds
    React.useEffect(() => {
        if (message.text) {
            const timer = setTimeout(() => {
                setMessage({ type: '', text: '' });
            }, 5000);
            return () => clearTimeout(timer);
        }
    }, [message]);

    React.useEffect(() => {
        loadVendors();
    }, []);

    const loadVendors = async () => {
        try {
            const data = await ApiService.getAllVendors();
            setVendors(data);
        } catch (e) {
            console.error(e);
        }
    };









    const handleRateUpload = async (e) => {
        e.preventDefault();
        uploadFile(file, ApiService.uploadRates, 'Rates');
    };

    const handleCarUpload = async (e) => {
        e.preventDefault();
        uploadFile(file, ApiService.uploadCars, 'Cars');
    };

    const handleAddVendor = async () => {
        if (!newVendor.name) return;
        try {
            await ApiService.addVendor(newVendor);
            setMessage({ type: 'success', text: 'Vendor added successfully.' });
            setNewVendor({ name: '', type: 'Maintenance', email: '', apiUrl: 'https://api.example.com/v1' });
            setShowVendorForm(false);
            loadVendors();
        } catch (e) {
            setMessage({ type: 'danger', text: 'Failed to add vendor.' });
        }
    };

    const handleTestConnection = async (id) => {
        try {
            const res = await ApiService.testVendorConnection(id);
            Swal.fire('Connection Success', res.message, 'success');
        } catch (e) {
            Swal.fire('Connection Failed', (e.response?.data?.message || e.message), 'error');
        }
    };

    const uploadFile = async (currentFile, apiMethod, typeName) => {
        if (!currentFile) {
            setMessage({ type: 'warning', text: 'Please select a file first.' });
            return;
        }

        setLoading(true);
        setMessage({ type: '', text: '' });

        try {
            const res = await apiMethod(currentFile);
            setMessage({ type: 'success', text: `${typeName} synchronization completed successfully!` });
            setFile(null);
            setUploadKey(Date.now());
        } catch (err) {
            console.error(`${typeName} upload error:`, err);
            const errMsg = err.response?.data?.message || err.message || 'Unknown error';
            setMessage({ type: 'danger', text: 'Upload failed: ' + errMsg });
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen bg-muted/30 py-12">
            <div className="container mx-auto px-6">
                <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-12 gap-6 animate-in fade-in slide-in-from-top-4 duration-700">
                    <div>
                        <div className="flex items-center gap-2 text-primary font-bold mb-2">
                            <LayoutDashboard className="h-5 w-5" /> Admin Intelligence Console
                        </div>
                        <h1 className="text-4xl md:text-5xl font-black tracking-tight uppercase">
                            Operational <span className="text-primary italic">Control</span>
                        </h1>
                        <p className="text-muted-foreground mt-2">Scale fleet logistics, pricing models, and ecosystem integrations.</p>
                    </div>
                    {loading && (
                        <div className="bg-primary/10 border border-primary/20 px-4 py-2 rounded-xl flex items-center gap-3 animate-pulse">
                            <Loader2 className="h-4 w-4 text-primary animate-spin" />
                            <span className="text-sm font-bold text-primary">Executing Sync Tasks...</span>
                        </div>
                    )}
                </div>

                {/* Notifications */}
                {message.text && (
                    <div className={`mb-8 p-4 rounded-2xl border flex items-center gap-4 animate-in slide-in-from-right-4 duration-500 shadow-lg ${message.type === 'success' ? 'bg-emerald-500/10 border-emerald-500/20 text-emerald-600' :
                        message.type === 'warning' ? 'bg-amber-500/10 border-amber-500/20 text-amber-600' :
                            'bg-destructive/10 border-destructive/20 text-destructive'
                        }`}>
                        {message.type === 'success' ? <CheckCircle2 className="h-6 w-6" /> : <AlertCircle className="h-6 w-6" />}
                        <div className="flex-grow">
                            <p className="font-black uppercase text-[10px] tracking-widest mb-0.5">{message.type === 'success' ? 'Task Verified' : 'Attention Required'}</p>
                            <p className="font-medium text-sm">{message.text}</p>
                        </div>
                        <Button variant="ghost" size="icon" onClick={() => setMessage({ type: '', text: '' })} className="hover:bg-transparent">
                            <X className="h-4 w-4" />
                        </Button>
                    </div>
                )}

                <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-8 mb-12">
                    {/* SYNC RATES */}
                    <Card className="border-none shadow-xl bg-card overflow-hidden hover:-translate-y-1 transition-transform">
                        <CardContent className="p-8 text-center flex flex-col items-center">
                            <div className="h-16 w-16 rounded-2xl bg-primary/10 flex items-center justify-center text-primary mb-6">
                                <IndianRupee className="h-8 w-8" />
                            </div>
                            <h3 className="text-xl font-black mb-2 uppercase">Sync Rates</h3>
                            <p className="text-xs text-muted-foreground mb-8 leading-relaxed">Dynamic pricing architecture & hub-specific rate charts.</p>
                            <form onSubmit={handleRateUpload} className="w-full space-y-4">
                                <div className="relative group">
                                    <Input
                                        key={`rate-${uploadKey}`}
                                        type="file"
                                        accept=".xlsx"
                                        onChange={(e) => setFile(e.target.files[0])}
                                        className="h-12 bg-muted/50 border-none rounded-xl cursor-pointer file:font-black file:uppercase file:text-[10px] file:bg-primary file:text-primary-foreground file:rounded-lg file:h-8 file:px-3 file:mr-4 file:hover:bg-primary/90 transition-all font-medium text-xs"
                                    />
                                </div>
                                <Button type="submit" className="w-full h-12 rounded-xl font-black uppercase tracking-widest text-[10px] shadow-lg shadow-primary/20" disabled={loading}>
                                    {loading ? 'Processing...' : 'Execute Sync'}
                                </Button>
                            </form>
                        </CardContent>
                    </Card>

                    {/* FLEET EXPANSION */}
                    <Card className="border-none shadow-xl bg-card overflow-hidden hover:-translate-y-1 transition-transform">
                        <CardContent className="p-8 text-center flex flex-col items-center">
                            <div className="h-16 w-16 rounded-2xl bg-primary/10 flex items-center justify-center text-primary mb-6">
                                <Car className="h-8 w-8" />
                            </div>
                            <h3 className="text-xl font-black mb-2 uppercase">Expand Fleet</h3>
                            <p className="text-xs text-muted-foreground mb-8 leading-relaxed">Automated vehicle onboarding & inventory synchronization.</p>
                            <form onSubmit={handleCarUpload} className="w-full space-y-4">
                                <Input
                                    key={`car-${uploadKey}`}
                                    type="file"
                                    accept=".xlsx"
                                    onChange={(e) => setFile(e.target.files[0])}
                                    className="h-12 bg-muted/50 border-none rounded-xl cursor-pointer file:font-black file:uppercase file:text-[10px] file:bg-primary file:text-primary-foreground file:rounded-lg file:h-8 file:px-3 file:mr-4 file:hover:bg-primary/90 transition-all font-medium text-xs"
                                />
                                <Button type="submit" className="w-full h-12 rounded-xl font-black uppercase tracking-widest text-[10px] shadow-lg shadow-primary/20" disabled={loading}>
                                    {loading ? 'Processing...' : 'Upload Inventory'}
                                </Button>
                            </form>
                        </CardContent>
                    </Card>

                    {/* BOOKING INSIGHTS */}
                    <Card className="border-none shadow-xl bg-card overflow-hidden hover:-translate-y-1 transition-transform border-t-4 border-emerald-500">
                        <CardContent className="p-8 text-center flex flex-col items-center">
                            <div className="h-16 w-16 rounded-2xl bg-emerald-500/10 flex items-center justify-center text-emerald-600 mb-6">
                                <CalendarCheck className="h-8 w-8" />
                            </div>
                            <h3 className="text-xl font-black mb-2 uppercase">Live Matrix</h3>
                            <p className="text-xs text-muted-foreground mb-8 leading-relaxed">Real-time surveillance of global reservations & schedules.</p>
                            <Button onClick={() => navigate('/admin/bookings')} variant="outline" className="w-full h-14 rounded-2xl font-black uppercase tracking-widest text-[10px] border-emerald-500/20 hover:bg-emerald-500 hover:text-white transition-all gap-2 mt-4">
                                View Records <ArrowRight className="h-3.5 w-3.5" />
                            </Button>
                        </CardContent>
                    </Card>

                    {/* OPERATOR CONTROL */}
                    <Card className="border-none shadow-xl bg-card overflow-hidden hover:-translate-y-1 transition-transform border-t-4 border-primary">
                        <CardContent className="p-8 text-center flex flex-col items-center">
                            <div className="h-16 w-16 rounded-2xl bg-primary/10 flex items-center justify-center text-primary mb-6">
                                <Users className="h-8 w-8" />
                            </div>
                            <h3 className="text-xl font-black mb-2 uppercase">Operators</h3>
                            <p className="text-xs text-muted-foreground mb-8 leading-relaxed">Administrative access controls & personnel management.</p>
                            <Button onClick={() => navigate('/admin/staff')} className="w-full h-14 rounded-2xl font-black uppercase tracking-widest text-[10px] shadow-lg shadow-primary/20 gap-2 mt-4">
                                Staff Portal <ArrowRight className="h-3.5 w-3.5" />
                            </Button>
                        </CardContent>
                    </Card>

                    {/* VEHICLE MANAGEMENT */}
                    <Card className="border-none shadow-xl bg-card overflow-hidden hover:-translate-y-1 transition-transform border-t-4 border-violet-500">
                        <CardContent className="p-8 text-center flex flex-col items-center">
                            <div className="h-16 w-16 rounded-2xl bg-violet-500/10 flex items-center justify-center text-violet-600 mb-6">
                                <Car className="h-8 w-8" />
                            </div>
                            <h3 className="text-xl font-black mb-2 uppercase">Vehicles</h3>
                            <p className="text-xs text-muted-foreground mb-8 leading-relaxed">Fleet status monitoring & vehicle availability tracking.</p>
                            <Button
                                onClick={() => navigate('/admin/vehicles')}
                                variant="outline"
                                className="w-full h-14 rounded-2xl font-black uppercase tracking-widest text-[10px] border-violet-500/20 hover:bg-violet-500 hover:text-white transition-all gap-2 mt-4"
                            >
                                View Fleet <ArrowRight className="h-3.5 w-3.5" />
                            </Button>
                        </CardContent>
                    </Card>
                </div>




            </div>
        </div>
    );
};

export default AdminDashboard;
