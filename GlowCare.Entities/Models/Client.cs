using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlowCare.Entities.Models;

public class Client 
{
    [Key]
    [Required]
    public string Id { get; set; } = null!;

    public ICollection<Review> Reviews { get; set; }
    = new List<Review>();

    public int MembershipId { get; set; }
    [ForeignKey(nameof(MembershipId))]
    public Membership? Membership { get; set; } 

    public ICollection<Procedure> Procedures { get; set; }
        = new List<Procedure>();

    public string UserId { get; set; } = null!;
    [ForeignKey(nameof(UserId))]
    public GlowUser User { get; set; } = null!;

}

