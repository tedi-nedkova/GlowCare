using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    public int? Rating { get; set; }

    [Required]
    public string Comment { get; set; } = null!;

    [Required]
    public DateTime CreatedAt { get; set; }
}

