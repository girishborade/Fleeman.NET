using System;
using System.Collections.Generic;
using System.Linq;
using FleetManagementSystem.Api.Data;
using FleetManagementSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetManagementSystem.Api.Services;

public class AddOnService : IAddOnService
{
    private readonly ApplicationDbContext _context;
    public AddOnService(ApplicationDbContext context) => _context = context;
    public List<AddOnMaster> GetAllAddOns() => _context.AddOns.ToList();
    public AddOnMaster AddAddOn(AddOnMaster addOn)
    {
        _context.AddOns.Add(addOn);
        _context.SaveChanges();
        return addOn;
    }
    public AddOnMaster GetAddOnById(int id) => _context.AddOns.Find(id);
    public void DeleteAddOn(int id)
    {
        var addon = _context.AddOns.Find(id);
        if (addon != null) { _context.AddOns.Remove(addon); _context.SaveChanges(); }
    }
}

public class VendorService : IVendorService
{
    private readonly ApplicationDbContext _context;
    public VendorService(ApplicationDbContext context) => _context = context;
    public List<Vendor> GetAllVendors() => _context.Vendors.ToList();
    public Vendor AddVendor(Vendor vendor)
    {
        _context.Vendors.Add(vendor);
        _context.SaveChanges();
        return vendor;
    }
    public void DeleteVendor(int id)
    {
        var vendor = _context.Vendors.Find(id);
        if (vendor != null) { _context.Vendors.Remove(vendor); _context.SaveChanges(); }
    }
}

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;
    public CustomerService(ApplicationDbContext context) => _context = context;

    public List<CustomerMaster> GetAllCustomers() => _context.Customers.ToList();
    
    public CustomerMaster GetCustomerByEmail(string email) => _context.Customers.FirstOrDefault(c => c.Email == email);
    
    public CustomerMaster AddCustomer(CustomerMaster customer)
    {
        // Check if customer exists by Email to prevent duplicates
        var existingCustomer = _context.Customers.FirstOrDefault(c => c.Email == customer.Email);
        if (existingCustomer != null)
        {
             // Update existing customer fields
             existingCustomer.FirstName = customer.FirstName;
             existingCustomer.LastName = customer.LastName;
             existingCustomer.MobileNumber = customer.MobileNumber;
             existingCustomer.AddressLine1 = customer.AddressLine1;
             
             // Handle nullable/optional updates
             if (!string.IsNullOrEmpty(customer.AddressLine2)) existingCustomer.AddressLine2 = customer.AddressLine2;
             if (!string.IsNullOrEmpty(customer.City)) existingCustomer.City = customer.City;
             if (!string.IsNullOrEmpty(customer.Pincode)) existingCustomer.Pincode = customer.Pincode;
             // Ensure PhoneNumber is set
             existingCustomer.PhoneNumber = !string.IsNullOrEmpty(customer.PhoneNumber) 
                                            ? customer.PhoneNumber 
                                            : (!string.IsNullOrEmpty(customer.MobileNumber) ? customer.MobileNumber : existingCustomer.PhoneNumber);

             if (!string.IsNullOrEmpty(customer.DrivingLicenseNumber)) existingCustomer.DrivingLicenseNumber = customer.DrivingLicenseNumber;
             // ... map other fields as needed or just save
             
             _context.SaveChanges();
             return existingCustomer;
        }

        // New Customer Logic
        customer.AddressLine2 ??= "";
        customer.City ??= "";
        customer.Pincode ??= "";
        
        // Fix for NOT NULL constraint on PhoneNumber
        if (string.IsNullOrEmpty(customer.PhoneNumber))
        {
            customer.PhoneNumber = customer.MobileNumber;
        }

        // Leave MembershipId null to avoid UNIQUE constraint violation on empty strings
        // customer.MembershipId ??= ""; 

        customer.IdpNumber ??= "";
        customer.PassportNumber ??= "";
        customer.PassportIssuedBy ??= "";
        
        _context.Customers.Add(customer);
        _context.SaveChanges();
        return customer;
    }

    public CustomerMaster UpdateCustomer(CustomerMaster customer)
    {
        var existing = _context.Customers.Find(customer.CustId);
        if (existing != null)
        {
             // Update properties logic. 
             // Simple mapping or manual update
             _context.Entry(existing).CurrentValues.SetValues(customer);
             _context.SaveChanges();
             return existing;
        }
        return null; // Or throw
    }
}
