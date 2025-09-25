using GlowCare.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;

public class DaysOfWeekConfiguration : IEntityTypeConfiguration<Models.DayOfWeek>
{
    public void Configure(EntityTypeBuilder<Models.DayOfWeek> builder)
    {
        builder.HasData(this.GetDaysOfWeek());
    }

    public IEnumerable<Models.DayOfWeek> GetDaysOfWeek()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/daysOfWeek.json");

        var daysOfWeek = JsonConvert.DeserializeObject<List<Models.DayOfWeek>>(json)
            ?? throw new Exception("Invalid json file path");

        return daysOfWeek;
    }
}

