import { clsx } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs) {
    return twMerge(clsx(inputs))
}

export function formatDateForInput(dateString) {
    if (!dateString) return '';
    // Extract yyyy-MM-dd from ISO or other date strings
    return dateString.split('T')[0];
}
