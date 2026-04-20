namespace GlowCare.ViewModels.Reviews;

public class AdminReviewManagementViewModel
{
    public IEnumerable<AdminReviewListItemViewModel> Reviews { get; set; }
        = new List<AdminReviewListItemViewModel>();
}
