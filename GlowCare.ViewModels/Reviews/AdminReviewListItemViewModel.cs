namespace GlowCare.ViewModels.Reviews;

public class AdminReviewListItemViewModel
{
    public int Id { get; set; }

    public string AuthorName { get; set; } = string.Empty;

    public string SpecialistName { get; set; } = string.Empty;

    public string ServiceName { get; set; } = string.Empty;

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
