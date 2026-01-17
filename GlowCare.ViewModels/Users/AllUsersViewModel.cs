namespace GlowCare.ViewModels.Users;

public class AllUsersViewModel
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public IList<string> Roles { get; set; }
    = new List<string>();

}

