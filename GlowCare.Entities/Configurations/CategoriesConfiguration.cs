using GlowCare.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;

public class CategoriesConfiguration
    : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasData(this.GetCategories());
    }

    public IEnumerable<Category> GetCategories()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/categories.json");

        var categories = JsonConvert.DeserializeObject<List<Category>>(json)
            ?? throw new Exception("Invalid json file path");

        return categories;
    }
}

