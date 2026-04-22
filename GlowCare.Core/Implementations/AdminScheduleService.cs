using System.Globalization;
using GlowCare.Core.Contracts;
using GlowCare.Entities;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Admin.Schedules;
using GlowCare.ViewModels.Common;
using Microsoft.EntityFrameworkCore;

namespace GlowCare.Core.Implementations;

public class AdminScheduleService(GlowCareDbContext context) : IAdminScheduleService
{
    public async Task<AdminScheduleManagementViewModel> GetScheduleManagementViewModelAsync()
    {
        List<Schedule> schedulesData = await context.Schedules
            .AsNoTracking()
            .Include(s => s.Employee)
            .ThenInclude(e => e!.User)
            .Where(s => s.Employee != null &&
                        !s.Employee.IsDeleted &&
                        !s.Employee.User.IsDeleted &&
                        s.Employee.User.IsSpecialist)
            .OrderBy(s => s.Employee!.User.FirstName)
            .ThenBy(s => s.Employee!.User.LastName)
            .ThenBy(s => s.DayOfWeek)
            .ThenBy(s => s.StartTime)
            .ToListAsync();

        List<AdminScheduleListItemViewModel> schedules = schedulesData
            .Select(s => new AdminScheduleListItemViewModel
            {
                Id = s.Id,
                EmployeeId = s.EmployeeId,
                SpecialistName = $"{s.Employee!.User.FirstName} {s.Employee.User.LastName}",
                DayOfWeek = GetDayNameInBulgarian(s.DayOfWeek),
                StartTime = s.StartTime,
                EndTime = s.EndTime
            })
            .ToList();

        return new AdminScheduleManagementViewModel
        {
            Schedules = schedules
        };
    }

    public async Task<CreateAdminScheduleViewModel> GetCreateScheduleViewModelAsync(
        CreateAdminScheduleViewModel? formModel = null)
    {
        CreateAdminScheduleViewModel model = formModel ?? new CreateAdminScheduleViewModel();

        model.Specialists = await GetSpecialistsAsync();
        model.Days = GetDays();

        return model;
    }

    public async Task<EditAdminScheduleViewModel> GetEditScheduleViewModelAsync(
        int id,
        EditAdminScheduleViewModel? formModel = null)
    {
        Schedule? schedule = await context.Schedules
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (schedule == null)
        {
            throw new NullReferenceException("Работното време не беше намерено.");
        }

        EditAdminScheduleViewModel model = formModel ?? new EditAdminScheduleViewModel
        {
            Id = schedule.Id,
            EmployeeId = schedule.EmployeeId,
            DayOfWeek = schedule.DayOfWeek,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime
        };

        model.Specialists = await GetSpecialistsAsync();
        model.Days = GetDays();

        return model;
    }

    public async Task CreateScheduleAsync(CreateAdminScheduleViewModel model)
    {
        ValidateRequiredFields(model.EmployeeId, model.DayOfWeek);
        await EnsureEmployeeExistsAndIsSpecialistAsync(model.EmployeeId!.Value);
        ValidateTimeRange(model.StartTime, model.EndTime);

        bool duplicateExists = await context.Schedules
            .AnyAsync(s => s.EmployeeId == model.EmployeeId.Value &&
                           s.DayOfWeek == model.DayOfWeek!.Value);

        if (duplicateExists)
        {
            throw new ArgumentException("Вече съществува работно време за този специалист и ден.");
        }

        Schedule schedule = new()
        {
            EmployeeId = model.EmployeeId.Value,
            DayOfWeek = model.DayOfWeek.Value,
            StartTime = model.StartTime,
            EndTime = model.EndTime
        };

        await context.Schedules.AddAsync(schedule);
        await context.SaveChangesAsync();
    }

    public async Task EditScheduleAsync(EditAdminScheduleViewModel model)
    {
        Schedule? schedule = await context.Schedules
            .FirstOrDefaultAsync(s => s.Id == model.Id);

        if (schedule == null)
        {
            throw new NullReferenceException("Работното време не беше намерено.");
        }

        ValidateRequiredFields(model.EmployeeId, model.DayOfWeek);
        await EnsureEmployeeExistsAndIsSpecialistAsync(model.EmployeeId!.Value);
        ValidateTimeRange(model.StartTime, model.EndTime);

        bool duplicateExists = await context.Schedules
            .AnyAsync(s => s.Id != model.Id &&
                           s.EmployeeId == model.EmployeeId.Value &&
                           s.DayOfWeek == model.DayOfWeek!.Value);

        if (duplicateExists)
        {
            throw new ArgumentException("Вече съществува работно време за този специалист и ден.");
        }

        schedule.EmployeeId = model.EmployeeId.Value;
        schedule.DayOfWeek = model.DayOfWeek.Value;
        schedule.StartTime = model.StartTime;
        schedule.EndTime = model.EndTime;

        await context.SaveChangesAsync();
    }

    public async Task DeleteScheduleAsync(int id)
    {
        Schedule? schedule = await context.Schedules
            .FirstOrDefaultAsync(s => s.Id == id);

        if (schedule == null)
        {
            throw new NullReferenceException("Работното време не беше намерено.");
        }

        context.Schedules.Remove(schedule);
        await context.SaveChangesAsync();
    }

    private async Task<List<DropdownItemViewModel>> GetSpecialistsAsync()
    {
        return await context.Employees
            .AsNoTracking()
            .Where(e => !e.IsDeleted &&
                        !e.User.IsDeleted &&
                        e.User.IsSpecialist)
            .OrderBy(e => e.User.FirstName)
            .ThenBy(e => e.User.LastName)
            .Select(e => new DropdownItemViewModel
            {
                Value = e.Id.ToString(),
                Text = $"{e.User.FirstName} {e.User.LastName}"
            })
            .ToListAsync();
    }

    private static List<DropdownItemViewModel> GetDays()
    {
        return Enum.GetValues<DayOfWeek>()
            .Select(day => new DropdownItemViewModel
            {
                Value = day.ToString(),
                Text = GetDayNameInBulgarian(day)
            })
            .ToList();
    }

    private async Task EnsureEmployeeExistsAndIsSpecialistAsync(Guid employeeId)
    {
        Employee? employee = await context.Employees
            .AsNoTracking()
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted);

        if (employee == null)
        {
            throw new NullReferenceException("Избраният специалист не беше намерен.");
        }

        if (employee.User.IsDeleted || !employee.User.IsSpecialist)
        {
            throw new ArgumentException("Избраният потребител не е активен специалист.");
        }
    }

    private static void ValidateRequiredFields(Guid? employeeId, DayOfWeek? dayOfWeek)
    {
        if (employeeId == null || employeeId == Guid.Empty)
        {
            throw new ArgumentException("Моля, изберете специалист.");
        }

        if (dayOfWeek == null)
        {
            throw new ArgumentException("Моля, изберете ден.");
        }
    }

    private static void ValidateTimeRange(string startTime, string endTime)
    {
        bool startValid = TimeOnly.TryParseExact(
            startTime,
            "HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out TimeOnly parsedStart);

        bool endValid = TimeOnly.TryParseExact(
            endTime,
            "HH:mm",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out TimeOnly parsedEnd);

        if (!startValid || !endValid)
        {
            throw new ArgumentException("Часът трябва да е във формат HH:mm.");
        }

        if (parsedStart >= parsedEnd)
        {
            throw new ArgumentException("Началният час трябва да бъде преди крайния.");
        }
    }

    private static string GetDayNameInBulgarian(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => "Понеделник",
            DayOfWeek.Tuesday => "Вторник",
            DayOfWeek.Wednesday => "Сряда",
            DayOfWeek.Thursday => "Четвъртък",
            DayOfWeek.Friday => "Петък",
            DayOfWeek.Saturday => "Събота",
            DayOfWeek.Sunday => "Неделя",
            _ => dayOfWeek.ToString()
        };
    }
}