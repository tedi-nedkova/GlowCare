using System.ComponentModel.DataAnnotations;

namespace GlowCare.Entities.Models;

public class Service
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Category { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Required]
    public int DurationInMinutes { get; set; }

    [Required]
    public decimal Price { get; set; }

    public ICollection<Procedure> Procedures { get; set; }
        = new List<Procedure>();
}

