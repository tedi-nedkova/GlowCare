using GlowCare.Core.Contracts;
using GlowCare.Entities;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Services;
using Microsoft.EntityFrameworkCore;

namespace GlowCare.Core.Implementations;

public class ServiceService(
    GlowCareDbContext dbContext,
    IRepository<Service, int> serviceRepository,
    IRepository<Category, int> categoryRepository,
    IRepository<Employee, Guid> employeeRepository,
    IRepository<GlowCare.Entities.Models.EmployeeService, int> employeeServiceRepository,
    IRepository<Schedule, int> scheduleRepository,
    IRepository<Procedure, int> procedureRepository)
    : IServiceService
{
    public async Task CreateServiceAsync(AddServiceViewModel model, Guid userId)
    {
        if (model == null)
        {
            throw new NullReferenceException("Записът не беше намерен.");
        }

        Service service = new()
        {
            Name = model.Name,
            CategoryId = model.CategoryId,
            Description = model.Description,
            DurationInMinutes = model.DurationInMinutes,
            Price = model.Price,
            Points = model.Points,
        };

        await serviceRepository.AddAsync(service);

        IEnumerable<Guid> selectedEmployeeIds = (model.SelectedEmployeeIds ?? Array.Empty<Guid>())
            .Where(id => id != Guid.Empty)
            .Distinct();

        if (!selectedEmployeeIds.Any())
        {
            return;
        }

        List<Guid> validEmployeeIds = await employeeRepository
            .GetAllAttached()
            .Where(e => selectedEmployeeIds.Contains(e.Id) && !e.IsDeleted && !e.User.IsDeleted && e.User.IsSpecialist)
            .Select(e => e.Id)
            .ToListAsync();

        foreach (Guid employeeId in validEmployeeIds)
        {
            await employeeServiceRepository.AddAsync(new GlowCare.Entities.Models.EmployeeService
            {
                EmployeeId = employeeId,
                ServiceId = service.Id
            });
        }
    }

    public async Task DeleteServiceAsync(DeleteServiceViewModel model)
    {
        Service service = await serviceRepository.GetByIdAsync(model.Id);

        if (service == null)
        {
            throw new NullReferenceException("Записът не беше намерен.");
        }

        if (service.IsDeleted)
        {
            throw new ArgumentException("Записът вече е изтрит.");
        }

        service.IsDeleted = true;
        await serviceRepository.UpdateAsync(service);
    }

    public async Task EditServiceAsync(EditServiceViewModel model)
    {
        if (model == null)
        {
            throw new NullReferenceException("Записът не беше намерен.");
        }

        Service? service = await serviceRepository
            .GetAllAttached()
            .Include(s => s.EmployeeServices)
            .FirstOrDefaultAsync(s => s.Id == model.Id && !s.IsDeleted);

        if (service == null)
        {
            throw new NullReferenceException("Записът не беше намерен.");
        }

        service.Name = model.Name;
        service.CategoryId = model.CategoryId;
        service.Description = model.Description;
        service.DurationInMinutes = model.DurationInMinutes;
        service.Price = model.Price;
        service.Points = model.Points;

        IEnumerable<Guid> selectedEmployeeIds = model.SelectedEmployeeIds ?? Array.Empty<Guid>();

        HashSet<Guid> validSelectedEmployeeIds = (await employeeRepository
            .GetAllAttached()
            .Where(e => selectedEmployeeIds.Contains(e.Id) && !e.IsDeleted && !e.User.IsDeleted && e.User.IsSpecialist)
            .Select(e => e.Id)
            .ToListAsync())
            .ToHashSet();

        List<GlowCare.Entities.Models.EmployeeService> assignmentsToRemove = service.EmployeeServices
            .Where(es => !validSelectedEmployeeIds.Contains(es.EmployeeId))
            .ToList();

        if (assignmentsToRemove.Count > 0)
        {
            dbContext.RemoveRange(assignmentsToRemove);
        }

        HashSet<Guid> currentEmployeeIds = service.EmployeeServices
            .Select(es => es.EmployeeId)
            .ToHashSet();

        IEnumerable<Guid> employeeIdsToAdd = validSelectedEmployeeIds
            .Except(currentEmployeeIds);

        foreach (Guid employeeId in employeeIdsToAdd)
        {
            service.EmployeeServices.Add(new GlowCare.Entities.Models.EmployeeService
            {
                EmployeeId = employeeId,
                ServiceId = service.Id
            });
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<EditServiceViewModel> GetEditServiceAsync(int id)
    {
        Service? service = await serviceRepository
            .GetAllAttached()
            .AsNoTracking()
            .Where(s => s.Id == id && !s.IsDeleted)
            .Include(s => s.EmployeeServices)
            .FirstOrDefaultAsync();

        if (service == null)
        {
            throw new NullReferenceException("Записът не беше намерен.");
        }

        EditServiceViewModel model = new()
        {
            Id = service.Id,
            CategoryId = service.CategoryId,
            Name = service.Name,
            Description = service.Description,
            DurationInMinutes = service.DurationInMinutes,
            Price = service.Price,
            Points = service.Points,
            SelectedEmployeeIds = service.EmployeeServices
                .Select(es => es.EmployeeId)
                .Distinct()
                .ToList()
        };

        await PopulateEditServiceLookupDataAsync(model);
        return model;
    }

    public async Task<AdminServiceManagementViewModel> GetAdminServiceManagementViewModelAsync(
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 6,
        AddServiceViewModel? formModel = null)
    {
        pageSize = pageSize <= 0 ? 6 : pageSize;
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;

        string? normalizedSearchTerm = string.IsNullOrWhiteSpace(searchTerm)
            ? null
            : searchTerm.Trim();

        AddServiceViewModel newServiceModel = formModel ?? new AddServiceViewModel();
        HashSet<Guid> selectedEmployeeIds = (newServiceModel.SelectedEmployeeIds ?? Array.Empty<Guid>())
            .Where(id => id != Guid.Empty)
            .ToHashSet();

        newServiceModel.Specialists = await BuildSpecialistOptionsAsync(selectedEmployeeIds);

        IQueryable<Service> servicesQuery = serviceRepository
            .GetAllAttached()
            .AsNoTracking()
            .Where(s => !s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(normalizedSearchTerm))
        {
            servicesQuery = servicesQuery.Where(s => EF.Functions.Like(s.Name, $"%{normalizedSearchTerm}%"));
        }

        int totalServices = await servicesQuery.CountAsync();
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalServices / (double)pageSize));

        if (pageNumber > totalPages)
        {
            pageNumber = totalPages;
        }

        List<Service> services = await servicesQuery
            .Include(s => s.Category)
            .Include(s => s.EmployeeServices)
                .ThenInclude(es => es.Employee)
                    .ThenInclude(e => e.User)
            .OrderBy(s => s.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new AdminServiceManagementViewModel
        {
            NewService = newServiceModel,
            Categories = await GetCategoryOptionsAsync(),
            SearchTerm = normalizedSearchTerm,
            CurrentPage = pageNumber,
            TotalPages = totalPages,
            TotalServices = totalServices,
            Services = services.Select(s => new AdminServiceListItemViewModel
            {
                Id = s.Id,
                Name = s.Name,
                CategoryName = s.Category != null ? s.Category.Name : string.Empty,
                Price = s.Price,
                DurationInMinutes = s.DurationInMinutes,
                Points = s.Points,
                AssignedSpecialists = s.EmployeeServices
                    .Where(es => !es.Employee.IsDeleted && !es.Employee.User.IsDeleted && es.Employee.User.IsSpecialist)
                    .Select(es => $"{es.Employee.User.FirstName} {es.Employee.User.LastName}")
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList()
            })
            .ToList()
        };
    }

    public async Task PopulateEditServiceLookupDataAsync(EditServiceViewModel model)
    {
        model.Categories = await GetCategoryOptionsAsync();
        model.Specialists = await BuildSpecialistOptionsAsync(model.SelectedEmployeeIds ?? Array.Empty<Guid>());
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

    public async Task<IEnumerable<AdminServiceListItemViewModel>> GetAllServicesForAdminAsync()
    {
        List<Service> services = await serviceRepository
            .GetAllAttached()
            .AsNoTracking()
            .Where(s => !s.IsDeleted)
            .Include(s => s.Category)
            .Include(s => s.EmployeeServices)
                .ThenInclude(es => es.Employee)
                    .ThenInclude(e => e.User)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return services.Select(s => new AdminServiceListItemViewModel
        {
            Id = s.Id,
            Name = s.Name,
            CategoryName = s.Category != null ? s.Category.Name : string.Empty,
            Price = s.Price,
            DurationInMinutes = s.DurationInMinutes,
            Points = s.Points,
            AssignedSpecialists = s.EmployeeServices
                .Where(es => !es.Employee.IsDeleted && !es.Employee.User.IsDeleted && es.Employee.User.IsSpecialist)
                .Select(es => $"{es.Employee.User.FirstName} {es.Employee.User.LastName}")
                .Distinct()
                .OrderBy(name => name)
                .ToList()
        })
        .ToList();
    }

    public async Task<IEnumerable<ServiceInfoViewModel>> GetFilteredServicesAsync(
        int? categoryId,
        string? priceRange,
        string? availabilityRange)
    {
        List<Service> services = await serviceRepository
            .GetAllAttached()
            .Where(s => !s.IsDeleted)
            .Include(s => s.Category)
            .ToListAsync();

        List<ServiceInfoViewModel> result = new List<ServiceInfoViewModel>();

        foreach (Service service in services)
        {
            DateTime? earliestSlot = await GetEarliestAvailableSlotForServiceAsync(service.Id, DateTime.Now);

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
            DateTime now = DateTime.Now;
            DateTime todayEnd = now.Date.AddDays(1).AddTicks(-1);
            DateTime next3DaysEnd = now.Date.AddDays(3).AddDays(1).AddTicks(-1);
            DateTime thisWeekEnd = now.Date.AddDays(7).AddDays(1).AddTicks(-1);

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

    private async Task<List<SpecialistOptionViewModel>> BuildSpecialistOptionsAsync(IEnumerable<Guid>? selectedEmployeeIds = null)
    {
        HashSet<Guid> selectedIds = selectedEmployeeIds?
            .Where(id => id != Guid.Empty)
            .ToHashSet() ?? new HashSet<Guid>();

        return await employeeRepository
            .GetAllAttached()
            .AsNoTracking()
            .Include(e => e.User)
            .Where(e => !e.IsDeleted && !e.User.IsDeleted && e.User.IsSpecialist)
            .OrderBy(e => e.User.FirstName)
            .ThenBy(e => e.User.LastName)
            .Select(e => new SpecialistOptionViewModel
            {
                Id = e.Id,
                FullName = $"{e.User.FirstName} {e.User.LastName}",
                IsSelected = selectedIds.Contains(e.Id)
            })
            .ToListAsync();
    }

    private async Task<DateTime?> GetEarliestAvailableSlotForServiceAsync(int serviceId, DateTime from)
    {
        Service? service = await serviceRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(s => s.Id == serviceId && !s.IsDeleted);

        if (service == null)
        {
            return null;
        }

        List<Guid> employeeIds = await employeeServiceRepository
            .GetAllAttached()
            .Where(es => es.ServiceId == serviceId)
            .Select(es => es.EmployeeId)
            .Distinct()
            .ToListAsync();

        if (!employeeIds.Any())
        {
            return null;
        }

        List<Schedule> schedules = await scheduleRepository
            .GetAllAttached()
            .Where(s => employeeIds.Contains(s.EmployeeId))
            .ToListAsync();

        List<Procedure> procedures = await procedureRepository
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
            DateTime currentDate = from.Date.AddDays(dayOffset);

            List<Schedule> daySchedules = schedules
                .Where(s => s.DayOfWeek == currentDate.DayOfWeek)
                .ToList();

            foreach (Schedule schedule in daySchedules)
            {
                if (!TimeSpan.TryParse(schedule.StartTime, out TimeSpan startTime) ||
                    !TimeSpan.TryParse(schedule.EndTime, out TimeSpan endTime))
                {
                    continue;
                }

                DateTime workStart = currentDate.Add(startTime);
                DateTime workEnd = currentDate.Add(endTime);

                DateTime slotStart = workStart;

                if (currentDate == from.Date && from > workStart)
                {
                    slotStart = RoundUpToNext30Minutes(from);
                }

                while (slotStart.AddMinutes(service.DurationInMinutes) <= workEnd)
                {
                    DateTime slotEnd = slotStart.AddMinutes(service.DurationInMinutes);

                    List<Procedure> employeeProcedures = procedures
                        .Where(p => p.EmployeeId == schedule.EmployeeId &&
                                    p.AppointmentDate.Date == currentDate.Date)
                        .ToList();

                    bool hasConflict = employeeProcedures.Any(p =>
                    {
                        if (p.Service == null)
                        {
                            return false;
                        }

                        DateTime existingStart = p.AppointmentDate;
                        DateTime existingEnd = p.AppointmentDate.AddMinutes(p.Service.DurationInMinutes);

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
        int minutesToAdd = 30 - (dateTime.Minute % 30);

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
}
