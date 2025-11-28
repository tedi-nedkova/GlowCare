namespace GlowCare.ViewModels.Users;

public class UserManagementViewModel
{
    public IEnumerable<AllUsersViewModel> Users { get; set; }
       = new List<AllUsersViewModel>();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

