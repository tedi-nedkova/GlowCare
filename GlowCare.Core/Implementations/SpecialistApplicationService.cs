using GlowCare.Core.Contracts;
using GlowCare.Core.Helpers;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.Entities.Models.Enums;
using GlowCare.ViewModels.SpecialistRequest;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GlowCare.Services.Implementations;

public class SpecialistApplicationService(
        IRepository<SpecialistApplication, int> specialistApplicationRepository,
        IRepository<Employee, Guid> employeeRepository,
        IRepository<GlowUser, Guid> userRepository,
        UserManager<GlowUser> userManager)
        : ISpecialistApplicationService
{
    public async Task<IEnumerable<SpecialistApplicationViewModel>> GetPendingApplicationsAsync()
    {
        return await specialistApplicationRepository
            .GetAllAttached()
            .Include(a => a.User)
            .Where(a => a.Status == RequestStatus.Pending)
            .Select(a => new SpecialistApplicationViewModel
            {
                Id = a.Id,
                UserId = a.UserId,
                UserFullName = $"{a.User.FirstName} {a.User.LastName}",
                Email = a.User.Email!,
                Occupation = a.Occupation,
                ExperienceYears = a.ExperienceYears,
                Biography = a.Biography,
                Status = a.Status,
                StatusText = BulgarianTextHelper.GetRequestStatusText(a.Status),
                CreatedOn = a.CreatedOn,
                RejectionReason = a.RejectionReason
            })
.ToListAsync();
    }

    public async Task<SpecialistApplicationViewModel?> GetByIdAsync(int id)
    {
        return await specialistApplicationRepository
            .GetAllAttached()
            .Include(a => a.User)
            .Where(a => a.Id == id)
            .Select(a => new SpecialistApplicationViewModel
            {
                Id = a.Id,
                UserId = a.UserId,
                UserFullName = $"{a.User.FirstName} {a.User.LastName}",
                Email = a.User.Email!,
                Occupation = a.Occupation,
                ExperienceYears = a.ExperienceYears,
                Biography = a.Biography,
                Status = a.Status,
                StatusText = BulgarianTextHelper.GetRequestStatusText(a.Status),
                CreatedOn = a.CreatedOn,
                RejectionReason = a.RejectionReason
            })
            .FirstOrDefaultAsync();
    }



    public async Task<SpecialistApplicationViewModel?> GetLatestByUserIdAsync(Guid userId)
    {
        return await specialistApplicationRepository
            .GetAllAttached()
            .AsNoTracking()
            .Include(a => a.User)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedOn)
            .Select(a => new SpecialistApplicationViewModel
            {
                Id = a.Id,
                UserId = a.UserId,
                UserFullName = $"{a.User.FirstName} {a.User.LastName}",
                Email = a.User.Email!,
                Occupation = a.Occupation,
                ExperienceYears = a.ExperienceYears,
                Biography = a.Biography,
                Status = a.Status,
                StatusText = BulgarianTextHelper.GetRequestStatusText(a.Status),
                CreatedOn = a.CreatedOn,
                RejectionReason = a.RejectionReason
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ApplySpecialistViewModel?> GetApplicationDraftAsync(Guid userId)
    {
        Employee? employee = await employeeRepository
            .GetAllAttached()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee != null)
        {
            return new ApplySpecialistViewModel
            {
                Occupation = employee.Occupation,
                ExperienceYears = employee.ExperienceYears,
                Biography = employee.Biography
            }
   ;
        }

        SpecialistApplication? latestApplication = await specialistApplicationRepository
            .GetAllAttached()
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedOn)
            .FirstOrDefaultAsync();

        if (latestApplication == null)
        {
            return null;
        }

        return new ApplySpecialistViewModel
        {
            Occupation = latestApplication.Occupation,
            ExperienceYears = latestApplication.ExperienceYears,
            Biography = latestApplication.Biography
        };

    }

    public async Task ApproveAsync(int id)
    {
        SpecialistApplication application = await specialistApplicationRepository.GetByIdAsync(id);

        if (application.Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException("Тази заявка вече е прегледана.");
        }

        GlowUser user = await userRepository.GetByIdAsync(application.UserId);

        bool alreadyInRole = await userManager.IsInRoleAsync(user, "Specialist");
        if (alreadyInRole || user.IsSpecialist)
        {
            throw new InvalidOperationException("Този потребител вече е специалист.");
        }

        Employee? existingEmployee = await employeeRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(e => e.UserId == application.UserId);

        application.Status = RequestStatus.Accepted;
        application.RejectionReason = null;
        user.IsSpecialist = true;

        bool applicationUpdated = await specialistApplicationRepository.UpdateAsync(application);
        if (!applicationUpdated)
        {
            throw new InvalidOperationException("Заявката не можа да бъде обновена.");
        }

        bool userUpdated = await userRepository.UpdateAsync(user);
        if (!userUpdated)
        {
            throw new InvalidOperationException("Потребителят не можа да бъде обновен.");
        }

        if (existingEmployee == null)
        {
            Employee employee = new Employee
            {
                UserId = application.UserId,
                Occupation = application.Occupation,
                ExperienceYears = application.ExperienceYears,
                Biography = application.Biography,
                IsDeleted = false
            }
    ;

            await employeeRepository.AddAsync(employee);
        }
        else
        {
            existingEmployee.Occupation = application.Occupation;
            existingEmployee.ExperienceYears = application.ExperienceYears;
            existingEmployee.Biography = application.Biography;
            existingEmployee.IsDeleted = false;

            bool employeeUpdated = await employeeRepository.UpdateAsync(existingEmployee);
            if (!employeeUpdated)
            {
                throw new InvalidOperationException("Профилът на специалиста не можа да бъде обновен.");
            }
        }

        IdentityResult roleResult = await userManager.AddToRoleAsync(user, "Specialist");
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException("Ролята „Специалист“ не можа да бъде зададена.");
        }
    }

    public async Task DeclineAsync(int id, string? rejectionReason)
    {
        SpecialistApplication application = await specialistApplicationRepository.GetByIdAsync(id);

        if (application.Status != RequestStatus.Pending)
        {
            throw new InvalidOperationException("Тази заявка вече е прегледана.");
        }

        application.Status = RequestStatus.Declined;
        application.RejectionReason = string.IsNullOrWhiteSpace(rejectionReason)
            ? null
            : rejectionReason.Trim();

        bool updateResult = await specialistApplicationRepository.UpdateAsync(application);

        if (!updateResult)
        {
            throw new InvalidOperationException("Заявката не можа да бъде отхвърлена.");
        }
    }

    public async Task<bool> UserHasPendingApplicationAsync(Guid userId)
    {
        return await specialistApplicationRepository
            .GetAllAttached()
            .AnyAsync(a => a.UserId == userId && a.Status == RequestStatus.Pending);
    }

    public async Task<bool> UserIsAlreadySpecialistAsync(Guid userId)
    {
        GlowUser? user = await userRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        if (user.IsSpecialist)
        {
            return true;
        }

        return await userManager.IsInRoleAsync(user, "Specialist");
    }

    public async Task ApplyAsync(Guid userId, ApplySpecialistViewModel model)
    {
        GlowUser? user = await userRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new InvalidOperationException("Потребителят не беше намерен.");
        }

        bool hasPendingApplication = await UserHasPendingApplicationAsync(userId);
        if (hasPendingApplication)
        {
            throw new InvalidOperationException("Вече имате чакаща заявка.");
        }

        bool isAlreadySpecialist = await UserIsAlreadySpecialistAsync(userId);
        if (isAlreadySpecialist)
        {
            throw new InvalidOperationException("Вече сте специалист.");
        }

        SpecialistApplication application = new()
        {
            UserId = userId,
            Occupation = model.Occupation,
            ExperienceYears = model.ExperienceYears,
            Biography = model.Biography,
            Status = RequestStatus.Pending,
            CreatedOn = DateTime.UtcNow,
            RejectionReason = null
        };


        await specialistApplicationRepository.AddAsync(application);
    }
}
