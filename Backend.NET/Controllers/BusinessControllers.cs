using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using FleetManagementSystem.Api.Models;
using FleetManagementSystem.Api.Services;
using FleetManagementSystem.Api.DTOs;

namespace FleetManagementSystem.Api.Controllers;

[ApiController]
[Route("api/v1")] 
public class AddOnController : ControllerBase
{
    private readonly IAddOnService _addOnService;
    public AddOnController(IAddOnService addOnService) => _addOnService = addOnService;

    [HttpGet("addons")]
    public ActionResult<List<AddOnMaster>> GetAllAddOns() => Ok(_addOnService.GetAllAddOns());

    [HttpPost("addons")]
    public ActionResult<AddOnMaster> AddAddOn([FromBody] AddOnMaster addOn) => Ok(_addOnService.AddAddOn(addOn));
    
    [HttpDelete("addons/{id}")]
    public IActionResult DeleteAddOn(int id)
    {
        _addOnService.DeleteAddOn(id);
        return Ok(new MessageResponse("Deleted successfully"));
    }
}

[ApiController]
[Route("api/v1")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;
    public VendorController(IVendorService vendorService) => _vendorService = vendorService;

    [HttpGet("vendors")]
    public ActionResult<List<Vendor>> GetAllVendors() => Ok(_vendorService.GetAllVendors());

    [HttpPost("vendors")]
    public ActionResult<Vendor> AddVendor([FromBody] Vendor vendor) => Ok(_vendorService.AddVendor(vendor));

    [HttpDelete("vendors/{id}")]
    public IActionResult DeleteVendor(int id)
    {
        _vendorService.DeleteVendor(id);
        return Ok(new MessageResponse("Deleted successfully"));
    }
}

[ApiController]
[Route("api/v1")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    public CustomerController(ICustomerService customerService) => _customerService = customerService;

    [HttpGet("customers")]
    public ActionResult<List<CustomerMaster>> GetAllCustomers() => Ok(_customerService.GetAllCustomers());

    [HttpGet("customers/{email}")]
    public ActionResult<CustomerMaster> GetCustomerByEmail(string email)
    {
        var cust = _customerService.GetCustomerByEmail(email);
        if (cust != null) return Ok(cust);
        return NotFound();
    }
    
    [HttpPost("customers")]
    public ActionResult<CustomerMaster> AddCustomer([FromBody] CustomerMaster customer)
    {
        return Ok(_customerService.AddCustomer(customer));
    }
    
    [HttpPut("customers/{id}")]
    public ActionResult<CustomerMaster> UpdateCustomer(int id, [FromBody] CustomerMaster customer)
    {
        customer.CustId = id;
        var updated = _customerService.UpdateCustomer(customer);
        if (updated != null) return Ok(updated);
        return NotFound();
    }
}
