using GlowCare.Core.Contracts;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.ViewModels.Reviews;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GlowCare.Core.Implementations;

public class ReviewService(
    IRepository<Review, int> reviewRepository,
    IRepository<Procedure, int> procedureRepository,
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

        Procedure? procedure = await procedureRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(p => p.Id == model.ProcedureId);

        if (procedure == null)
        {
            throw new NullReferenceException("Procedure not found");
        }

        var review = new Review()
        {
            Comment = model.Comment,
            CreatedAt = DateTime.UtcNow,
            UserId = userId,
            Rating = model.Rating,
            EmployeeId = procedure.EmployeeId,
            ProcedureId = model.ProcedureId
        };

        await reviewRepository.AddAsync(review);

        user.Reviews.Add(review);
    }

    public async Task<IEnumerable<Review>> GetReviewsByServiceIdAsync(int serviceId)
    {
        return await reviewRepository
            .GetAllAttached()
            .Include(r => r.User)
            .Include(r => r.Employee)
            .Include(r => r.Procedure)
            .Where(r => r.Procedure.ServiceId == serviceId)
            .ToListAsync();
    }
}