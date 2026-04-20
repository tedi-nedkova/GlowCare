using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static GlowCare.Common.Constants.ReviewConstants;

namespace GlowCare.Entities.Models;

public class Review
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

    [Range(MinRating, MaxRating)]
    public int Rating { get; set; }

    [Required]
    [MinLength(CommentMinLength)]
    [MaxLength(CommentMaxLength)]
    public string Comment { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }

    public int ServiceId { get; set; }
    [ForeignKey(nameof(ServiceId))]
    public Service? Service { get; set; }

    public int? ProcedureId { get; set; }
    [ForeignKey(nameof(ProcedureId))]
    public Procedure? Procedure { get; set; }
}

