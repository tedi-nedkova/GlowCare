namespace GlowCare.ViewModels.Roles;
public class AdminRoleRequestViewModel
{
    public IEnumerable<RoleRequestInfoViewModel> SpecialistRoleRequests{ get; set; }
        = new List<RoleRequestInfoViewModel>();
}
