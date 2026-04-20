namespace GlowCare.ViewModels.Services;

public class AdminServiceListItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int DurationInMinutes { get; set; }

    public int Points { get; set; }

    public IEnumerable<string> AssignedSpecialists { get; set; }
        = new List<string>();
}
