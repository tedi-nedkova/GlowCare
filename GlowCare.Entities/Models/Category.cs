using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;
using static GlowCare.Common.Constants.CategoryConstants;

namespace GlowCare.Entities.Models;

public class Category
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    [MinLength(CategoryNameMinLength)]
    [MaxLength(CategoryNameMaxLength)]
    public string Name { get; set; } = null!;

    public ICollection<Service> Services { get; set; }
        = new List<Service>();
}

