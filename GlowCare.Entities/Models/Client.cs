using System.ComponentModel.DataAnnotations.Schema;

namespace GlowCare.Entities.Models;

public class Client : GlowUser
{
    public ICollection<Review> Reviews { get; set; }
    = new List<Review>();

    public int MembershipId { get; set; }
    [ForeignKey(nameof(MembershipId))]
    public Membership? Membership { get; set; } 

    public ICollection<Procedure> Procedures { get; set; }
        = new List<Procedure>();
}

