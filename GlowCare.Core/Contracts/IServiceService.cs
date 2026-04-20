using GlowCare.ViewModels.Services;

namespace GlowCare.Core.Contracts;

public interface IServiceService
{
    Task CreateServiceAsync(
        AddServiceViewModel model,
        Guid userId);

    Task DeleteServiceAsync(
        DeleteServiceViewModel model);

    Task EditServiceAsync(
        EditServiceViewModel model);

    Task<EditServiceViewModel> GetEditServiceAsync(
        int id);

    Task<AdminServiceManagementViewModel> GetAdminServiceManagementViewModelAsync(
        string? searchTerm = null,
        int pageNumber = 1,
        int pageSize = 6,
        AddServiceViewModel? formModel = null);

    Task PopulateEditServiceLookupDataAsync(
        EditServiceViewModel model);

    Task<IEnumerable<ServiceInfoViewModel>> GetAllServicesAsync();

    Task<IEnumerable<AdminServiceListItemViewModel>> GetAllServicesForAdminAsync();

    Task<IEnumerable<ServiceInfoViewModel>> GetFilteredServicesAsync(
        int? categoryId,
        string? priceRange,
        string? availabilityRange);

    Task<IEnumerable<ServiceCategoryOptionViewModel>> GetCategoryOptionsAsync();

}
