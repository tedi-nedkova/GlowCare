using GlowCare.ViewModels.Employees;

namespace GlowCare.Core.Contracts
{
    public interface IEmployeeService
    {

        Task<EmployeeIndexViewModel> GetEmployeesForIndexAsync(string? searchTerm, string? selectedService);

        Task<EmployeeInfoViewModel?> GetEmployeeByIdAsync(Guid employeeId);
    }
}
