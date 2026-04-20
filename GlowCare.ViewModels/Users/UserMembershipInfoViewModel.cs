namespace GlowCare.ViewModels.Users;

public class UserMembershipInfoViewModel
{
    public string Title { get; set; } = null!;
    public int DiscountPercentage { get; set; }
    public int RequiredPoints { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsUnlocked { get; set; }
}
