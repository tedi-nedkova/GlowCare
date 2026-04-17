using GlowCare.Core.Contracts;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Procedures;
using GlowCare.ViewModels.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static GlowCare.Common.Constants.ProcedureConstants;

namespace GlowCare.Core.Implementations;

public class ProcedureService(
           IRepository<Procedure, int> procedureRepository,
           IRepository<Service, int> serviceRepository,
           IRepository<Employee, int> employeeRepository,
           IRepository<Schedule, int> scheduleRepository,
           IRepository<GlowCare.Entities.Models.EmployeeService, int> employeeServiceRepository,
           UserManager<GlowUser> userManager)
           : IProcedureService
{
    public async Task CreateProcedureAsync(
        IndexViewModel model,
        Guid userId)
    {
        if (model == null)
        {
            throw new NullReferenceException("Entity not found.");
        }

        Procedure procedure = new()
        {
            UserId = userId,
            EmployeeId = model.EmployeeId,
            ServiceId = model.ServiceId,
            AppointmentDate = model.AppointmentDate,
            Status = Entities.Models.Enums.Status.Scheduled,
            Notes = model.Notes,
        };

        await procedureRepository.AddAsync(procedure);
    }

    public async Task DeleteProcedureAsync(DeleteProcedureViewModel model)
    {
        Procedure procedure = await procedureRepository.GetByIdAsync(model.Id);

        if (procedure == null)
        {
            throw new NullReferenceException("Entity not found.");
        }

        if (procedure.IsDeleted)
        {
            throw new ArgumentException("Entity is already deleted.");
        }

        procedure.IsDeleted = true;
        await procedureRepository.UpdateAsync(procedure);
    }

    public async Task<Procedure> EditProcedureAsync(
        EditProcedureViewModel model,
        int id)
    {
        Procedure procedure = await procedureRepository.GetByIdAsync(id)
            ?? throw new NullReferenceException("Entity not found.");

        if (procedure.IsDeleted)
        {
            throw new ArgumentException("Entity is already deleted.");
        }

        procedure.EmployeeId = model.EmployeeId;
        procedure.ServiceId = model.ServiceId;
        procedure.AppointmentDate = model.AppointmentDate;
        procedure.Notes = model.Notes;

        await procedureRepository.UpdateAsync(procedure);

        return procedure;
    }

    public async Task<IEnumerable<DetailsProcedureViewModel>> GetAllProcedureDetailsByUserIdAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new NullReferenceException("User not found.");
        }

        var procedures = await procedureRepository
            .GetAllAttached()
            .Include(p => p.Employee)
                .ThenInclude(e => e!.User)
            .Include(p => p.Service)
            .Where(p => !p.IsDeleted && p.UserId == userId)
            .Select(p => new DetailsProcedureViewModel
            {
                Id = p.Id,
                SpecialistName = $"{p.Employee!.User.FirstName} {p.Employee!.User.LastName}",
                Service = p.Service!.Name,
                Price = p.Service.Price,
                AppointmentDate = p.AppointmentDate.ToString(AppointmentDateFormat)
            })
            .ToListAsync();

        return procedures;
    }

    public async Task<DeleteProcedureViewModel> GetDeleteProcedureAsync(int id, Guid userId)
    {
        List<Procedure> entities = await procedureRepository.GetAllAttached()
            .Include(p => p.Service)
            .Include(p => p.User)
            .ToListAsync();

        Procedure entity = entities
            .FirstOrDefault(e => e.Id == id)
            ?? throw new NullReferenceException("Entity not found.");

        if (entity.IsDeleted)
        {
            throw new NullReferenceException("Entity is already deleted.");
        }

        if (entity.UserId == null || entity.UserId != userId)
        {
            throw new NullReferenceException("Invalid client id.");
        }

        var user = entity.User;
        Service service = entity.Service ?? throw new NullReferenceException("Service not found.");

        return new DeleteProcedureViewModel
        {
            Id = entity.Id,
            ClientName = $"{user!.FirstName} {user!.LastName}",
            ServiceName = service.Name,
        };
    }

    public async Task<EditProcedureViewModel> GetEditProcedureAsync(int id)
    {
        var entity = await procedureRepository.GetByIdAsync(id)
            ?? throw new NullReferenceException("Entity not found.");

        if (entity.IsDeleted)
        {
            throw new ArgumentException("Entity already deleted.");
        }

        return new EditProcedureViewModel
        {
            EmployeeId = entity.EmployeeId,
            ServiceId = entity.ServiceId,
            AppointmentDate = entity.AppointmentDate,
            Notes = entity.Notes,
        };
    }

    public async Task<AvailabilityCheckResultViewModel> IsSlotAvailableAsync(Guid employeeId, int serviceId, DateTime requestedTime)
    {
        var service = await serviceRepository.GetAllAttached()
            .FirstOrDefaultAsync(s => s.Id == serviceId && !s.IsDeleted);

        if (service == null)
        {
            return new AvailabilityCheckResultViewModel
            {
                IsAvailable = false,
                Message = "Услугата не беше намерена."
            };
        }

        bool employeeCanDoService = await employeeServiceRepository
            .GetAllAttached()
            .AnyAsync(es => es.EmployeeId == employeeId && es.ServiceId == serviceId);

        if (!employeeCanDoService)
        {
            return new AvailabilityCheckResultViewModel
            {
                IsAvailable = false,
                Message = "Избраният специалист не извършва тази процедура."
            };
        }

        var schedule = await scheduleRepository.GetAllAttached()
            .FirstOrDefaultAsync(s =>
                s.EmployeeId == employeeId &&
                s.DayOfWeek == requestedTime.DayOfWeek);

        if (schedule == null)
        {
            return new AvailabilityCheckResultViewModel
            {
                IsAvailable = false,
                Message = "Избраният час не е зает, но специалистът не работи в този ден."
            };
        }

        if (!TimeSpan.TryParse(schedule.StartTime, out var startTime) ||
            !TimeSpan.TryParse(schedule.EndTime, out var endTime))
        {
            return new AvailabilityCheckResultViewModel
            {
                IsAvailable = false,
                Message = "Работното време на специалиста не е конфигурирано правилно."
            };
        }

        int duration = service.DurationInMinutes;

        var workStart = requestedTime.Date.Add(startTime);
        var workEnd = requestedTime.Date.Add(endTime);
        var requestedEnd = requestedTime.AddMinutes(duration);

        if (requestedTime < workStart || requestedEnd > workEnd)
        {
            return new AvailabilityCheckResultViewModel
            {
                IsAvailable = false,
                Message = $"Избраният час не е зает, но е извън работното време на специалиста ({schedule.StartTime} - {schedule.EndTime})."
            };
        }

        var procedures = await procedureRepository.GetAllAttached()
            .Where(p =>
                p.EmployeeId == employeeId &&
                !p.IsDeleted &&
                p.Status != Entities.Models.Enums.Status.Cancelled &&
                p.AppointmentDate.Date == requestedTime.Date)
            .Include(p => p.Service)
            .ToListAsync();

        bool hasConflict = procedures.Any(p =>
        {
            if (p.Service == null)
            {
                return false;
            }

            var existingStart = p.AppointmentDate;
            var existingEnd = p.AppointmentDate.AddMinutes(p.Service.DurationInMinutes);

            return requestedTime < existingEnd && requestedEnd > existingStart;
        });

        if (hasConflict)
        {
            return new AvailabilityCheckResultViewModel
            {
                IsAvailable = false,
                Message = "Избраният час е зает."
            };
        }

        return new AvailabilityCheckResultViewModel
        {
            IsAvailable = true,
            Message = "Избраният час е свободен."
        };
    }

    public async Task<IEnumerable<SelectListItem>> GetEmployeeSelectListAsync()
    {
        return await employeeRepository
            .GetAllAttached()
            .Include(e => e.User)
            .Where(e => !e.IsDeleted
                && e.User != null
                && !e.User.IsDeleted
                && e.User.IsSpecialist)
            .Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.User.FirstName + " " + e.User.LastName + " - " + e.Occupation
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<SelectListItem>> GetServiceSelectListAsync()
    {
        return await serviceRepository
            .GetAllAttached()
            .Where(s => !s.IsDeleted)
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<SelectListItem>> GetServicesByEmployeeIdAsync(Guid employeeId)
    {
        return await employeeServiceRepository
            .GetAllAttached()
            .Where(es => es.EmployeeId == employeeId)
            .Include(es => es.Service)
            .Where(es => es.Service != null && !es.Service.IsDeleted)
            .Select(es => new SelectListItem
            {
                Value = es.Service!.Id.ToString(),
                Text = es.Service.Name
            })
            .ToListAsync();
    }
}