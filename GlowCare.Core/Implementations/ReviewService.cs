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
    IRepository<Employee, Guid> employeeRepository,
    UserManager<GlowUser> userManager) : IReviewService
{
    public async Task<AddReviewViewModel?> GetAddReviewModelAsync(Guid employeeId, Guid userId, int? procedureId = null)
    {
        Employee? employee = await employeeRepository
            .GetAllAttached()
            .AsNoTracking()
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted);

        if (employee == null)
        {
            return null;
        }

        List<ProcedureOptionViewModel> procedures = await procedureRepository
            .GetAllAttached()
            .AsNoTracking()
            .Where(p => p.EmployeeId == employeeId
                        && p.UserId == userId
                        && !p.IsDeleted
                        && p.AppointmentDate <= DateTime.Now)
            .Select(p => new ProcedureOptionViewModel
            {
                ProcedureId = p.Id,
                ServiceId = p.ServiceId,
                DisplayName = $"{p.Service!.Name} - {p.AppointmentDate:dd.MM.yyyy HH:mm}"
            })
            .OrderByDescending(p => p.ProcedureId)
            .ToListAsync();

        int? selectedProcedureId = procedureId;
        int selectedServiceId = 0;

        if (selectedProcedureId.HasValue)
        {
            ProcedureOptionViewModel? selectedProcedure =
                procedures.FirstOrDefault(p => p.ProcedureId == selectedProcedureId.Value);

            if (selectedProcedure != null)
            {
                selectedServiceId = selectedProcedure.ServiceId;
            }
            else
            {
                selectedProcedureId = null;
            }
        }

        if (!selectedProcedureId.HasValue && procedures.Any())
        {
            ProcedureOptionViewModel firstProcedure = procedures.First();
            selectedProcedureId = firstProcedure.ProcedureId;
            selectedServiceId = firstProcedure.ServiceId;
        }

        return new AddReviewViewModel
        {
            EmployeeId = employee.Id,
            ProcedureId = selectedProcedureId,
            SpecialistName = $"{employee.User!.FirstName} {employee.User.LastName}",
            ServiceId = selectedServiceId,
            AvailableProcedures = procedures
        };
    }

    public async Task CreateReviewAsync(AddReviewViewModel model, Guid userId)
    {
        GlowUser? user = await userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new NullReferenceException("Невалидно user id");
        }

        Employee? employee = await employeeRepository
            .GetAllAttached()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == model.EmployeeId && !e.IsDeleted);

        if (employee == null)
        {
            throw new NullReferenceException("Специалиста не е намерен");
        }

        if (!model.ProcedureId.HasValue)
        {
            throw new InvalidOperationException("Моля, избери процедура.");
        }

        Procedure? procedure = await procedureRepository
            .GetAllAttached()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == model.ProcedureId.Value
                                      && p.UserId == userId
                                      && p.EmployeeId == model.EmployeeId
                                      && !p.IsDeleted);

        if (procedure == null)
        {
            throw new InvalidOperationException("Невалидна процедура за този потребител и специалист.");
        }

        Review review = new Review
        {
            Comment = model.Comment,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            UserId = userId,
            Rating = model.Rating,
            EmployeeId = model.EmployeeId,
            ProcedureId = procedure.Id,
            ServiceId = procedure.ServiceId
        };

        await reviewRepository.AddAsync(review);
    }

    public async Task<ReviewIndexViewModel?> GetReviewsByEmployeeIdAsync(Guid employeeId)
    {
        Employee? employee = await employeeRepository
            .GetAllAttached()
            .AsNoTracking()
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted);

        if (employee == null)
        {
            return null;
        }

        List<ReviewListItemViewModel> reviews = await reviewRepository
            .GetAllAttached()
            .AsNoTracking()
            .Where(r => r.EmployeeId == employeeId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewListItemViewModel
            {
                AuthorName = r.User != null
                    ? $"{r.User.FirstName} {r.User.LastName}"
                    : "Потребител",
                ServiceName = r.Service != null
                    ? r.Service.Name
                    : "Процедура",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        double averageRating = reviews
            .Select(r => (double?)r.Rating)
            .Average() ?? 0d;

        return new ReviewIndexViewModel
        {
            EmployeeId = employee.Id,
            SpecialistName = $"{employee.User!.FirstName} {employee.User.LastName}",
            AverageRating = averageRating,
            ReviewsCount = reviews.Count,
            Reviews = reviews
        };
    }

    public async Task<IEnumerable<AdminReviewListItemViewModel>> GetAllReviewsForAdminAsync()
    {
        return await reviewRepository
            .GetAllAttached()
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .Include(r => r.User)
            .Include(r => r.Employee)
                .ThenInclude(e => e.User)
            .Include(r => r.Service)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new AdminReviewListItemViewModel
            {
                Id = r.Id,
                AuthorName = r.User != null
                    ? $"{r.User.FirstName} {r.User.LastName}"
                    : "Потребител",
                SpecialistName = r.Employee != null && r.Employee.User != null
                    ? $"{r.Employee.User.FirstName} {r.Employee.User.LastName}"
                    : "Специалист",
                ServiceName = r.Service != null
                    ? r.Service.Name
                    : "Услуга",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task SoftDeleteReviewAsync(int reviewId)
    {
        Review review = await reviewRepository.GetByIdAsync(reviewId);

        if (review.IsDeleted)
        {
            throw new InvalidOperationException("Ревюто вече е изтрито.");
        }

        review.IsDeleted = true;
        await reviewRepository.UpdateAsync(review);
    }
}