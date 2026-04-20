using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;

public class UserRolesConfiguration
    : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
    {
        builder.HasData(this.GetUserRoles());
    }

    public IEnumerable<IdentityUserRole<Guid>> GetUserRoles()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/userRoles.json");

        var userRoles = JsonConvert.DeserializeObject<List<IdentityUserRole<Guid>>>(json)
            ?? throw new Exception("Invalid json file path");

        return userRoles;
    }
}