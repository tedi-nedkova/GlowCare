using GlowCare.ViewModels.SpecialistRequest;

namespace GlowCare.Core.Contracts;

public interface ISpecialistApplicationService
{
    Task<bool> UserHasPendingApplicationAsync(Guid userId);

    Task<bool> UserIsAlreadySpecialistAsync(Guid userId);

    Task<ApplySpecialistViewModel?> GetApplicationDraftAsync(Guid userId);

    Task ApplyAsync(Guid userId, ApplySpecialistViewModel model);

    Task<IEnumerable<SpecialistApplicationViewModel>> GetPendingApplicationsAsync();

    Task<SpecialistApplicationViewModel?> GetByIdAsync(int id);

    Task<SpecialistApplicationViewModel?> GetLatestByUserIdAsync(Guid userId);

    Task ApproveAsync(int id);

    Task DeclineAsync(int id, string? rejectionReason);
}