using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static GlowCare.Common.Constants.ReviewConstants;

namespace GlowCare.Entities.Models;

public class Review
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

    [Range(MinRating, MaxRating)]
    public int? Rating { get; set; }

    [Required]
    [MinLength(CommentMinLength)]
    [MaxLength(CommentMaxLength)]
    public string Comment { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ProcedureId { get; set; } 
    [ForeignKey(nameof(ProcedureId))]
    public Procedure? Procedure { get; set; }
}

