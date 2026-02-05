import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Languages, ShieldCheck, Tag, Headset, ArrowRight, Users, CheckCircle2, MapPin } from "lucide-react";
import { motion, useScroll } from 'framer-motion';



const Home = () => {
    const { t, i18n } = useTranslation();
    const { scrollYProgress } = useScroll();

    // Parallax logic can remain or be simplified
    // const carX = useTransform(scrollYProgress, [0, 0.5], [-200, 1000]); 

    const changeLanguage = (lng) => {
        i18n.changeLanguage(lng);
    };

    const getLanguageLabel = () => {
        if (i18n.language?.startsWith('en')) return 'English';
        if (i18n.language?.startsWith('mr')) return 'मराठी';
        if (i18n.language?.startsWith('fr')) return 'Français';
        return 'Language';
    };

    return (
        <div className="min-h-screen bg-background selection:bg-primary/30 overflow-x-hidden relative">
            {/* Language Switcher */}
            <div className="absolute top-6 right-4 z-50">
                <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                        <Button variant="outline" size="sm" className="rounded-full gap-2 glass border-primary/20 hover:border-primary/50 transition-all duration-300">
                            <Languages className="h-4 w-4" />
                            {getLanguageLabel()}
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end" className="glass-card w-48 animate-in slide-in-from-top-2">
                        <DropdownMenuItem onClick={() => changeLanguage('en')}>English</DropdownMenuItem>
                        <DropdownMenuItem onClick={() => changeLanguage('mr')}>मराठी (Marathi)</DropdownMenuItem>
                        <DropdownMenuItem onClick={() => changeLanguage('fr')}>Français (French)</DropdownMenuItem>
                    </DropdownMenuContent>
                </DropdownMenu>
            </div>

            {/* Radical Hero Redesign */}
            <header className="relative min-h-[90vh] flex items-center justify-center overflow-hidden pt-20 pb-10">
                {/* Dynamic Background */}
                <div className="absolute inset-0 -z-20">
                    <div className="absolute inset-0 bg-[url('https://images.unsplash.com/photo-1617788138017-80ad40651399?auto=format&fit=crop&q=80')] bg-cover bg-center" />
                    <div className="absolute inset-0 bg-gradient-to-r from-background via-background/95 to-background/40" />
                </div>

                <div className="container mx-auto px-4 relative z-10 text-center">
                    {/* Centered Content */}
                    <motion.div
                        initial={{ opacity: 0, y: 30 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ duration: 0.8 }}
                        className="max-w-4xl mx-auto space-y-8"
                    >
                        <motion.div
                            initial={{ width: 0 }}
                            animate={{ width: "100px" }}
                            transition={{ duration: 1, delay: 0.5 }}
                            className="h-1 bg-primary mb-6 mx-auto"
                        />
                        <h1 className="text-6xl md:text-8xl font-black tracking-tighter leading-[0.9]">
                            <span className="text-foreground">{t('home.sloganLine1')}</span>
                            <br />
                            <span className="text-transparent bg-clip-text bg-gradient-to-r from-amber-400 via-yellow-500 to-amber-600">
                                {t('home.sloganLine2')}
                            </span>
                        </h1>
                        <p className="text-xl md:text-2xl text-muted-foreground font-light max-w-2xl mx-auto leading-relaxed">
                            {t('home.sloganSub')}
                        </p>

                        <div className="flex flex-wrap justify-center gap-4 pt-4">
                            <Link to="/booking">
                                <Button size="lg" className="rounded-none h-14 px-8 text-lg bg-primary hover:bg-primary/90 text-black font-bold tracking-wide">
                                    BOOK NOW
                                </Button>
                            </Link>
                            <Link to="/explore-vehicles">
                                <Button variant="outline" size="lg" className="rounded-none h-14 px-8 text-lg border-2 border-primary/20 hover:bg-primary/5 hover:border-primary transition-colors">
                                    EXPLORE VEHICLES
                                </Button>
                            </Link>
                        </div>

                        {/* Trust Badges Minimal */}
                        <div className="flex justify-center gap-8 pt-8 opacity-60">
                            <div className="flex items-center gap-2">
                                <Users className="w-5 h-5" />
                                <span className="font-semibold tracking-wider text-sm">{t('home.statsMembers')}</span>
                            </div>
                            <div className="flex items-center gap-2">
                                <CheckCircle2 className="w-5 h-5" />
                                <span className="font-semibold tracking-wider text-sm">{t('home.statsDealers')}</span>
                            </div>
                        </div>
                    </motion.div>
                </div>
            </header>

            {/* Bento Grid Features Section */}
            <section className="py-32 container mx-auto px-4">
                <div className="text-center mb-20 space-y-4">
                    <span className="text-primary font-semibold tracking-wider uppercase text-sm">{t('home.whyChoose')}</span>
                    <h2 className="text-4xl md:text-5xl font-black tracking-tight">
                        {t('home.whyChooseTitle')} <span className="text-primary">{t('home.whyChooseHighlight')}</span>
                    </h2>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-4 md:grid-rows-2 gap-6 h-auto md:h-[600px]">
                    {/* Large Card - Safety */}
                    <motion.div
                        whileHover={{ scale: 0.98 }}
                        className="md:col-span-2 md:row-span-2 glass-card rounded-[2rem] p-8 md:p-12 relative overflow-hidden group border-white/10 flex flex-col justify-between"
                    >
                        <div className="absolute top-0 right-0 w-64 h-64 bg-emerald-500/10 rounded-full blur-[80px] -mr-16 -mt-16" />
                        <div>
                            <ShieldCheck className="w-16 h-16 text-emerald-500 mb-6" />
                            <h3 className="text-3xl font-bold mb-4">{t('home.safetyTitle')}</h3>
                            <p className="text-muted-foreground text-lg leading-relaxed max-w-sm">{t('home.safetyDesc')}</p>
                        </div>
                        <div className="mt-8 flex gap-2">
                            <div className="px-3 py-1 rounded-full bg-emerald-500/10 text-emerald-500 text-xs font-bold">{t('home.safetyTag1')}</div>
                            <div className="px-3 py-1 rounded-full bg-emerald-500/10 text-emerald-500 text-xs font-bold">{t('home.safetyTag2')}</div>
                        </div>
                        <ShieldCheck className="absolute -bottom-10 -right-10 w-64 h-64 text-emerald-500/5 group-hover:scale-110 transition-transform duration-700" />
                    </motion.div>

                    {/* Wide Card - Price */}
                    <motion.div
                        whileHover={{ scale: 0.98 }}
                        className="md:col-span-2 glass-card rounded-[2rem] p-8 md:p-10 relative overflow-hidden group border-white/10 flex flex-col justify-center"
                    >
                        <div className="absolute top-0 left-0 w-full h-full bg-gradient-to-r from-blue-500/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity" />
                        <div className="flex items-start justify-between relative z-10">
                            <div>
                                <Tag className="w-10 h-10 text-blue-500 mb-4" />
                                <h3 className="text-2xl font-bold">{t('home.priceTitle')}</h3>
                                <p className="text-muted-foreground mt-2">{t('home.priceDesc')}</p>
                            </div>
                            <div className="text-4xl font-black text-white/5 group-hover:text-blue-500/20 transition-colors">$$$</div>
                        </div>
                    </motion.div>

                    {/* Small Card - Support */}
                    <motion.div
                        whileHover={{ scale: 0.98 }}
                        className="md:col-span-1 glass-card rounded-[2rem] p-8 relative overflow-hidden group border-white/10 flex flex-col justify-center bg-gradient-to-br from-primary/5 to-transparent"
                    >
                        <Headset className="w-10 h-10 text-primary mb-4" />
                        <h3 className="text-xl font-bold">{t('home.supportTitle')}</h3>
                        <p className="text-muted-foreground text-sm mt-2">{t('home.supportDesc')}</p>
                    </motion.div>

                    {/* Small Card - Tech */}
                    <motion.div
                        whileHover={{ scale: 0.98 }}
                        className="md:col-span-1 glass-card rounded-[2rem] p-8 relative overflow-hidden group border-white/10 flex flex-col justify-center"
                    >
                        <MapPin className="w-10 h-10 text-indigo-500 mb-4" />
                        <h3 className="text-xl font-bold">{t('home.techTitle')}</h3>
                        <p className="text-muted-foreground text-sm mt-2">{t('home.techDesc')}</p>
                    </motion.div>
                </div>
            </section>

            {/* Stats Section with Parallax Background */}
            <section className="py-24 relative overflow-hidden">
                <div className="absolute inset-0 bg-black/90">
                    <div className="absolute inset-0 bg-[url('https://images.unsplash.com/photo-1492144534655-ae79c964c9d7?auto=format&fit=crop&w=1920&q=80')] bg-cover bg-center opacity-20 fixed-background" />
                    <div className="absolute inset-0 bg-gradient-to-t from-background via-transparent to-background" />
                </div>

                <div className="container mx-auto px-4 relative z-10">
                    <div className="grid grid-cols-2 lg:grid-cols-4 gap-8">
                        {[
                            { label: 'Vehicles', value: '1,200+' },
                            { label: 'Cities', value: '24' },
                            { label: 'Happy Clients', value: '50k+' },
                            { label: 'Support', value: '24/7' }
                        ].map((stat, idx) => (
                            <motion.div
                                key={idx}
                                initial={{ opacity: 0, scale: 0.5 }}
                                whileInView={{ opacity: 1, scale: 1 }}
                                viewport={{ once: true }}
                                transition={{ type: "spring", stiffness: 200, delay: idx * 0.1 }}
                                className="glass p-8 rounded-3xl text-center group border border-primary/10 hover:border-primary/50 transition-all duration-500 hover:bg-black/40"
                            >
                                <div className="text-4xl lg:text-5xl font-extrabold text-primary mb-2 group-hover:scale-110 transition-transform drop-shadow-[0_0_15px_rgba(234,179,8,0.3)]">{stat.value}</div>
                                <div className="text-muted-foreground font-medium uppercase tracking-wider text-sm group-hover:text-white transition-colors">{stat.label}</div>
                            </motion.div>
                        ))}
                    </div>
                </div>
            </section>
        </div>
    );
};

export default Home;
