import React, { useState, useEffect } from 'react';
import { useLocation, useNavigate, Link } from 'react-router-dom';
import ApiService from '../../services/api';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
    Car,
    ArrowLeft,
    Search,
    Loader2,
    Users,
    Settings,
    Fuel,
    Briefcase,
    Star,
    MapPin,
    ArrowRight,
    Sparkles,
    IndianRupee
} from "lucide-react";

const CarSelection = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const [carTypes, setCarTypes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const { pickupHub, pickupDateTime, returnDateTime } = location.state || {};

    useEffect(() => {
        if (!location.state || !pickupHub) {
            navigate('/booking');
            return;
        }

        const fetchCarTypes = async () => {
            try {
                const data = await ApiService.getCarTypes();
                setCarTypes(data);
            } catch (err) {
                setError('Failed to load fleet catalog. Please try again.');
                console.error(err);
            } finally {
                setLoading(false);
            }
        };

        fetchCarTypes();
    }, [location.state, navigate, pickupHub]);

    const handleCarSelect = (type) => {
        // Construct a mock car object from the type since specific availability is bypassed
        const mockCar = {
            carId: 0, // Placeholder, backend needs to handle auto-assignment or this is just a catalogue request
            carModel: type.carTypeName,
            carType: type,
            hubId: pickupHub.hubId
        };

        navigate('/booking', {
            state: {
                selectedCar: mockCar,
                pickupHub,
                startDate: pickupDateTime,
                endDate: returnDateTime
            }
        });
    };

    if (loading) return (
        <div className="flex flex-col items-center justify-center min-h-[70vh] gap-6">
            <div className="relative">
                <div className="h-24 w-24 rounded-full border-4 border-primary/20 border-t-primary animate-spin"></div>
                <div className="absolute inset-0 flex items-center justify-center">
                    <Car className="h-8 w-8 text-primary animate-pulse" />
                </div>
            </div>
            <div className="text-center">
                <h3 className="text-2xl font-black tracking-tight mb-2">Loading Fleet Catalog</h3>
                <p className="text-muted-foreground animate-pulse">Retrieving premium vehicle rates...</p>
            </div>
        </div>
    );

    return (
        <div className="min-h-screen bg-background py-16">
            <div className="container mx-auto px-4">
                <div className="flex flex-col lg:flex-row justify-between items-start lg:items-end mb-12 gap-8">
                    <div className="animate-in fade-in slide-in-from-left-4 duration-700">
                        <Link to="/select-hub" state={location.state}>
                            <Button variant="ghost" className="mb-4 gap-2 text-muted-foreground hover:text-foreground">
                                <ArrowLeft className="h-4 w-4" /> Change Location
                            </Button>
                        </Link>
                        <h1 className="text-4xl md:text-5xl font-black tracking-tight mb-2 uppercase">
                            Premium <span className="text-primary italic">Selection</span>
                        </h1>
                        <div className="flex items-center gap-2 text-muted-foreground">
                            <MapPin className="h-4 w-4 text-primary" />
                            <span className="font-bold text-foreground">{pickupHub?.hubName}</span>
                            <span className="text-xs uppercase tracking-widest hidden sm:inline">• Full Catalog View</span>
                        </div>
                    </div>
                </div>

                {error && (
                    <div className="max-w-md mx-auto bg-destructive/10 text-destructive p-4 rounded-2xl mb-12 text-center flex items-center justify-center gap-2">
                        <Search className="h-5 w-5" /> {error}
                    </div>
                )}

                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 max-w-7xl mx-auto">
                    {carTypes.length > 0 ? (
                        carTypes.map((type, index) => (
                            <Card key={type.carTypeId} className="group border-none shadow-xl hover:shadow-2xl transition-all duration-500 bg-card overflow-hidden hover:-translate-y-2 flex flex-col">
                                <CardContent className="p-0 flex flex-col h-full">
                                    <div className="relative aspect-[16/10] overflow-hidden">
                                        <img
                                            src={type.imagePath ? (type.imagePath.startsWith('/') ? type.imagePath : `/${type.imagePath}`) : 'https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?auto=format&fit=crop&w=800&q=80'}
                                            alt={type.carTypeName}
                                            className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-110"
                                        />
                                        <div className="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500"></div>
                                        <div className="absolute top-4 right-4 animate-in fade-in zoom-in duration-500">
                                            <Badge className="h-10 px-4 bg-primary text-primary-foreground text-sm font-black rounded-xl shadow-lg shadow-primary/30 flex items-center gap-1">
                                                <IndianRupee className="h-3 w-3" /> {type.dailyRate}<span className="text-[10px] font-normal opacity-70">/DAY</span>
                                            </Badge>
                                        </div>
                                    </div>

                                    <div className="p-8 flex flex-col flex-grow">
                                        <div className="flex justify-between items-start mb-6">
                                            <div>
                                                <h3 className="text-2xl font-black mb-1 tracking-tight group-hover:text-primary transition-colors">{type.carTypeName}</h3>
                                                <div className="flex items-center gap-2">
                                                    <span className="text-xs font-bold uppercase tracking-widest text-muted-foreground">Standard Series</span>
                                                    <div className="flex gap-0.5 text-amber-500">
                                                        <Star className="h-3 w-3 fill-current" />
                                                        <Star className="h-3 w-3 fill-current" />
                                                        <Star className="h-3 w-3 fill-current" />
                                                        <Star className="h-3 w-3 fill-current" />
                                                        <Star className="h-3 w-3 fill-current" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        {/* Rate Card Grid */}
                                        <div className="grid grid-cols-3 gap-2 mb-8 bg-muted/20 p-4 rounded-xl border border-border/50">
                                            <div className="text-center border-r border-border/50">
                                                <p className="text-[10px] uppercase font-bold text-muted-foreground mb-1">Daily</p>
                                                <p className="font-black text-sm">₹{type.dailyRate}</p>
                                            </div>
                                            <div className="text-center border-r border-border/50">
                                                <p className="text-[10px] uppercase font-bold text-muted-foreground mb-1">Weekly</p>
                                                <p className="font-black text-sm">₹{type.weeklyRate}</p>
                                            </div>
                                            <div className="text-center">
                                                <p className="text-[10px] uppercase font-bold text-muted-foreground mb-1">Monthly</p>
                                                <p className="font-black text-sm">₹{type.monthlyRate}</p>
                                            </div>
                                        </div>

                                        <div className="grid grid-cols-2 gap-3 mb-8">
                                            {[
                                                { icon: Users, label: '5 Seats' },
                                                { icon: Settings, label: type.carTypeName?.includes('Automatic') ? 'Auto' : 'Manual' },
                                                { icon: Fuel, label: 'Petrol' },
                                                { icon: Briefcase, label: '2 Large' }
                                            ].map((spec, i) => (
                                                <div key={i} className="flex items-center gap-2 text-xs font-bold text-muted-foreground bg-muted/30 px-3 py-2 rounded-xl border border-border/50">
                                                    <spec.icon className="h-3.5 w-3.5 text-primary" />
                                                    {spec.label}
                                                </div>
                                            ))}
                                        </div>

                                        <div className="mt-auto">
                                            <Button
                                                className="w-full h-14 rounded-2xl font-black text-lg bg-primary hover:shadow-xl hover:shadow-primary/30 transition-all group/btn"
                                                onClick={() => handleCarSelect(type)}
                                            >
                                                Select Category
                                                <ArrowRight className="ml-2 h-5 w-5 transition-transform group-hover/btn:translate-x-1" />
                                            </Button>
                                        </div>
                                    </div>
                                </CardContent>
                            </Card>
                        ))
                    ) : (
                        <div className="col-span-full py-20 text-center">
                            <div className="bg-muted/30 rounded-[4rem] p-16 border border-dashed border-border max-w-2xl mx-auto">
                                <div className="h-24 w-24 bg-muted rounded-full flex items-center justify-center mx-auto mb-8">
                                    <Car className="h-10 w-10 text-muted-foreground/30" />
                                </div>
                                <h3 className="text-3xl font-black mb-4 tracking-tight">Catalog Empty</h3>
                                <p className="text-muted-foreground text-lg mb-10 leading-relaxed">
                                    No vehicle categories found in the system.
                                </p>
                            </div>
                        </div>
                    )}
                </div>

                <div className="mt-20 py-10 border-t border-border/50 text-center animate-in fade-in duration-1000 delay-500">
                    <p className="text-sm text-muted-foreground uppercase tracking-[0.2em] font-black flex items-center justify-center gap-3">
                        <Sparkles className="h-4 w-4 text-primary" />
                        All rentals include 24/7 Roadside Assistance & Premium Insurance
                    </p>
                </div>
            </div>
        </div>
    );
};

export default CarSelection;
