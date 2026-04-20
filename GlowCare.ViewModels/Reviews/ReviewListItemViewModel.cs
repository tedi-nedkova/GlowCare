namespace GlowCare.ViewModels.Reviews
{
    public class ReviewListItemViewModel
    {
        public string AuthorName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}