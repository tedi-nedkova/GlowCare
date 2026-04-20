namespace GlowCare.ViewModels.Users;

public class UserProfileViewModel
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int Age { get; set; }
    public string Gender { get; set; } = null!;
    public int TotalPoints { get; set; }
    public string CurrentMembershipTitle { get; set; } = null!;
    public int CurrentMembershipDiscountPercentage { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsSpecialist { get; set; }
    public List<UserProfileProcedureViewModel> Procedures { get; set; } = new();
    public List<UserMembershipInfoViewModel> Memberships { get; set; } = new();
}
