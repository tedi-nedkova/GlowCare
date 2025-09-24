using GlowCare.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;

public class EmployeesConfiguration 
    : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
       
    }

    public IEnumerable<Employee> GetEmployees()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/employees.json");

        var employees = JsonConvert.DeserializeObject<List<Employee>>(json)
            ?? throw new Exception("Invalid json file path");

        return employees;
    } 
}

