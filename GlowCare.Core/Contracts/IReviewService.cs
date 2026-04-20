using GlowCare.ViewModels.Reviews;

namespace GlowCare.Core.Contracts
{
    public interface IReviewService
    {
        Task<AddReviewViewModel?> GetAddReviewModelAsync(Guid employeeId, Guid userId, int? procedureId = null);

        Task CreateReviewAsync(AddReviewViewModel model, Guid userId);

        Task<ReviewIndexViewModel?> GetReviewsByEmployeeIdAsync(Guid employeeId);

        Task<IEnumerable<AdminReviewListItemViewModel>> GetAllReviewsForAdminAsync();

        Task SoftDeleteReviewAsync(int reviewId);
    }
}