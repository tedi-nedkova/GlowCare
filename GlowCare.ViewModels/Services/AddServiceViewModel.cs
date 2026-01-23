using GlowCare.Entities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace GlowCare.ViewModels.Services;

public class AddServiceViewModel
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int DurationInMinutes { get; set; }

    public decimal Price { get; set; }

    public int Points { get; set; }
}

