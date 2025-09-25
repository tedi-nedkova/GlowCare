using GlowCare.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;

public class SchedulesDaysOfWeekConfiguration : IEntityTypeConfiguration<ScheduleDayOfWeek>
{
    public void Configure(EntityTypeBuilder<ScheduleDayOfWeek> builder)
    {
        builder.HasData(this.GetSchedulesDaysOfWeek());
    }

    public IEnumerable<ScheduleDayOfWeek> GetSchedulesDaysOfWeek()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/schedulesDaysOfWeek.json");

        var schedulesDaysOfWeek = JsonConvert.DeserializeObject<List<ScheduleDayOfWeek>>(json)
            ?? throw new Exception("Invalid json file path");

        return schedulesDaysOfWeek;
    }
}

