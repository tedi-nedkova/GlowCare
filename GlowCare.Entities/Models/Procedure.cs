using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GlowCare.Entities.Models.Enums;

namespace GlowCare.Entities.Models;

public class Procedure
{
    [Key]
    [Required]
    public int Id { get; set; }

    public Guid UserId { get; set; } 
    [ForeignKey(nameof(UserId))]
    public GlowUser? User { get; set; }

    public Guid EmployeeId { get; set; } 
    [ForeignKey(nameof(EmployeeId))]
    public Employee? Employee { get; set; }
    public int ServiceId { get; set; }
    [ForeignKey(nameof(ServiceId))]
    public Service? Service { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

 

    public Status? Status { get; set; }

    public string? Notes { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<Review> Reviews { get; set; }
         = new List<Review>();
}

