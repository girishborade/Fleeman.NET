using System.Collections.Generic;
using FleetManagementSystem.Api.Models;
using System.Threading.Tasks;

namespace FleetManagementSystem.Api.Services;

public interface IAddOnService
{
    List<AddOnMaster> GetAllAddOns();
    AddOnMaster AddAddOn(AddOnMaster addOn);
    AddOnMaster GetAddOnById(int id); 
    void DeleteAddOn(int id);
}

public interface IVendorService
{
    List<Vendor> GetAllVendors();
    Vendor AddVendor(Vendor vendor);
    void DeleteVendor(int id);
}

public interface ICustomerService
{
    List<CustomerMaster> GetAllCustomers();
    CustomerMaster GetCustomerByEmail(string email);
    CustomerMaster AddCustomer(CustomerMaster customer);
    CustomerMaster UpdateCustomer(CustomerMaster customer); 
}
