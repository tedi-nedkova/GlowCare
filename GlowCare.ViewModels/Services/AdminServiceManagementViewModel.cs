namespace GlowCare.ViewModels.Services;

public class AdminServiceManagementViewModel
{
    public AddServiceViewModel NewService { get; set; } = new();

    public IEnumerable<ServiceCategoryOptionViewModel> Categories { get; set; }
        = new List<ServiceCategoryOptionViewModel>();

    public IEnumerable<AdminServiceListItemViewModel> Services { get; set; }
        = new List<AdminServiceListItemViewModel>();

    public string? SearchTerm { get; set; }

    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; } = 1;

    public int TotalServices { get; set; }
}
