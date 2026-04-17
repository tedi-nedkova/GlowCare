using GlowCare.Core.Implementations;
using GlowCare.ViewModels.Employees;

namespace GlowCare.Core.Contracts
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeInfoViewModel>> GetAllEmployeesAsync();

        Task<EmployeeIndexViewModel> GetEmployeesForIndexAsync(string? searchTerm, string? selectedService);

        Task<EmployeeInfoViewModel?> GetEmployeeByIdAsync(Guid employeeId);
    }
}
