namespace GlowCare.ViewModels.Services;

public class ServiceIndexViewModel
{
    public int? SelectedCategoryId { get; set; }

    public string? SelectedPriceRange { get; set; }

    public string? SelectedAvailabilityRange { get; set; }

    public IEnumerable<ServiceCategoryOptionViewModel> Categories { get; set; }
        = new List<ServiceCategoryOptionViewModel>();

    public IEnumerable<ServiceInfoViewModel> Services { get; set; }
        = new List<ServiceInfoViewModel>();
}