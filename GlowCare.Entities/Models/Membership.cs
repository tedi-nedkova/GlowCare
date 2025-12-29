using System.ComponentModel.DataAnnotations;
using GlowCare.Entities.Models.Enums;
using static GlowCare.Common.Constants.MembershipConstants;

namespace GlowCare.Entities.Models;

public class Membership
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public MembershipTitle Title { get; set; }

    [Required]
    [Range(MinDiscountProcentage, MaxDiscountProcentage)]
    public int DiscountPercentage { get; set; }

    [Required]
    public int Points { get; set; }

    public ICollection<GlowUser> Users { get; set; }
        = new List<GlowUser>();
}

