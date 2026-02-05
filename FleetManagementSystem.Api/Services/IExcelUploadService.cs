using Microsoft.AspNetCore.Http;

namespace FleetManagementSystem.Api.Services;

public interface IExcelUploadService
{
    void Save(IFormFile file); // Keep for compatibility if needed, though likely unused now
    void SaveRates(IFormFile file);
    void SaveCars(IFormFile file);
}
