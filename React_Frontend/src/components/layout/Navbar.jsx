import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import AuthService from '../../services/authService';
import { Button } from "@/components/ui/button";
import {
    Sheet,
    SheetContent,
    SheetTrigger,
} from "@/components/ui/sheet";
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
    DropdownMenuSeparator,
    DropdownMenuLabel,
} from "@/components/ui/dropdown-menu";
import { Menu, Moon, Sun, Shield, LogOut, User as UserIcon } from "lucide-react";

const Navbar = ({ theme, toggleTheme }) => {
    const navigate = useNavigate();
    const user = AuthService.getCurrentUser();
    const role = user?.role;
    const [isOpen, setIsOpen] = useState(false);
    const [scrolled, setScrolled] = useState(false);

    React.useEffect(() => {
        const handleScroll = () => setScrolled(window.scrollY > 20);
        window.addEventListener('scroll', handleScroll);
        return () => window.removeEventListener('scroll', handleScroll);
    }, []);

    const handleLogout = () => {
        AuthService.logout();
        navigate('/login');
    };

    const NavItems = ({ mobile = false }) => (
        <>
            {[
                { label: 'Home', path: '/', condition: !['ADMIN', 'STAFF'].includes(role) },
                { label: 'About Us', path: '/about', condition: !['ADMIN', 'STAFF'].includes(role) },
                { label: 'Book a Car', path: '/booking', condition: !user || role === 'CUSTOMER' },
                { label: 'My Bookings', path: '/my-bookings', condition: role === 'CUSTOMER' },
                { label: 'Support', path: '/customer-care', condition: !['ADMIN', 'STAFF'].includes(role) }
            ].map((item, idx) => (
                (item.condition === undefined || item.condition) && (
                    <Link
                        key={idx}
                        to={item.path}
                        className={`text-sm font-medium transition-all hover:text-primary relative group ${mobile ? 'text-lg py-2' : ''}`}
                        onClick={() => setIsOpen(false)}
                    >
                        {item.label}
                        {!mobile && <span className="absolute -bottom-1 left-0 w-0 h-0.5 bg-primary transition-all group-hover:w-full" />}
                    </Link>
                )
            ))}
        </>
    );

    return (
        <header className={`sticky top-0 z-50 w-full transition-all duration-300 ${scrolled ? 'glass shadow-lg py-2' : 'bg-transparent py-4'}`}>
            <div className={`container mx-auto px-4 flex items-center ${['ADMIN', 'STAFF'].includes(role) ? 'justify-between' : 'justify-between'}`}>
                <Link className="flex items-center gap-2 font-bold text-2xl group" to={role === 'ADMIN' ? '/admin/dashboard' : role === 'STAFF' ? '/staff/dashboard' : '/'}>
                    <div className="relative">
                        <div className="absolute inset-0 bg-primary/20 rounded-full blur-xl group-hover:bg-primary/40 transition-all duration-500" />
                        <Shield className="h-8 w-8 text-primary relative z-10 transition-transform duration-500 group-hover:rotate-12" />
                    </div>
                    <span className="bg-gradient-to-r from-amber-400 to-yellow-600 dark:from-amber-300 dark:to-yellow-500 bg-clip-text text-transparent font-extrabold tracking-tight">
                        IndiaDrive
                    </span>
                </Link>

                {/* Desktop Navigation */}
                <nav className="hidden md:flex items-center gap-8">
                    <NavItems />
                </nav>

                <div className="flex items-center gap-3">
                    <Button
                        variant="ghost"
                        size="icon"
                        onClick={toggleTheme}
                        className="rounded-full hover:bg-amber-500/10 hover:text-amber-600 transition-colors"
                        title="Toggle Theme"
                    >
                        {/* User requested: Change to Sun when choosing bright theme. 
                            Interpreted as: Icon represents CURRENT state. 
                            Light Theme -> Sun Icon. Dark Theme -> Moon Icon. */}
                        {theme === 'dark' ? <Moon className="h-5 w-5" /> : <Sun className="h-5 w-5 text-amber-500 fill-amber-500" />}
                    </Button>

                    {user ? (
                        <DropdownMenu>
                            <DropdownMenuTrigger asChild>
                                <Button variant="ghost" className="gap-2 px-2 sm:px-4 rounded-full hover:bg-primary/10">
                                    <div className="h-9 w-9 rounded-full bg-gradient-to-br from-primary to-yellow-600 flex items-center justify-center text-primary-foreground shadow-md ring-2 ring-background">
                                        <UserIcon className="h-4 w-4" />
                                    </div>
                                    <span className="hidden sm:inline-block font-medium">{user.username}</span>
                                </Button>
                            </DropdownMenuTrigger>
                            <DropdownMenuContent align="end" className="w-56 glass-card p-2 animate-in slide-in-from-top-2">
                                <DropdownMenuLabel>My Account</DropdownMenuLabel>
                                <DropdownMenuSeparator />
                                <DropdownMenuItem className="cursor-pointer rounded-lg focus:bg-primary/10" onClick={() => navigate(role === 'ADMIN' ? '/admin/dashboard' : role === 'STAFF' ? '/staff/dashboard' : '/my-bookings')}>
                                    Dashboard
                                </DropdownMenuItem>
                                <DropdownMenuItem className="text-destructive focus:text-destructive cursor-pointer rounded-lg focus:bg-destructive/10" onClick={handleLogout}>
                                    <LogOut className="mr-2 h-4 w-4" />
                                    Logout
                                </DropdownMenuItem>
                            </DropdownMenuContent>
                        </DropdownMenu>
                    ) : (
                        <div className="hidden md:flex items-center gap-3">
                            <Link to="/login">
                                <Button variant="ghost" className="hover:bg-primary/10 rounded-full font-medium">Login</Button>
                            </Link>
                            <Link to="/register">
                                <Button className="rounded-full px-6 bg-gradient-to-r from-amber-400 to-yellow-600 hover:from-amber-500 hover:to-yellow-700 text-black font-bold shadow-lg hover:shadow-primary/25 transition-all">
                                    Join Now
                                </Button>
                            </Link>
                        </div>
                    )}

                    {/* Mobile Menu */}
                    <div className="md:hidden">
                        <Sheet open={isOpen} onOpenChange={setIsOpen}>
                            <SheetTrigger asChild>
                                <Button variant="ghost" size="icon" className="rounded-full">
                                    <Menu className="h-6 w-6" />
                                </Button>
                            </SheetTrigger>
                            <SheetContent side="right" className="glass-card w-full sm:w-80 border-l border-white/20">
                                <div className="flex flex-col gap-6 mt-10">
                                    <NavItems mobile />
                                    {!user && (
                                        <div className="flex flex-col gap-3 mt-6">
                                            <Link to="/login" onClick={() => setIsOpen(false)}>
                                                <Button variant="outline" className="w-full rounded-full h-12 border-primary/20">Login</Button>
                                            </Link>
                                            <Link to="/register" onClick={() => setIsOpen(false)}>
                                                <Button className="w-full rounded-full h-12 bg-gradient-to-r from-blue-600 to-indigo-600 shadow-lg">Join Now</Button>
                                            </Link>
                                        </div>
                                    )}
                                </div>
                            </SheetContent>
                        </Sheet>
                    </div>
                </div>
            </div>
        </header>
    );
};

export default Navbar;
