import React from 'react';
import { Calendar, Car, MapPin, Mail, Phone, User, Building2, IndianRupee } from 'lucide-react';
import './invoice.css';

const ProfessionalInvoice = ({ booking }) => {
    if (!booking) return null;

    // Calculate rental duration
    const start = new Date(booking.startDate);
    const end = new Date(booking.endDate);
    const days = Math.max(1, Math.ceil(Math.abs(end - start) / (1000 * 60 * 60 * 24)));

    // Calculate costs
    const baseRentalTotal = (booking.dailyRate || 0) * days;
    const addonTotal = booking.totalAddonAmount || 0;
    const grandTotal = booking.totalAmount || (baseRentalTotal + addonTotal);

    // Format date for invoice
    const invoiceDate = new Date().toLocaleDateString('en-IN', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
    });

    // Format booking dates
    const formatDate = (dateString) => {
        if (!dateString) return 'N/A';
        return new Date(dateString).toLocaleDateString('en-IN', {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        });
    };

    return (
        <div className="professional-invoice">
            {/* Company Header */}
            <div className="invoice-header">
                <div className="company-info">
                    <div className="company-logo">
                        <Building2 className="logo-icon" />
                    </div>
                    <div>
                        <h1 className="company-name">IndiaDrive</h1>
                        <p className="company-tagline">Premium Fleet Management</p>
                    </div>
                </div>
                <div className="invoice-meta">
                    <h2 className="invoice-title">INVOICE</h2>
                    <p className="invoice-number">#{booking.bookingId}</p>
                    <p className="invoice-date">{invoiceDate}</p>
                </div>
            </div>

            <div className="invoice-divider"></div>

            {/* Customer & Booking Info Grid */}
            <div className="info-grid">
                {/* Customer Information */}
                <div className="info-section">
                    <h3 className="section-title">
                        <User className="section-icon" />
                        Customer Information
                    </h3>
                    <div className="info-content">
                        <div className="info-row">
                            <span className="info-label">Name:</span>
                            <span className="info-value">{booking.customerName || `${booking.firstName || ''} ${booking.lastName || ''}`.trim() || 'N/A'}</span>
                        </div>
                        <div className="info-row">
                            <span className="info-label">Email:</span>
                            <span className="info-value">{booking.email || booking.emailId || 'N/A'}</span>
                        </div>
                        <div className="info-row">
                            <span className="info-label">Address:</span>
                            <span className="info-value">{booking.address || 'N/A'}</span>
                        </div>
                        {booking.state && (
                            <div className="info-row">
                                <span className="info-label">State:</span>
                                <span className="info-value">{booking.state}</span>
                            </div>
                        )}
                        {booking.pin && (
                            <div className="info-row">
                                <span className="info-label">PIN Code:</span>
                                <span className="info-value">{booking.pin}</span>
                            </div>
                        )}
                    </div>
                </div>

                {/* Booking Information */}
                <div className="info-section">
                    <h3 className="section-title">
                        <Calendar className="section-icon" />
                        Booking Details
                    </h3>
                    <div className="info-content">
                        <div className="info-row">
                            <span className="info-label">Booking ID:</span>
                            <span className="info-value">{booking.bookingId}</span>
                        </div>
                        {booking.confirmationNumber && (
                            <div className="info-row">
                                <span className="info-label">Confirmation:</span>
                                <span className="info-value">{booking.confirmationNumber}</span>
                            </div>
                        )}
                        <div className="info-row">
                            <span className="info-label">Status:</span>
                            <span className={`status-badge status-${(booking.bookingStatus || 'confirmed').toLowerCase()}`}>
                                {booking.bookingStatus || 'CONFIRMED'}
                            </span>
                        </div>
                        <div className="info-row">
                            <span className="info-label">Pickup Date:</span>
                            <span className="info-value">{formatDate(booking.startDate)}</span>
                        </div>
                        <div className="info-row">
                            <span className="info-label">Return Date:</span>
                            <span className="info-value">{formatDate(booking.endDate)}</span>
                        </div>
                        <div className="info-row">
                            <span className="info-label">Duration:</span>
                            <span className="info-value font-bold">{days} Day{days > 1 ? 's' : ''}</span>
                        </div>
                    </div>
                </div>
            </div>

            {/* Vehicle Information */}
            <div className="vehicle-section">
                <h3 className="section-title">
                    <Car className="section-icon" />
                    Vehicle Details
                </h3>
                <div className="vehicle-card">
                    <div className="vehicle-info">
                        <h4 className="vehicle-name">{booking.carName || 'Premium Vehicle'}</h4>
                        {booking.numberPlate && (
                            <p className="vehicle-plate">Registration: {booking.numberPlate}</p>
                        )}
                        <div className="vehicle-specs">
                            {booking.fuelType && <span className="spec-badge">{booking.fuelType}</span>}
                            {booking.seatingCapacity && <span className="spec-badge">{booking.seatingCapacity} Seats</span>}
                            <span className="spec-badge">Automatic</span>
                        </div>
                    </div>
                    <div className="location-info">
                        <div className="location-item">
                            <MapPin className="location-icon" />
                            <div>
                                <p className="location-label">Pickup Location</p>
                                <p className="location-value">{booking.pickupHub || booking.pickupHubId || 'Main Hub'}</p>
                            </div>
                        </div>
                        {booking.returnHub && (
                            <div className="location-item">
                                <MapPin className="location-icon" />
                                <div>
                                    <p className="location-label">Return Location</p>
                                    <p className="location-value">{booking.returnHub}</p>
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </div>

            {/* Cost Breakdown */}
            <div className="cost-section">
                <h3 className="section-title">
                    <IndianRupee className="section-icon" />
                    Cost Breakdown
                </h3>
                <table className="cost-table">
                    <thead>
                        <tr>
                            <th>Description</th>
                            <th>Rate</th>
                            <th>Quantity</th>
                            <th className="text-right">Amount</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                <strong>Vehicle Rental</strong>
                                <br />
                                <span className="item-description">{booking.carName || 'Premium Vehicle'}</span>
                            </td>
                            <td>₹{(booking.dailyRate || 0).toLocaleString('en-IN')}/day</td>
                            <td>{days} day{days > 1 ? 's' : ''}</td>
                            <td className="text-right">₹{baseRentalTotal.toLocaleString('en-IN')}</td>
                        </tr>
                        {/* Add-on Services - Use detailed data if available */}
                        {booking.addOnDetails && booking.addOnDetails.length > 0 ? (
                            <>
                                {booking.addOnDetails.map((addon, idx) => {
                                    const totalForAddon = addon.dailyRate * addon.quantity * days;
                                    const displayName = addon.quantity > 1
                                        ? `${addon.addOnName} (x${addon.quantity})`
                                        : addon.addOnName;
                                    const effectiveDailyRate = addon.dailyRate * addon.quantity;

                                    return (
                                        <tr key={idx}>
                                            <td>
                                                <strong>Add-on Service</strong>
                                                <br />
                                                <span className="item-description">{displayName}</span>
                                            </td>
                                            <td>₹{effectiveDailyRate.toLocaleString('en-IN')}/day</td>
                                            <td>{days} day{days > 1 ? 's' : ''}</td>
                                            <td className="text-right">₹{totalForAddon.toLocaleString('en-IN')}</td>
                                        </tr>
                                    );
                                })}
                            </>
                        ) : (
                            <>
                                {/* Fallback for old data without addOnDetails */}
                                {booking.selectedAddOns && booking.selectedAddOns.length > 0 && (
                                    <>
                                        {booking.selectedAddOns.map((addonName, idx) => {
                                            const addonDailyRate = (addonTotal / booking.selectedAddOns.length) / days;
                                            const addonTotal_item = addonDailyRate * days;

                                            return (
                                                <tr key={idx}>
                                                    <td>
                                                        <strong>Add-on Service</strong>
                                                        <br />
                                                        <span className="item-description">{addonName}</span>
                                                    </td>
                                                    <td>₹{addonDailyRate.toFixed(0)}/day</td>
                                                    <td>{days} day{days > 1 ? 's' : ''}</td>
                                                    <td className="text-right">₹{addonTotal_item.toLocaleString('en-IN')}</td>
                                                </tr>
                                            );
                                        })}
                                    </>
                                )}
                                {addonTotal > 0 && (!booking.selectedAddOns || booking.selectedAddOns.length === 0) && (
                                    <tr>
                                        <td>
                                            <strong>Add-on Services</strong>
                                            <br />
                                            <span className="item-description">Various add-ons selected</span>
                                        </td>
                                        <td>-</td>
                                        <td>{days} day{days > 1 ? 's' : ''}</td>
                                        <td className="text-right">₹{addonTotal.toLocaleString('en-IN')}</td>
                                    </tr>
                                )}
                            </>
                        )}
                    </tbody>
                    <tfoot>
                        <tr className="subtotal-row">
                            <td colSpan="3"><strong>Subtotal</strong></td>
                            <td className="text-right">₹{(baseRentalTotal + addonTotal).toLocaleString('en-IN')}</td>
                        </tr>
                        <tr className="total-row">
                            <td colSpan="3"><strong>TOTAL AMOUNT</strong></td>
                            <td className="text-right"><strong>₹{grandTotal.toLocaleString('en-IN')}</strong></td>
                        </tr>
                    </tfoot>
                </table>
            </div>

            {/* Terms & Conditions */}
            <div className="terms-section">
                <h3 className="section-title">Terms & Conditions</h3>
                <div className="terms-content">
                    <ul className="terms-list">
                        <li>The vehicle must be returned with the same fuel level as at pickup.</li>
                        <li>Any damage to the vehicle during the rental period is the responsibility of the renter.</li>
                        <li>Late returns may incur additional charges at the daily rate.</li>
                        <li>The renter must possess a valid driving license throughout the rental period.</li>
                        <li>Insurance coverage is included as per the rental agreement.</li>
                        <li>Cancellation charges apply as per company policy.</li>
                    </ul>
                </div>
            </div>

            {/* Footer */}
            <div className="invoice-footer">
                <div className="footer-content">
                    <div className="footer-section">
                        <h4>Contact Us</h4>
                        <p><Mail className="footer-icon" /> support@indiadrive.com</p>
                        <p><Phone className="footer-icon" /> +91 123 456 7890</p>
                    </div>
                    <div className="footer-section">
                        <h4>Office Address</h4>
                        <p>IndiaDrive Premium Fleet Management</p>
                        <p>Main Terminal, IndiaDrive Plaza</p>
                        <p>Mumbai, Maharashtra 400001</p>
                    </div>
                </div>
                <div className="thank-you">
                    <p>Thank you for choosing IndiaDrive!</p>
                    <p className="footer-note">This is a computer-generated invoice and does not require a signature.</p>
                </div>
            </div>
        </div>
    );
};

export default ProfessionalInvoice;
