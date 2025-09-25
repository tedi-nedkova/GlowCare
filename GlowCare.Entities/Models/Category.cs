using System.ComponentModel.DataAnnotations;

namespace GlowCare.Entities.Models;

public class Category
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;
}

