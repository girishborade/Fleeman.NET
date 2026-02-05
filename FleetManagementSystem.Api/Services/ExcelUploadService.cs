using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using FleetManagementSystem.Api.Data;
using FleetManagementSystem.Api.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace FleetManagementSystem.Api.Services;

public class ExcelUploadService : IExcelUploadService
{
    private readonly ApplicationDbContext _context;

    public ExcelUploadService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Save(IFormFile file)
    {
        // TODO: Implement Excel parsing using NPOI for Cars if needed in future
        Console.WriteLine($"Uploading file: {file.FileName}");
    }

    public void SaveRates(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = workbook.GetSheetAt(0);

            // Skip header row (index 0)
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                // Expected Columns:
                // 0: Car Type Name
                // 1: Daily Rate
                // 2: Weekly Rate
                // 3: Monthly Rate
                // 4: Image Path

                var carTypeName = GetCellValue(row.GetCell(0));
                if (string.IsNullOrWhiteSpace(carTypeName)) continue;

                var existingCarType = _context.CarTypes
                    .FirstOrDefault(c => c.CarTypeName == carTypeName);

                if (existingCarType != null)
                {
                    if (double.TryParse(GetCellValue(row.GetCell(1)), out double dailyRate))
                    {
                        existingCarType.DailyRate = dailyRate;
                    }

                    if (double.TryParse(GetCellValue(row.GetCell(2)), out double weeklyRate))
                    {
                        existingCarType.WeeklyRate = weeklyRate;
                    }

                    if (double.TryParse(GetCellValue(row.GetCell(3)), out double monthlyRate))
                    {
                        existingCarType.MonthlyRate = monthlyRate;
                    }

                    var imagePath = GetCellValue(row.GetCell(4));
                    if (!string.IsNullOrWhiteSpace(imagePath))
                    {
                        existingCarType.ImagePath = imagePath;
                    }
                }
                else
                {
                    // Create new CarTypeMaster
                    var newCarType = new CarTypeMaster
                    {
                        CarTypeName = carTypeName
                    };

                    if (double.TryParse(GetCellValue(row.GetCell(1)), out double dailyRate))
                    {
                        newCarType.DailyRate = dailyRate;
                    }

                    if (double.TryParse(GetCellValue(row.GetCell(2)), out double weeklyRate))
                    {
                        newCarType.WeeklyRate = weeklyRate;
                    }

                    if (double.TryParse(GetCellValue(row.GetCell(3)), out double monthlyRate))
                    {
                        newCarType.MonthlyRate = monthlyRate;
                    }

                    var imagePath = GetCellValue(row.GetCell(4));
                    if (!string.IsNullOrWhiteSpace(imagePath))
                    {
                        newCarType.ImagePath = imagePath;
                    }

                    _context.CarTypes.Add(newCarType);
                }
            }

            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to parse Excel file: " + ex.Message);
        }
    }

    public void SaveCars(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            IWorkbook workbook = new XSSFWorkbook(stream);
            ISheet sheet = workbook.GetSheetAt(0);

            // Skip header row (index 0)
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                // Expected Columns:
                // 0: Car Name
                // 1: Number Plate (Unique Key)
                // 2: Car Type Name (Lookup)
                // 3: Hub Name (Lookup)
                // 4: Status
                // 5: Is Available (Yes/No)
                // 6: Mileage
                // 7: Maintenance Due Date
                // 8: Image Path

                var carName = GetCellValue(row.GetCell(0));
                var numberPlate = GetCellValue(row.GetCell(1));
                
                if (string.IsNullOrWhiteSpace(numberPlate)) continue; // Mandatory

                // Lookup IDs
                var carTypeName = GetCellValue(row.GetCell(2));
                var carType = _context.CarTypes.FirstOrDefault(c => c.CarTypeName == carTypeName);
                long? carTypeId = carType?.CarTypeId;

                var hubName = GetCellValue(row.GetCell(3));
                var hub = _context.Hubs.FirstOrDefault(h => h.HubName == hubName);
                int? hubId = hub?.HubId;

                var status = GetCellValue(row.GetCell(4));
                var isAvailable = GetCellValue(row.GetCell(5));
                
                double mileage = 0;
                double.TryParse(GetCellValue(row.GetCell(6)), out mileage);

                DateTime? maintenanceDate = null;
                if (DateTime.TryParse(GetCellValue(row.GetCell(7)), out DateTime mDate))
                {
                    maintenanceDate = mDate;
                }

                var imagePath = GetCellValue(row.GetCell(8));

                var existingCar = _context.Cars.FirstOrDefault(c => c.NumberPlate == numberPlate);

                if (existingCar != null)
                {
                    // Update
                    existingCar.CarName = carName;
                    if (carTypeId.HasValue) existingCar.CarTypeId = carTypeId;
                    if (hubId.HasValue) existingCar.HubId = hubId;
                    if (!string.IsNullOrWhiteSpace(status)) existingCar.Status = status;
                    if (!string.IsNullOrWhiteSpace(isAvailable)) existingCar.IsAvailable = isAvailable;
                    existingCar.Mileage = mileage;
                    existingCar.MaintenanceDueDate = maintenanceDate;
                    if (!string.IsNullOrWhiteSpace(imagePath)) existingCar.ImagePath = imagePath;
                }
                else
                {
                    // Create
                    var newCar = new CarMaster
                    {
                        CarName = carName,
                        NumberPlate = numberPlate,
                        CarTypeId = carTypeId,
                        HubId = hubId,
                        Status = status,
                        IsAvailable = isAvailable,
                        Mileage = mileage,
                        MaintenanceDueDate = maintenanceDate,
                        ImagePath = imagePath
                    };
                    _context.Cars.Add(newCar);
                }
            }

            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to parse Excel file: " + ex.Message);
        }
    }

    private string GetCellValue(ICell cell)
    {
        if (cell == null) return "";
        
        switch (cell.CellType)
        {
            case CellType.String:
                return cell.StringCellValue;
            case CellType.Numeric:
                return cell.NumericCellValue.ToString();
            case CellType.Boolean:
                return cell.BooleanCellValue.ToString();
            default:
                return "";
        }
    }
}
