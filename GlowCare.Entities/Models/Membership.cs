using System.ComponentModel.DataAnnotations;

namespace GlowCare.Entities.Models;

public class Membership
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public MembershipTitle Title { get; set; }

    [Required]
    public int DiscountPercentage { get; set; }

    public ICollection<Client> Clients { get; set; }
        = new List<Client>();
}

