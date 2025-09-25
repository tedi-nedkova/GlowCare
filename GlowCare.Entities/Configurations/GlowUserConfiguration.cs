using GlowCare.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;



public class ApplicationUserConfiguration : IEntityTypeConfiguration<GlowUser>
{
    public void Configure(EntityTypeBuilder<GlowUser> builder)
    {
        builder.HasData(this.GetUsers());
    }
    public IEnumerable<GlowUser> GetUsers()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/users.json");

        var users = JsonConvert.DeserializeObject<List<GlowUser>>(json)
            ?? throw new Exception("Invalid json file path");

        return users;
    }
}
