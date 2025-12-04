using GlowCare.Entities.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace GlowCare.ViewModels.Users;

public class RegisterViewModel
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int Age { get; set; }

    public Gender Gender { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }
}

