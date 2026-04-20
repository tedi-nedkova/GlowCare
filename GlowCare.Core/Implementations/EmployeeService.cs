using GlowCare.Core.Contracts;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Employees;
using Microsoft.EntityFrameworkCore;

namespace GlowCare.Core.Implementations
{
    public class EmployeeService(
    IRepository<Employee, Guid> employeeRepository,
    IRepository<GlowCare.Entities.Models.EmployeeService, int> employeeServiceRepository,
    IRepository<Review, int> reviewRepository)
    : IEmployeeService
    {
        public async Task<EmployeeInfoViewModel?> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await employeeRepository
                .GetAllAttached()
                .Where(e => e.Id == id && !e.IsDeleted)
                .Include(e => e.User)
                .Include(e => e.EmployeeServices)
                    .ThenInclude(es => es.Service)
                .Include(e => e.Schedules)
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return null;
            }

            List<Review> reviews = await reviewRepository
                .GetAllAttached()
                .AsNoTracking()
                .Where(r => r.EmployeeId == id && !r.IsDeleted)
                .ToListAsync();

            double averageRating = reviews
                .Select(r => (double?)r.Rating)
                .Average() ?? 0d;

            int reviewsCount = reviews.Count;

            return new EmployeeInfoViewModel
            {
                Id = employee.Id,
                UserId = employee.UserId,
                FullName = $"{employee.User.FirstName} {employee.User.LastName}",
                Occupation = employee.Occupation,
                ExperienceYears = employee.ExperienceYears,
                Biography = employee.Biography,
                Email = employee.User.Email,
                PhoneNumber = employee.User.PhoneNumber,
                AverageRating = averageRating,
                ReviewsCount = reviewsCount,
                Services = employee.EmployeeServices
                    .Where(es => !es.Service.IsDeleted)
                    .Select(es => es.Service.Name)
                    .ToList(),
                WorkingHours = employee.Schedules
                    .OrderBy(s => (int)s.DayOfWeek)
                    .Select(s => new EmployeeScheduleViewModel
                    {
                        DayOfWeek = GetDayNameInBulgarian(s.DayOfWeek),
                        StartTime = s.StartTime,
                        EndTime = s.EndTime
                    })
                    .ToList()
            };
        }

        public async Task<EmployeeIndexViewModel> GetEmployeesForIndexAsync(string? searchTerm, string? selectedService)
        {
            searchTerm = searchTerm?.Trim();
            selectedService = selectedService?.Trim();

            List<Employee> employees = await employeeRepository
                .GetAllAttached()
                .AsNoTracking()
                .Include(e => e.User)
                .Where(e => !e.IsDeleted && !e.User.IsDeleted && e.User.IsSpecialist)
                .OrderBy(e => e.User.FirstName)
                .ThenBy(e => e.User.LastName)
                .ToListAsync();

            Dictionary<Guid, List<string>> servicesByEmployee =
                await GetServicesByEmployeeAsync(employees.Select(e => e.Id));

            List<EmployeeInfoViewModel> specialists = employees
                .Select(employee => new EmployeeInfoViewModel
                {
                    Id = employee.Id,
                    UserId = employee.UserId,
                    FullName = $"{employee.User.FirstName} {employee.User.LastName}",
                    Occupation = employee.Occupation,
                    ExperienceYears = employee.ExperienceYears,
                    Biography = employee.Biography,
                    Email = employee.User.Email,
                    PhoneNumber = employee.User.PhoneNumber,
                    Services = servicesByEmployee.TryGetValue(employee.Id, out List<string>? services)
                        ? services
                        : new List<string>()
                })
                .ToList();

            List<string> availableServices = specialists
                .SelectMany(e => e.Services)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            IEnumerable<EmployeeInfoViewModel> filtered = specialists;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filtered = filtered.Where(e =>
                    e.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(selectedService))
            {
                filtered = filtered.Where(e =>
                    e.Services.Any(s => s.Equals(selectedService, StringComparison.OrdinalIgnoreCase)));
            }

            return new EmployeeIndexViewModel
            {
                SearchTerm = searchTerm,
                SelectedService = selectedService,
                AvailableServices = availableServices,
                Employees = filtered.ToList()
            };
        }

        private async Task<Dictionary<Guid, List<string>>> GetServicesByEmployeeAsync(IEnumerable<Guid> employeeIds)
        {
            List<Guid> ids = employeeIds
                .Distinct()
                .ToList();

            if (ids.Count == 0)
            {
                return new Dictionary<Guid, List<string>>();
            }

            var employeeServices = await employeeServiceRepository
                .GetAllAttached()
                .Where(es => ids.Contains(es.EmployeeId) && !es.Service.IsDeleted)
                .Select(es => new
                {
                    es.EmployeeId,
                    ServiceName = es.Service.Name
                })
                .ToListAsync();

            return employeeServices
                .GroupBy(es => es.EmployeeId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ServiceName)
                          .Distinct()
                          .ToList());
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
}
