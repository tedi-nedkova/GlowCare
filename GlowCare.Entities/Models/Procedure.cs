using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GlowCare.Entities.Models.Enums;

namespace GlowCare.Entities.Models;

public class Procedure
{
    [Key]
    [Required]
    public int Id { get; set; }

    public string ClientId { get; set; } = null!;
    [ForeignKey(nameof(ClientId))]
    public Client? Client { get; set; }

    public string EmployeeId { get; set; } = null!;
    [ForeignKey(nameof(EmployeeId))]
    public Employee? Employee { get; set; }
    public int ServiceId { get; set; }
    [ForeignKey(nameof(ServiceId))]
    public Service? Service { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    public Status? Status { get; set; }

    public string? Notes { get; set; }

    public ICollection<Review> Reviews { get; set; }
         = new List<Review>();
}

