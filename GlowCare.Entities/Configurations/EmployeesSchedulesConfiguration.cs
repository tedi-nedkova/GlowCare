using GlowCare.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;

public class EmployeesSchedulesConfiguration : IEntityTypeConfiguration<EmployeeSchedule>
{
    public void Configure(EntityTypeBuilder<EmployeeSchedule> builder)
    {
        builder.HasData(this.GetEmployeesSchedules());
    }

    public IEnumerable<EmployeeSchedule> GetEmployeesSchedules()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/employeesSchedules.json");

        var employeesSchedules = JsonConvert.DeserializeObject<List<EmployeeSchedule>>(json)
            ?? throw new Exception("Invalid json file path");

        return employeesSchedules;
    }
}

