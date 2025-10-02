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

        var hasher = new PasswordHasher<GlowUser>();

        foreach (var user in users)
        {
            if (user.Id == "a7d3c5e2-9b41-4f12-8f34-123456789abc" ||
                user.Id == "ac31b0bb-d05a-438d-be06-9bfe3323cf08" ||
                user.Id == "29965aaa-46cf-4829-93b8-e38401be7547" ||
                user.Id == "c9f4e7b1-2d33-4a11-8f56-abcdef123456")
            {
                user.PasswordHash = hasher.HashPassword(user, "Employee_123");
            }
            else if (user.Id == "e5c2g9b3-4a67-4f89-8d23-556677889900")
            {
                user.PasswordHash = hasher.HashPassword(user, "Client_123");
            }
            else if (user.Id == "fc95b3fa-f342-4172-ac8b-5b35951ad760")
            {
                user.PasswordHash = hasher.HashPassword(user, "Admin_123");
            }
        }

        return users;
    }
}
