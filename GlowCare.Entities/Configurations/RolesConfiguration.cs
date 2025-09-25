using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;

public class RolesConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(this.GetRoles());
    }

    private List<IdentityRole> GetRoles()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/roles.json");

        var roles = JsonConvert.DeserializeObject<List<IdentityRole>>(json)
            ?? throw new Exception("Invalid json file path");

        return roles;
    }
}

