using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlowCare.Entities.Models;

[PrimaryKey(nameof(EmployeeId), nameof(ServiceId))]
public class EmployeeService
{
    public string EmployeeId { get; set; } = null!;
    [ForeignKey(nameof(EmployeeId))]
    public Employee Employee { get; set; } = null!;

    public int ServiceId { get; set; }
    [ForeignKey(nameof(ServiceId))]
    public Service Service { get; set; } = null!;
}

