using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlowCare.Entities.Models;

public class Service
{
    [Key]
    [Required]
    public int Id { get; set; }


    public int CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Required]
    public int DurationInMinutes { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int Points { get; set; }

    public ICollection<Procedure> Procedures { get; set; }
        = new List<Procedure>();
}

