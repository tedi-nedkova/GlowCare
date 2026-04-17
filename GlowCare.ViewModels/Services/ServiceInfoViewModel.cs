namespace GlowCare.ViewModels.Services;

public class ServiceInfoViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public int DurationInMinutes { get; set; }

    public DateTime? EarliestAvailableSlot { get; set; }
}