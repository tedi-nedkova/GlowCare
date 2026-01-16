using GlowCare.ViewModels.Reviews;

namespace GlowCare.Core.Contracts;

public interface IReviewService
{
    Task CreateReviewAsync(AddReviewViewModel model, Guid userId);
}

