using GlowCare.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;

public class ServicesConfiguration
    : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasData(this.GetServices());
    }

    public IEnumerable<Service> GetServices()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/services.json");

        var services = JsonConvert.DeserializeObject<List<Service>>(json)
            ?? throw new Exception("Invalid json file path");

        return services;
    }
}

