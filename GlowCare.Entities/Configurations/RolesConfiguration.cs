using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel;

namespace GlowCare.Entities.Configurations;

public class RolesConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData(this.GetRoles());
    }

    private List<IdentityRole<Guid>> GetRoles()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/roles.json");

        var roles = JsonConvert.DeserializeObject<List<IdentityRole<Guid>>>(json)
            ?? throw new Exception("Invalid json file path");

        return roles;
    }
}

