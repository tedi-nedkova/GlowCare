using GlowCare.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;

public class ProceduresConfiguration
    : IEntityTypeConfiguration<Procedure>
{
    public void Configure(EntityTypeBuilder<Procedure> builder)
    {
        builder.HasData(this.GetProcedure());
    }

    public IEnumerable<Procedure> GetProcedure()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/procedures.json");

        var procedures = JsonConvert.DeserializeObject<List<Procedure>>(json)
            ?? throw new Exception("Invalid json file path");

        return procedures;
    }
}

