namespace GlowCare.ViewModels.Employees;

public class EmployeeIndexViewModel
{
    public string? SearchTerm { get; set; }

    public string? SelectedService { get; set; }

    public IEnumerable<string> AvailableServices { get; set; } = new List<string>();

    public IEnumerable<EmployeeInfoViewModel> Employees { get; set; } = new List<EmployeeInfoViewModel>();
}
