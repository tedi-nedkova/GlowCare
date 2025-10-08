using GlowCare.Entities.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace GlowCare.ViewModels.Roles;

    public class RoleRequestSentViewModel
    {
    public int Id { get; set; }

    public string Sender { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Status { get; set; } = RequestStatus.Pending.ToString();
}

