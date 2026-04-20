namespace GlowCare.ViewModels.Users;

public class UserProfileProcedureViewModel
{
    public int Id { get; set; }
    public string ServiceName { get; set; } = null!;
    public string SpecialistName { get; set; } = null!;
    public string? ClientName { get; set; }
    public decimal Price { get; set; }
    public int EarnedPoints { get; set; }
    public string AppointmentDate { get; set; } = null!;
    public string Status { get; set; } = null!;
    public bool CanBeRejectedBySpecialist { get; set; }
}
