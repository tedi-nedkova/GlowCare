using GlowCare.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;

public class EmployeesServiceConfiguration: IEntityTypeConfiguration<EmployeeService>
{
    public void Configure(EntityTypeBuilder<EmployeeService> builder)
    {
        builder.HasData(this.GetEmployeesServices());
    }

    public IEnumerable<EmployeeService> GetEmployeesServices()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/employeesServices.json");

        var employeesServices = JsonConvert.DeserializeObject<List<EmployeeService>>(json)
            ?? throw new Exception("Invalid json file path");

        return employeesServices;
    }
}

