using GlowCare.Core.Contracts;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.Entities.Models.Enums;
using GlowCare.ViewModels.Procedures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using static GlowCare.Common.Constants.ProcedureConstants;

namespace GlowCare.Core.Implementations;

public class ProcedureService(
    IRepository<Procedure, int> procedureRepository,
    IRepository<Service, int> serviceRepository,
    IRepository<Employee, int> employeeRepository,
    IRepository<GlowCare.Entities.Models.EmployeeService, int> employeeServiceRepository,
    IRepository<EmployeeSchedule, int> employeeScheduleRepository,
    UserManager<GlowUser> userManager)
    : IProcedureService
{
    public async Task CreateProcedureAsync(
        AddProcedureViewModel model,
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

    public async Task DeleteProcedureAsync(
        DeleteProcedureViewModel model)
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

        if (!procedure.IsDeleted || procedure != null)
        {
            procedure.IsDeleted = true;

            await procedureRepository.UpdateAsync(procedure);
        }
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

    public async Task<IEnumerable<DetailsProcedureViewModel>> GetAllProcedureDetailsByUserIdAsync(
        Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new NullReferenceException("User not found");
        }


        var procedures = await procedureRepository
                .GetAllAttached()
                .Include(p => p.Employee)
                .ThenInclude(e => e!.User)
                .Include(p => p.Service)
                .Where(p => p.IsDeleted == false && p.UserId == userId)
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


    public async Task<DeleteProcedureViewModel> GetDeleteProcedureAsync(
        int id,
        Guid userId)
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

        var procedure = new DeleteProcedureViewModel
        {
            Id = entity.Id,
            ClientName = $"{user!.FirstName} {user!.LastName}",
            ServiceName = service.Name,
        };

        return procedure;
    }

    public async Task<EditProcedureViewModel> GetEditProcedureAsync(
        int id)
    {
        var entity = await procedureRepository.GetByIdAsync(id)
            ?? throw new NullReferenceException("Entity not found");

        if (entity.IsDeleted)
        {
            throw new ArgumentException("Entity already deleted");
        }

        var procedure = new EditProcedureViewModel()
        {
            EmployeeId = entity.EmployeeId,
            ServiceId = entity.ServiceId,
            AppointmentDate = entity.AppointmentDate,
            Notes = entity.Notes,
        };

        return procedure;
    }

    public async Task<bool> IsSlotAvailableAsync(Guid employeeId, int serviceId, DateTime requestedTime)
    {
        var service = await serviceRepository.GetAllAttached()
            .FirstOrDefaultAsync(s => s.Id == serviceId && !s.IsDeleted);

        if (service == null)
        {
            throw new ArgumentException("Service not found.");
        }

        int duration = service.DurationInMinutes;
        string requestedDay = requestedTime.DayOfWeek.ToString();

        var employeeSchedules = await employeeScheduleRepository.GetAllAttached()
            .Where(es => es.EmployeeId == employeeId)
            .Include(es => es.Schedule)
            .ToListAsync();

        var employeeSchedule = employeeSchedules
            .FirstOrDefault(es =>
                es.Schedule != null &&
                !string.IsNullOrWhiteSpace(es.Schedule.DaysOfWeek) &&
                es.Schedule.DaysOfWeek
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Trim())
                    .Any(d => d.Equals(requestedDay, StringComparison.OrdinalIgnoreCase)));

        if (employeeSchedule?.Schedule == null)
        {
            return false;
        }

        var schedule = employeeSchedule.Schedule;

        if (!TimeSpan.TryParse(schedule.StartTime, out var startTime) ||
            !TimeSpan.TryParse(schedule.EndTime, out var endTime))
        {
            return false;
        }

        var workStart = requestedTime.Date.Add(startTime);
        var workEnd = requestedTime.Date.Add(endTime);
        var requestedEnd = requestedTime.AddMinutes(duration);

        if (requestedTime < workStart || requestedEnd > workEnd)
        {
            return false;
        }

        var procedures = await procedureRepository.GetAllAttached()
            .Where(p => p.EmployeeId == employeeId
                        && !p.IsDeleted
                        && p.Status != Status.Cancelled
                        && p.AppointmentDate.Date == requestedTime.Date)
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

        return !hasConflict;
    }

    public async Task<IEnumerable<SelectListItem>> GetEmployeeSelectListAsync()
    {
        return await employeeRepository
            .GetAllAttached()
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
            .Select(es => new SelectListItem
            {
                Value = es.Service.Id.ToString(),
                Text = es.Service.Name
            })
            .ToListAsync();
    }
}

