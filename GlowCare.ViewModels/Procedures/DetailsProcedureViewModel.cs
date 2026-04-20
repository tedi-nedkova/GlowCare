namespace GlowCare.ViewModels.Procedures;

public class DetailsProcedureViewModel
{
    public required int Id { get; set; }
    public required Guid EmployeeId { get; set; }
    public required string SpecialistName { get; set; }
    public required string Service { get; set; }
    public required decimal Price { get; set; }
    public required string AppointmentDate { get; set; }
    public required string Status { get; set; }
    public int EarnedPoints { get; set; }
}

