namespace FleetManagementSystem.Api.Services;

public interface IInvoiceService
{
    void SendInvoiceEmail(long bookingId, string email);
}
