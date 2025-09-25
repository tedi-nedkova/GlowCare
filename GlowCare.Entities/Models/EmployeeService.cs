using System.ComponentModel.DataAnnotations.Schema;

namespace GlowCare.Entities.Models;

public class EmployeeService
{
    public string EmployeeId { get; set; } = null!;
    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    public string ServiceId { get; set; } = null!;
    [ForeignKey(nameof(ServiceId))]
    public Service Service { get; set; } = null!;
}

