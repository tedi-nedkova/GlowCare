using GlowCare.Entities.Models.Enums;
using GlowCare.ViewModels.Services;

namespace GlowCare.ViewModels.Shared;

public class IndexViewModel
{

    public Guid EmployeeId { get; set; }

    public int ServiceId { get; set; }

    public DateTime AppointmentDate { get; set; }

    public Status Status { get; set; }

    public string? Notes { get; set; }

    public IEnumerable<ServiceInfoViewModel> ServicesInfo { get; set; }
        = new List<ServiceInfoViewModel>();

}
