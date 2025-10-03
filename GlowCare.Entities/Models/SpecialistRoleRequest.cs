using GlowCare.Entities.Models.Enums;
using System.ComponentModel.DataAnnotations;
using static GlowCare.Common.Constants.RoleRequestConstants;

namespace GlowCare.Entities.Models;

public class SpecialistRoleRequest
{
    [Key]
    public int Id { get; set; }

    [MaxLength(SenderMaxLength)]
    [Required]
    public string Sender { get; set; } = null!;

    [Required]
    public string SenderId { get; set; } = null!;

    [MaxLength(DescriptionMaxLength)]
    [Required]
    public string Description { get; set; } = null!;

    public RequestStatus Status { get; set; }
}

