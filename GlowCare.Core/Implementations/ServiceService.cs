using GlowCare.Core.Contracts;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GlowCare.Core.Implementations;

public class ServiceService(
    IRepository<Service, int> serviceRepository,
    IRepository<Category, int> categoryRepository,
    IRepository<GlowCare.Entities.Models.EmployeeService, int> employeeServiceRepository,
    IRepository<Schedule, int> scheduleRepository,
    IRepository<Procedure, int> procedureRepository,
    UserManager<GlowUser> userManager
    ) : IServiceService
{
    public async Task CreateServiceAsync(AddServiceViewModel model, Guid userId)
    {
        if (model == null)
        {
            throw new NullReferenceException("Entity not found");
        }

        var service = new Service
        {
            Name = model.Name,
            CategoryId = model.CategoryId,
            Description = model.Description,
            DurationInMinutes = model.DurationInMinutes,
            Price = model.Price,
            Points = model.Points,
        };

        await serviceRepository.AddAsync(service);
    }

    public async Task DeleteServiceAsync(DeleteServiceViewModel model)
    {
        Service service = await serviceRepository.GetByIdAsync(model.Id);

        if (service == null)
        {
            throw new NullReferenceException("Entity not found.");
        }

        if (service.IsDeleted)
        {
            throw new ArgumentException("Entity is already deleted.");
        }

        service.IsDeleted = true;
        await serviceRepository.UpdateAsync(service);
    }

    public async Task<IEnumerable<ServiceInfoViewModel>> GetAllServicesAsync()
    {
        return await serviceRepository
            .GetAllAttached()
            .Where(s => !s.IsDeleted)
            .Include(s => s.Category)
            .Select(s => new ServiceInfoViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Price = s.Price,
                CategoryId = s.CategoryId,
                CategoryName = s.Category != null ? s.Category.Name : string.Empty,
                DurationInMinutes = s.DurationInMinutes,
                EarliestAvailableSlot = null
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceInfoViewModel>> GetFilteredServicesAsync(
        int? categoryId,
        string? priceRange,
        string? availabilityRange)
    {
        var services = await serviceRepository
            .GetAllAttached()
            .Where(s => !s.IsDeleted)
            .Include(s => s.Category)
            .ToListAsync();

        var result = new List<ServiceInfoViewModel>();

        foreach (var service in services)
        {
            var earliestSlot = await GetEarliestAvailableSlotForServiceAsync(service.Id, DateTime.Now);

            result.Add(new ServiceInfoViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                CategoryId = service.CategoryId,
                CategoryName = service.Category?.Name ?? string.Empty,
                DurationInMinutes = service.DurationInMinutes,
                EarliestAvailableSlot = earliestSlot
            });
        }

        if (categoryId.HasValue)
        {
            result = result
                .Where(s => s.CategoryId == categoryId.Value)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(priceRange))
        {
            result = priceRange switch
            {
                "under50" => result.Where(s => s.Price < 50m).ToList(),
                "50to100" => result.Where(s => s.Price >= 50m && s.Price <= 100m).ToList(),
                "100to200" => result.Where(s => s.Price > 100m && s.Price <= 200m).ToList(),
                "200plus" => result.Where(s => s.Price > 200m).ToList(),
                _ => result
            };
        }

        if (!string.IsNullOrWhiteSpace(availabilityRange))
        {
            var now = DateTime.Now;
            var todayEnd = now.Date.AddDays(1).AddTicks(-1);
            var next3DaysEnd = now.Date.AddDays(3).AddDays(1).AddTicks(-1);
            var thisWeekEnd = now.Date.AddDays(7).AddDays(1).AddTicks(-1);

            result = availabilityRange switch
            {
                "today" => result.Where(s =>
                    s.EarliestAvailableSlot.HasValue &&
                    s.EarliestAvailableSlot.Value >= now &&
                    s.EarliestAvailableSlot.Value <= todayEnd).ToList(),

                "next3days" => result.Where(s =>
                    s.EarliestAvailableSlot.HasValue &&
                    s.EarliestAvailableSlot.Value >= now &&
                    s.EarliestAvailableSlot.Value <= next3DaysEnd).ToList(),

                "thisWeek" => result.Where(s =>
                    s.EarliestAvailableSlot.HasValue &&
                    s.EarliestAvailableSlot.Value >= now &&
                    s.EarliestAvailableSlot.Value <= thisWeekEnd).ToList(),

                _ => result
            };
        }

        return result
            .OrderBy(s => s.Name)
            .ToList();
    }

    public async Task<IEnumerable<ServiceCategoryOptionViewModel>> GetCategoryOptionsAsync()
    {
        return await categoryRepository
            .GetAllAttached()
            .OrderBy(c => c.Name)
            .Select(c => new ServiceCategoryOptionViewModel
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    private async Task<DateTime?> GetEarliestAvailableSlotForServiceAsync(int serviceId, DateTime from)
    {
        var service = await serviceRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(s => s.Id == serviceId && !s.IsDeleted);

        if (service == null)
        {
            return null;
        }

        var employeeIds = await employeeServiceRepository
            .GetAllAttached()
            .Where(es => es.ServiceId == serviceId)
            .Select(es => es.EmployeeId)
            .Distinct()
            .ToListAsync();

        if (!employeeIds.Any())
        {
            return null;
        }

        var schedules = await scheduleRepository
            .GetAllAttached()
            .Where(s => employeeIds.Contains(s.EmployeeId))
            .ToListAsync();

        var procedures = await procedureRepository
            .GetAllAttached()
            .Where(p =>
                employeeIds.Contains(p.EmployeeId) &&
                !p.IsDeleted &&
                p.Status != Entities.Models.Enums.Status.Cancelled &&
                p.AppointmentDate >= from.Date)
            .Include(p => p.Service)
            .ToListAsync();

        for (int dayOffset = 0; dayOffset < 7; dayOffset++)
        {
            var currentDate = from.Date.AddDays(dayOffset);

            var daySchedules = schedules
                .Where(s => s.DayOfWeek == currentDate.DayOfWeek)
                .ToList();

            foreach (var schedule in daySchedules)
            {
                if (!TimeSpan.TryParse(schedule.StartTime, out var startTime) ||
                    !TimeSpan.TryParse(schedule.EndTime, out var endTime))
                {
                    continue;
                }

                var workStart = currentDate.Add(startTime);
                var workEnd = currentDate.Add(endTime);

                var slotStart = workStart;

                if (currentDate == from.Date && from > workStart)
                {
                    slotStart = RoundUpToNext30Minutes(from);
                }

                while (slotStart.AddMinutes(service.DurationInMinutes) <= workEnd)
                {
                    var slotEnd = slotStart.AddMinutes(service.DurationInMinutes);

                    var employeeProcedures = procedures
                        .Where(p => p.EmployeeId == schedule.EmployeeId &&
                                    p.AppointmentDate.Date == currentDate.Date)
                        .ToList();

                    bool hasConflict = employeeProcedures.Any(p =>
                    {
                        if (p.Service == null)
                        {
                            return false;
                        }

                        var existingStart = p.AppointmentDate;
                        var existingEnd = p.AppointmentDate.AddMinutes(p.Service.DurationInMinutes);

                        return slotStart < existingEnd && slotEnd > existingStart;
                    });

                    if (!hasConflict)
                    {
                        return slotStart;
                    }

                    slotStart = slotStart.AddMinutes(30);
                }
            }
        }

        return null;
    }

    private static DateTime RoundUpToNext30Minutes(DateTime dateTime)
    {
        var minutesToAdd = 30 - (dateTime.Minute % 30);

        if (minutesToAdd == 30 && dateTime.Second == 0 && dateTime.Millisecond == 0)
        {
            minutesToAdd = 0;
        }

        return new DateTime(
            dateTime.Year,
            dateTime.Month,
            dateTime.Day,
            dateTime.Hour,
            dateTime.Minute,
            0).AddMinutes(minutesToAdd);
    }

    public Task<Procedure> EditProcedureAsync(EditServiceViewModel model, int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<DetailsServiceViewModel>> GetAllProcedureDetailsByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<DeleteServiceViewModel> GetDeleteProcedureAsync(int id, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<EditServiceViewModel> GetEditProcedureAsync(int id)
    {
        throw new NotImplementedException();
    }
}