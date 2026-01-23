using GlowCare.Entities.Models.Enums;

namespace GlowCare.ViewModels.Procedures;

public class AddProcedureViewModel
{

    public Guid EmployeeId { get; set; } 

    public int ServiceId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public Status Status { get; set; }

    public string? Notes { get; set; }
}

