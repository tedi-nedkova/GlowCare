using GlowCare.Core.Contracts;
using GlowCare.Core.Helpers;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.Entities.Models.Enums;
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
           UserManager<GlowUser> userManager,
           IUserService userService)
           : IProcedureService
{
    public async Task CreateProcedureAsync(
        IndexViewModel model,
        Guid userId)
    {
        if (model == null)
        {
            throw new NullReferenceException("Записът не беше намерен.");
        }

        Procedure procedure = new()
        {
            UserId = userId,
            EmployeeId = model.EmployeeId,
            ServiceId = model.ServiceId,
            AppointmentDate = model.AppointmentDate,
            Status = Status.Scheduled,
            Notes = model.Notes,
            RewardPointsGranted = false,
        };

        await procedureRepository.AddAsync(procedure);
    }

    public async Task DeleteProcedureAsync(DeleteProcedureViewModel model)
    {
        Procedure procedure = await procedureRepository.GetByIdAsync(model.Id);

        if (procedure == null)
        {
            throw new NullReferenceException("Записът не беше намерен.");
        }

        if (procedure.IsDeleted)
        {
            throw new ArgumentException("Записът вече е изтрит.");
        }

        procedure.IsDeleted = true;
        await procedureRepository.UpdateAsync(procedure);
    }

    public async Task<Procedure> EditProcedureAsync(
        EditProcedureViewModel model,
        int id)
    {
        Procedure procedure = await procedureRepository.GetByIdAsync(id)
            ?? throw new NullReferenceException("Записът не беше намерен.");

        if (procedure.IsDeleted)
        {
            throw new ArgumentException("Записът вече е изтрит.");
        }

        procedure.EmployeeId = model.EmployeeId;
        procedure.ServiceId = model.ServiceId;
        procedure.AppointmentDate = model.AppointmentDate;
        procedure.Notes = model.Notes;

        if (procedure.Status == Status.Completed && procedure.AppointmentDate > DateTime.Now)
        {
            procedure.Status = Status.Scheduled;
            procedure.RewardPointsGranted = false;
        }

        await procedureRepository.UpdateAsync(procedure);

        return procedure;
    }

    public async Task<IEnumerable<DetailsProcedureViewModel>> GetAllProcedureDetailsByUserIdAsync(Guid userId)
    {
        var user = await userManager.Users
            .Include(u => u.Membership)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new NullReferenceException("Потребителят не беше намерен.");
        }

        await userService.UpdateUserMembershipAsync(user);
        await SyncCompletedProceduresForUserAsync(user);

        var procedures = await procedureRepository
            .GetAllAttached()
            .Include(p => p.Employee)
                .ThenInclude(e => e!.User)
            .Include(p => p.Service)
            .Where(p => !p.IsDeleted && p.UserId == userId)
            .OrderByDescending(p => p.AppointmentDate)
            .Select(p => new DetailsProcedureViewModel
            {
                Id = p.Id,
                EmployeeId = p.EmployeeId,
                SpecialistName = $"{p.Employee!.User.FirstName} {p.Employee!.User.LastName}",
                Service = p.Service!.Name,
                Price = p.Service.Price,
                AppointmentDate = p.AppointmentDate.ToString(AppointmentDateFormat),
                Status = BulgarianTextHelper.GetProcedureStatusText(p.Status!.Value),
                EarnedPoints = p.Status == Status.Completed ? p.Service.Points : 0
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
            ?? throw new NullReferenceException("Записът не беше намерен.");

        if (entity.IsDeleted)
        {
            throw new NullReferenceException("Записът вече е изтрит.");
        }

        if (entity.UserId != userId)
        {
            throw new NullReferenceException("Невалиден идентификатор на клиент.");
        }

        var userEntity = entity.User;
        Service service = entity.Service ?? throw new NullReferenceException("Услугата не беше намерена.");

        return new DeleteProcedureViewModel
        {
            Id = entity.Id,
            ClientName = $"{userEntity!.FirstName} {userEntity!.LastName}",
            ServiceName = service.Name,
        };
    }

    public async Task<EditProcedureViewModel> GetEditProcedureAsync(int id)
    {
        var entity = await procedureRepository.GetByIdAsync(id)
            ?? throw new NullReferenceException("Записът не беше намерен.");

        if (entity.IsDeleted)
        {
            throw new ArgumentException("Записът вече е изтрит.");
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
                Message = "Специалистът не работи в този ден."
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
                Message = $"Избраният час е извън работното време на специалиста ({schedule.StartTime} - {schedule.EndTime})."
            };
        }

        var procedures = await procedureRepository.GetAllAttached()
            .Where(p =>
                p.EmployeeId == employeeId &&
                !p.IsDeleted &&
                p.Status != Status.Cancelled &&
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
            Message = $"Часът е свободен. Продължителност: {service.DurationInMinutes} минути."
        };
    }

    public async Task<IEnumerable<SelectListItem>> GetEmployeeSelectListAsync()
        => await employeeRepository.GetAllAttached()
            .Where(e => !e.IsDeleted)
            .Include(e => e.User)
            .Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = $"{e.User!.FirstName} {e.User.LastName}"
            })
            .ToListAsync();

    public async Task<IEnumerable<SelectListItem>> GetServiceSelectListAsync()
        => await serviceRepository.GetAllAttached()
            .Where(s => !s.IsDeleted)
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            })
            .ToListAsync();

    public async Task<IEnumerable<SelectListItem>> GetServicesByEmployeeIdAsync(Guid employeeId)
    {
        return await employeeServiceRepository.GetAllAttached()
            .Where(es => es.EmployeeId == employeeId)
            .Include(es => es.Service)
            .Where(es => es.Service != null && !es.Service.IsDeleted)
            .Select(es => new SelectListItem
            {
                Value = es.ServiceId.ToString(),
                Text = es.Service!.Name
            })
            .Distinct()
            .ToListAsync();
    }

    public async Task CancelProcedureAsync(int procedureId, Guid userId)
    {
        Procedure procedure = await procedureRepository
            .GetAllAttached()
            .Include(p => p.Employee)
            .FirstOrDefaultAsync(p => p.Id == procedureId && !p.IsDeleted)
            ?? throw new NullReferenceException("Процедурата не беше намерена.");

        bool isClient = procedure.UserId == userId;
        bool isSpecialist = procedure.Employee != null && procedure.Employee.UserId == userId;

        if (!isClient && !isSpecialist)
        {
            throw new UnauthorizedAccessException("Нямате право да отказвате този час.");
        }

        if (procedure.Status != Status.Scheduled || procedure.AppointmentDate <= DateTime.Now)
        {
            throw new InvalidOperationException("Могат да бъдат отказвани само бъдещи планирани часове.");
        }

        procedure.Status = Status.Cancelled;
        procedure.IsDeleted = true;

        await procedureRepository.UpdateAsync(procedure);
    }

    private async Task SyncCompletedProceduresForUserAsync(GlowUser user)
    {
        var procedures = await procedureRepository
            .GetAllAttached()
            .Where(p => p.UserId == user.Id
                && !p.IsDeleted
                && p.Status == Status.Scheduled
                && p.AppointmentDate <= DateTime.Now)
            .Include(p => p.Service)
            .ToListAsync();

        if (!procedures.Any())
        {
            return;
        }

        foreach (var procedure in procedures)
        {
            procedure.Status = Status.Completed;

            if (!procedure.RewardPointsGranted && procedure.Service != null)
            {
                user.LoyaltyPoints += procedure.Service.Points;
                procedure.RewardPointsGranted = true;
            }

            await procedureRepository.UpdateAsync(procedure);
        }

        await userService.UpdateUserMembershipAsync(user);
    }
}
