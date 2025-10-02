namespace GlowCare.ViewModels.Procedures;

public class ProcedureEditViewModel
{
    public required string EmployeeId { get; set; } 

    public int ServiceId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public string? Notes { get; set; }
}

