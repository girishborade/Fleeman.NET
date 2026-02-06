import React, { useState, useEffect } from 'react';
import ApiService from '../../services/api';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { Car, Loader2 } from "lucide-react";

const AdminVehicles = () => {
    const [fleetData, setFleetData] = useState(null);
    const [expandedHubs, setExpandedHubs] = useState({});
    const [searchTerm, setSearchTerm] = useState('');
    const [statusFilter, setStatusFilter] = useState('All');
    const [dateRange, setDateRange] = useState({ startDate: '', endDate: '' });
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadFleetOverview();
    }, [dateRange]);

    const loadFleetOverview = async () => {
        try {
            setLoading(true);
            const data = await ApiService.getFleetOverview(dateRange.startDate, dateRange.endDate);
            setFleetData(data);
            // Expand all hubs by default
            const expanded = {};
            if (data?.hubs) {
                data.hubs.forEach(hub => { expanded[hub.hubId] = true; });
            }
            setExpandedHubs(expanded);
        } catch (e) {
            console.error('Failed to load fleet overview:', e);
        } finally {
            setLoading(false);
        }
    };

    const toggleHub = (hubId) => {
        setExpandedHubs(prev => ({ ...prev, [hubId]: !prev[hubId] }));
    };

    const getFilteredCars = (cars) => {
        return cars.filter(car => {
            const matchesSearch = car.model.toLowerCase().includes(searchTerm.toLowerCase()) ||
                car.registrationNumber.toLowerCase().includes(searchTerm.toLowerCase());
            const matchesStatus = statusFilter === 'All' || car.status === statusFilter;
            return matchesSearch && matchesStatus;
        });
    };

    const getStatusColor = (status) => {
        switch (status) {
            case 'Available': return 'bg-emerald-500/10 text-emerald-600 border-emerald-500/20';
            case 'Rented': return 'bg-blue-500/10 text-blue-600 border-blue-500/20';
            case 'Maintenance': return 'bg-amber-500/10 text-amber-600 border-amber-500/20';
            default: return 'bg-gray-500/10 text-gray-600 border-gray-500/20';
        }
    };

    if (loading) {
        return (
            <div className="flex h-screen w-full items-center justify-center bg-muted/30">
                <Loader2 className="h-10 w-10 animate-spin text-primary" />
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-muted/30 py-12">
            <div className="container mx-auto px-6">
                <div className="mb-12 animate-in fade-in slide-in-from-top-4 duration-700">
                    <div className="flex items-center gap-2 text-primary font-bold mb-2">
                        <Car className="h-5 w-5" /> Vehicle Management
                    </div>
                    <h1 className="text-4xl md:text-5xl font-black tracking-tight uppercase">
                        Fleet <span className="text-primary italic">Status</span>
                    </h1>
                    <p className="text-muted-foreground mt-2">Monitor vehicle availability and status across all hubs.</p>
                </div>

                {fleetData && (
                    <Card className="border-none shadow-2xl bg-card overflow-hidden mt-12">
                        <CardHeader className="p-8 bg-muted/50 border-bottom border-border/50">
                            <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
                                <div>
                                    <CardTitle className="text-2xl font-black uppercase tracking-tight">Fleet Overview</CardTitle>
                                    <CardDescription className="font-medium">Real-time vehicle status across all hubs</CardDescription>
                                </div>
                                <div className="flex flex-wrap gap-4">
                                    <div className="bg-emerald-500/10 border border-emerald-500/20 px-4 py-2 rounded-xl">
                                        <p className="text-[10px] font-black uppercase text-muted-foreground tracking-widest">Available</p>
                                        <p className="text-2xl font-black text-emerald-600">{fleetData.statistics.totalAvailable}</p>
                                    </div>
                                    <div className="bg-blue-500/10 border border-blue-500/20 px-4 py-2 rounded-xl">
                                        <p className="text-[10px] font-black uppercase text-muted-foreground tracking-widest">Rented</p>
                                        <p className="text-2xl font-black text-blue-600">{fleetData.statistics.totalRented}</p>
                                    </div>
                                    <div className="bg-amber-500/10 border border-amber-500/20 px-4 py-2 rounded-xl">
                                        <p className="text-[10px] font-black uppercase text-muted-foreground tracking-widest">Maintenance</p>
                                        <p className="text-2xl font-black text-amber-600">{fleetData.statistics.totalMaintenance}</p>
                                    </div>
                                    <div className="bg-primary/10 border border-primary/20 px-4 py-2 rounded-xl">
                                        <p className="text-[10px] font-black uppercase text-muted-foreground tracking-widest">Utilization</p>
                                        <p className="text-2xl font-black text-primary">{fleetData.statistics.utilizationRate}%</p>
                                    </div>
                                </div>
                            </div>
                            <div className="flex flex-col md:flex-row gap-4 mt-6">
                                <div className="flex gap-4 flex-1">
                                    <div className="flex-1">
                                        <Input
                                            type="date"
                                            value={dateRange.startDate}
                                            onChange={(e) => setDateRange(prev => ({ ...prev, startDate: e.target.value }))}
                                            className="h-12 bg-background border-none rounded-xl font-medium text-sm"
                                            placeholder="Start Date"
                                        />
                                    </div>
                                    <div className="flex-1">
                                        <Input
                                            type="date"
                                            value={dateRange.endDate}
                                            onChange={(e) => setDateRange(prev => ({ ...prev, endDate: e.target.value }))}
                                            className="h-12 bg-background border-none rounded-xl font-medium text-sm"
                                            placeholder="End Date"
                                        />
                                    </div>
                                    {(dateRange.startDate || dateRange.endDate) && (
                                        <Button
                                            variant="ghost"
                                            onClick={() => setDateRange({ startDate: '', endDate: '' })}
                                            className="h-12 px-4 hover:bg-destructive/10 hover:text-destructive"
                                        >
                                            Clear
                                        </Button>
                                    )}
                                </div>
                                <div className="flex-1">
                                    <Input
                                        type="text"
                                        placeholder="Search by model or registration..."
                                        value={searchTerm}
                                        onChange={(e) => setSearchTerm(e.target.value)}
                                        className="h-12 bg-background border-none rounded-xl font-medium text-sm"
                                    />
                                </div>
                                <select
                                    value={statusFilter}
                                    onChange={(e) => setStatusFilter(e.target.value)}
                                    className="flex h-12 w-full md:w-48 rounded-xl bg-background px-4 py-2 text-sm font-bold border-none focus-visible:ring-2 focus-visible:ring-primary"
                                >
                                    <option>All</option>
                                    <option>Available</option>
                                    <option>Rented</option>
                                    <option>Maintenance</option>
                                </select>
                            </div>
                        </CardHeader>
                        <CardContent className="p-0">
                            <div className="space-y-4 p-8">
                                {fleetData.hubs.map((hub) => {
                                    const filteredCars = getFilteredCars(hub.cars);
                                    if (filteredCars.length === 0) return null;

                                    return (
                                        <Card key={hub.hubId} className="border-border/50 overflow-hidden">
                                            <CardHeader
                                                className="p-6 bg-muted/30 cursor-pointer hover:bg-muted/50 transition-colors"
                                                onClick={() => toggleHub(hub.hubId)}
                                            >
                                                <div className="flex items-center justify-between">
                                                    <div className="flex items-center gap-4">
                                                        <div className="h-12 w-12 rounded-xl bg-primary/10 flex items-center justify-center text-primary">
                                                            <Car className="h-6 w-6" />
                                                        </div>
                                                        <div>
                                                            <h3 className="text-lg font-black uppercase">{hub.hubName}</h3>
                                                            <p className="text-xs text-muted-foreground font-medium">{hub.cityName || 'Unknown City'}</p>
                                                        </div>
                                                    </div>
                                                    <div className="flex items-center gap-6">
                                                        <div className="flex gap-3">
                                                            <Badge className="bg-emerald-500/10 text-emerald-600 border-emerald-500/20 font-black text-xs px-3 py-1">
                                                                {hub.availableCars} Available
                                                            </Badge>
                                                            <Badge className="bg-blue-500/10 text-blue-600 border-blue-500/20 font-black text-xs px-3 py-1">
                                                                {hub.rentedCars} Rented
                                                            </Badge>
                                                            <Badge className="bg-amber-500/10 text-amber-600 border-amber-500/20 font-black text-xs px-3 py-1">
                                                                {hub.maintenanceCars} Maintenance
                                                            </Badge>
                                                        </div>
                                                        <Button variant="ghost" size="icon" className="h-8 w-8">
                                                            {expandedHubs[hub.hubId] ? '▼' : '▶'}
                                                        </Button>
                                                    </div>
                                                </div>
                                            </CardHeader>
                                            {expandedHubs[hub.hubId] && (
                                                <CardContent className="p-0">
                                                    <div className="overflow-x-auto">
                                                        <table className="w-full text-left border-collapse">
                                                            <thead className="bg-muted/20">
                                                                <tr className="text-[10px] font-black uppercase tracking-widest text-muted-foreground border-b border-border/50">
                                                                    <th className="px-6 py-4">Vehicle</th>
                                                                    <th className="px-6 py-4">Type</th>
                                                                    <th className="px-6 py-4">Registration</th>
                                                                    <th className="px-6 py-4">Status</th>
                                                                    <th className="px-6 py-4">Daily Rate</th>
                                                                    <th className="px-6 py-4">Current Rental</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                {filteredCars.map((car) => (
                                                                    <tr key={car.carId} className="group hover:bg-muted/20 transition-colors border-b border-border/50">
                                                                        <td className="px-6 py-5">
                                                                            <p className="font-black text-sm">{car.model}</p>
                                                                        </td>
                                                                        <td className="px-6 py-5">
                                                                            <p className="text-xs text-muted-foreground font-medium">{car.carType || 'N/A'}</p>
                                                                        </td>
                                                                        <td className="px-6 py-5">
                                                                            <p className="font-mono text-xs font-bold">{car.registrationNumber}</p>
                                                                        </td>
                                                                        <td className="px-6 py-5">
                                                                            <Badge className={`${getStatusColor(car.status)} font-black text-xs px-3 py-1`}>
                                                                                {car.status}
                                                                            </Badge>
                                                                        </td>
                                                                        <td className="px-6 py-5">
                                                                            <p className="font-black text-sm">
                                                                                {car.dailyRate ? `₹${car.dailyRate.toLocaleString()}` : 'N/A'}
                                                                            </p>
                                                                        </td>
                                                                        <td className="px-6 py-5">
                                                                            {car.currentRental ? (
                                                                                <div className="text-xs">
                                                                                    <p className="font-bold">{car.currentRental.customerName}</p>
                                                                                    <p className="text-muted-foreground">
                                                                                        {new Date(car.currentRental.startDate).toLocaleDateString()} - {new Date(car.currentRental.endDate).toLocaleDateString()}
                                                                                    </p>
                                                                                </div>
                                                                            ) : (
                                                                                <p className="text-xs text-muted-foreground italic">—</p>
                                                                            )}
                                                                        </td>
                                                                    </tr>
                                                                ))}
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                </CardContent>
                                            )}
                                        </Card>
                                    );
                                })}
                            </div>
                        </CardContent>
                    </Card>
                )}
            </div>
        </div>
    );
};

export default AdminVehicles;
