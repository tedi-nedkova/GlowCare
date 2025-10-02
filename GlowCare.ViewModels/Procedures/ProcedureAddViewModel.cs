using GlowCare.Entities.Models.Enums;

namespace GlowCare.ViewModels.Procedures;

public class ProcedureAddViewModel
{
    public string ClientId { get; set; } = string.Empty;

    public string EmployeeId { get; set; } = string.Empty;

    public int ServiceId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public Status Status { get; set; }

    public string? Notes { get; set; }
}

