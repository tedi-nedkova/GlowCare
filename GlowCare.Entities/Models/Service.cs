using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static GlowCare.Common.Constants.ServiceConstants;

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
    [MinLength(NameMinLength)]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; } 

    [Required]
    [Range(MinDurationInMinutes, MaxDurationInMinutes)]
    public int DurationInMinutes { get; set; }

    [Required]
    [Range(MinPrice, MaxPrice)]
    public decimal Price { get; set; }

    [Required]
    public int Points { get; set; }

    public ICollection<Procedure> Procedures { get; set; }
        = new List<Procedure>();
}

