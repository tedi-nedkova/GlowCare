namespace GlowCare.ViewModels.Reviews
{
    public class ReviewIndexViewModel
    {
        public Guid EmployeeId { get; set; }
        public string SpecialistName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public IEnumerable<ReviewListItemViewModel> Reviews { get; set; }
            = new List<ReviewListItemViewModel>();
    }
}