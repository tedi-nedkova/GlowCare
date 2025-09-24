using GlowCare.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;
public class MembershipsConfiguration 
    : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.HasData(this.GetMemberships());
    }

    public IEnumerable<Membership> GetMemberships()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/memberships.json");

        var memberships = JsonConvert.DeserializeObject<List<Membership>>(json)
            ?? throw new Exception("Invalid json file path");

        return memberships;
    }
}

