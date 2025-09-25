using GlowCare.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GlowCare.Entities.Configurations;

public class ReviewsConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
       builder.HasData(this.GetReviews());
    }

    public IEnumerable<Review> GetReviews()
    {
        var workingDirectory = Environment.CurrentDirectory;
        var projectDirectory = Directory.GetParent(workingDirectory);
        var json = File.ReadAllText($"{projectDirectory}/GlowCare.Entities/Data/reviews.json");

        var reviews = JsonConvert.DeserializeObject<List<Review>>(json)
            ?? throw new Exception("Invalid json file path");

        return reviews;
    }
}
