using GlowCare.Core.Contracts;
using GlowCare.Core.Helpers;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using GlowCare.Entities.Models.Enums;
using GlowCare.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GlowCare.Core.Implementations;

public class UserService(
    RoleManager<IdentityRole<Guid>> roleManager,
    UserManager<GlowUser> userManager,
    IRepository<Employee, Guid> employeeRepository,
    IRepository<SpecialistApplication, int> specialistApplicationRepository,
    IRepository<Procedure, int> procedureRepository,
    IRepository<Membership, int> membershipRepository)
    : IUserService
{
    public async Task<bool> AssignUserToRoleAsync(Guid userId, string roleName)
    {
        GlowUser? user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null || user.IsDeleted || !(await roleManager.RoleExistsAsync(roleName)))
        {
            return false;
        }

        if (roleName == "Specialist")
        {
            return false;
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            IdentityResult result = await userManager.AddToRoleAsync(user, roleName);

            if (!result.Succeeded)
            {
                return false;
            }
        }

        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        GlowUser? user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null || user.IsDeleted)
        {
            return false;
        }

        user.IsDeleted = true;
        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;

        Employee? employee = await employeeRepository
            .GetAllAttached()
            .FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee != null)
        {
            employee.IsDeleted = true;

            bool employeeUpdated = await employeeRepository.UpdateAsync(employee);
            if (!employeeUpdated)
            {
                return false;
            }
        }

        IdentityResult result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                Console.WriteLine($"{error.Code}: {error.Description}");
            }

            return false;
        }

        await userManager.UpdateSecurityStampAsync(user);

        return true;
    }

    public async Task<IEnumerable<AllUsersViewModel>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 5)
    {
        var users = await userManager.Users
            .Where(u => !u.IsDeleted)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var userViewModels = new List<AllUsersViewModel>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);

            userViewModels.Add(new AllUsersViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Roles = roles.ToList()
            });
        }

        return userViewModels;
    }

    public async Task<UserProfileViewModel> GetUserProfileAsync(Guid userId)
    {
        GlowUser user = await userManager.Users
            .Include(u => u.Membership)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted)
            ?? throw new NullReferenceException("Потребителят не беше намерен.");

        var roles = await userManager.GetRolesAsync(user);
        bool isAdmin = roles.Contains("Admin");
        bool isSpecialist = roles.Contains("Specialist") || user.IsSpecialist;

        if (!isAdmin && !isSpecialist)
        {
            await SynchronizeProcedureRewardsAsync(user);

            user = await userManager.Users
                .Include(u => u.Membership)
                .FirstAsync(u => u.Id == userId);
        }

        List<UserProfileProcedureViewModel> procedures = new();

        if (isSpecialist)
        {
            procedures = await procedureRepository
                .GetAllAttached()
                .Where(p => !p.IsDeleted && p.Employee != null && p.Employee.UserId == userId)
                .Include(p => p.Service)
                .Include(p => p.User)
                .Include(p => p.Employee)
                    .ThenInclude(e => e!.User)
                .OrderByDescending(p => p.AppointmentDate)
                .Select(p => new UserProfileProcedureViewModel
                {
                    Id = p.Id,
                    ServiceName = p.Service!.Name,
                    SpecialistName = $"{p.Employee!.User.FirstName} {p.Employee.User.LastName}",
                    ClientName = $"{p.User!.FirstName} {p.User.LastName}",
                    Price = p.Service.Price,
                    EarnedPoints = 0,
                    AppointmentDate = p.AppointmentDate.ToString("dd.MM.yyyy HH:mm"),
                    Status = BulgarianTextHelper.GetProcedureStatusText(p.Status!.Value),
                    CanBeRejectedBySpecialist = p.Status == Status.Scheduled && p.AppointmentDate > DateTime.Now,
                    CanBeCancelledByUser = false
                })
                .ToListAsync();
        }
        else if (!isAdmin)
        {
            procedures = await procedureRepository
                .GetAllAttached()
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .Include(p => p.Service)
                .Include(p => p.Employee)
                    .ThenInclude(e => e!.User)
                .OrderByDescending(p => p.AppointmentDate)
                .Select(p => new UserProfileProcedureViewModel
                {
                    Id = p.Id,
                    ServiceName = p.Service!.Name,
                    SpecialistName = $"{p.Employee!.User.FirstName} {p.Employee.User.LastName}",
                    Price = p.Service.Price,
                    EarnedPoints = p.Status == Status.Completed ? p.Service.Points : 0,
                    AppointmentDate = p.AppointmentDate.ToString("dd.MM.yyyy HH:mm"),
                    Status = BulgarianTextHelper.GetProcedureStatusText(p.Status!.Value),
                    CanBeRejectedBySpecialist = false,
                    CanBeCancelledByUser = p.Status == Status.Scheduled && p.AppointmentDate > DateTime.Now
                })
                .ToListAsync();
        }

        List<UserMembershipInfoViewModel> memberships = new();
        if (!isAdmin && !isSpecialist)
        {
            memberships = await membershipRepository
                .GetAllAttached()
                .OrderBy(m => m.Points)
                .Select(m => new UserMembershipInfoViewModel
                {
                    Title = BulgarianTextHelper.GetMembershipTitleText(m.Title),
                    DiscountPercentage = m.DiscountPercentage,
                    RequiredPoints = m.Points,
                    IsCurrent = user.MembershipId == m.Id,
                    IsUnlocked = user.LoyaltyPoints >= m.Points
                })
                .ToListAsync();
        }

        Membership? currentMembership = user.Membership;

        return new UserProfileViewModel
        {
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email ?? string.Empty,
            Age = user.Age,
            Gender = BulgarianTextHelper.GetGenderText(user.Gender),
            TotalPoints = user.LoyaltyPoints,
            CurrentMembershipTitle = currentMembership != null ? BulgarianTextHelper.GetMembershipTitleText(currentMembership.Title) : "Добре дошъл",
            CurrentMembershipDiscountPercentage = currentMembership?.DiscountPercentage ?? 0,
            IsAdmin = isAdmin,
            IsSpecialist = isSpecialist,
            Procedures = procedures,
            Memberships = memberships
        };
    }

    public async Task<int> GetTotalPagesAsync(int pageSize = 5)
    {
        var totalUsers = await userManager.Users
            .Where(u => !u.IsDeleted)
            .CountAsync();

        return (int)Math.Ceiling(totalUsers / (double)pageSize);
    }

    public async Task<bool> RemoveUserFromRoleAsync(Guid userId, string roleName)
    {
        GlowUser? user = await userManager.FindByIdAsync(userId.ToString());

        if (user == null || user.IsDeleted || !(await roleManager.RoleExistsAsync(roleName)))
        {
            return false;
        }

        bool alreadyInRole = await userManager.IsInRoleAsync(user, roleName);

        if (!alreadyInRole)
        {
            return true;
        }

        IdentityResult result = await userManager.RemoveFromRoleAsync(user, roleName);

        if (!result.Succeeded)
        {
            return false;
        }

        if (roleName == "Specialist")
        {
            user.IsSpecialist = false;

            IdentityResult userUpdateResult = await userManager.UpdateAsync(user);
            if (!userUpdateResult.Succeeded)
            {
                return false;
            }

            SpecialistApplication? latestAcceptedApplication = await specialistApplicationRepository
                .GetAllAttached()
                .Where(a => a.UserId == userId && a.Status == RequestStatus.Accepted)
                .OrderByDescending(a => a.CreatedOn)
                .FirstOrDefaultAsync();

            if (latestAcceptedApplication != null)
            {
                latestAcceptedApplication.Status = RequestStatus.Revoked;
                latestAcceptedApplication.RejectionReason = null;

                bool applicationUpdated = await specialistApplicationRepository.UpdateAsync(latestAcceptedApplication);
                if (!applicationUpdated)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public async Task UpdateUserMembershipAsync(GlowUser user)
    {
        Membership? membership = await membershipRepository
            .GetAllAttached()
            .Where(m => m.Points <= user.LoyaltyPoints)
            .OrderByDescending(m => m.Points)
            .FirstOrDefaultAsync();

        user.MembershipId = membership?.Id;
        await userManager.UpdateAsync(user);
    }

    public async Task<bool> UserExistsByIdAsync(Guid userId)
    {
        GlowUser? user = await userManager.FindByIdAsync(userId.ToString());

        return user != null && !user.IsDeleted;
    }

    private async Task SynchronizeProcedureRewardsAsync(GlowUser user)
    {
        var proceduresToReward = await procedureRepository
            .GetAllAttached()
            .Where(p => p.UserId == user.Id
                && !p.IsDeleted
                && p.Status == Status.Scheduled
                && p.AppointmentDate <= DateTime.Now)
            .Include(p => p.Service)
            .ToListAsync();

        if (!proceduresToReward.Any())
        {
            return;
        }

        foreach (var procedure in proceduresToReward)
        {
            procedure.Status = Status.Completed;

            if (!procedure.RewardPointsGranted && procedure.Service != null)
            {
                user.LoyaltyPoints += procedure.Service.Points;
                procedure.RewardPointsGranted = true;
            }

            await procedureRepository.UpdateAsync(procedure);
        }

        await UpdateUserMembershipAsync(user);
    }
}
