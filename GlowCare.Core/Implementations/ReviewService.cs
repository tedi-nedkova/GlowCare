using GlowCare.Core.Contracts;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Reviews;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GlowCare.Core.Implementations;

public class ReviewService(
    IRepository<Review, int> reviewRepository,
        UserManager<GlowUser> userManager) : IReviewService
{
    public async Task CreateReviewAsync(AddReviewViewModel model, Guid userId)
    {
        GlowUser? user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new NullReferenceException("Invalid user id");
        }

        if (model == null)
        {
            throw new NullReferenceException("Entity not found");
        }

        var review = new Review()
        {
            Comment = model.Comment,
            CreatedAt = DateTime.UtcNow,
            PublisherId = model.PublisherId,
            StarRating = model.StarRating,
            JobId = model.JobId,
            CourseId = model.CourseId
        };

        await reviewRepository.AddAsync(review);

        user.Reviews.Add(review);
    }


}

