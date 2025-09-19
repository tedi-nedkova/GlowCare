using System.ComponentModel.DataAnnotations;

namespace GlowCare.Entities.Models;

public class Certificate
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required]
    public string Url { get; set; } = null!;

    [Required]
    public DateTime CertificateDate { get; set; }
}

