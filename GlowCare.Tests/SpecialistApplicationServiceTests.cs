using GlowCare.Entities;
using GlowCare.Entities.Models;
using GlowCare.Entities.Models.Enums;
using GlowCare.Entities.Repositories;
using GlowCare.Services.Implementations;
using GlowCare.ViewModels.SpecialistRequest;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace GlowCare.Tests;

public class SpecialistApplicationServiceTests
{
    [Fact]
    public async Task GetPendingApplicationsAsync_ShouldReturnOnlyPendingApplications()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        SpecialistApplicationService service = CreateService(context, userManager);

        GlowUser pendingUser = await CreatePersistedUserAsync(userManager, "Pending", "User", "pending@test.bg");
        GlowUser acceptedUser = await CreatePersistedUserAsync(userManager, "Accepted", "User", "accepted@test.bg");
        context.Set<SpecialistApplication>().AddRange(
            new SpecialistApplication { Id = 1, UserId = pendingUser.Id, User = pendingUser, Occupation = "Therapist", ExperienceYears = 5, Status = RequestStatus.Pending, CreatedOn = DateTime.UtcNow },
            new SpecialistApplication { Id = 2, UserId = acceptedUser.Id, User = acceptedUser, Occupation = "Therapist", ExperienceYears = 5, Status = RequestStatus.Accepted, CreatedOn = DateTime.UtcNow });
        await context.SaveChangesAsync();

        List<SpecialistApplicationViewModel> result = (await service.GetPendingApplicationsAsync()).ToList();

        Assert.Single(result);
        Assert.Equal("Pending User", result[0].UserFullName);
    }

    [Fact]
    public async Task GetByIdAsync_AndGetLatestByUserIdAsync_ShouldReturnMappedApplication()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        SpecialistApplicationService service = CreateService(context, userManager);

        GlowUser user = await CreatePersistedUserAsync(userManager, "Test", "User", "test@test.bg");
        context.Set<SpecialistApplication>().AddRange(
            new SpecialistApplication { Id = 1, UserId = user.Id, User = user, Occupation = "Old", ExperienceYears = 1, Status = RequestStatus.Declined, CreatedOn = DateTime.UtcNow.AddDays(-2) },
            new SpecialistApplication { Id = 2, UserId = user.Id, User = user, Occupation = "New", ExperienceYears = 4, Status = RequestStatus.Pending, CreatedOn = DateTime.UtcNow });
        await context.SaveChangesAsync();

        SpecialistApplicationViewModel? byId = await service.GetByIdAsync(2);
        SpecialistApplicationViewModel? latest = await service.GetLatestByUserIdAsync(user.Id);

        Assert.NotNull(byId);
        Assert.NotNull(latest);
        Assert.Equal("New", byId!.Occupation);
        Assert.Equal(2, latest!.Id);
    }

    [Fact]
    public async Task GetApplicationDraftAsync_ShouldPreferExistingEmployee()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        SpecialistApplicationService service = CreateService(context, userManager);

        GlowUser user = await CreatePersistedUserAsync(userManager, "Employee", "User", "employee@test.bg");
        context.Employees.Add(new Employee
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Occupation = "Senior Therapist",
            ExperienceYears = 11,
            Biography = "Employee bio"
        });
        context.Set<SpecialistApplication>().Add(new SpecialistApplication
        {
            Id = 1,
            UserId = user.Id,
            User = user,
            Occupation = "Old App",
            ExperienceYears = 1,
            Biography = "App bio",
            Status = RequestStatus.Pending,
            CreatedOn = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        ApplySpecialistViewModel? result = await service.GetApplicationDraftAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal("Senior Therapist", result!.Occupation);
        Assert.Equal(11, result.ExperienceYears);
        Assert.Equal("Employee bio", result.Biography);
    }

    [Fact]
    public async Task GetApplicationDraftAsync_ShouldReturnLatestApplication_WhenEmployeeDoesNotExist()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        SpecialistApplicationService service = CreateService(context, userManager);

        GlowUser user = await CreatePersistedUserAsync(userManager, "Applicant", "User", "applicant@test.bg");
        context.Set<SpecialistApplication>().AddRange(
            new SpecialistApplication { Id = 1, UserId = user.Id, User = user, Occupation = "Old", ExperienceYears = 1, Biography = "Old", Status = RequestStatus.Declined, CreatedOn = DateTime.UtcNow.AddDays(-3) },
            new SpecialistApplication { Id = 2, UserId = user.Id, User = user, Occupation = "Latest", ExperienceYears = 5, Biography = "Latest bio", Status = RequestStatus.Pending, CreatedOn = DateTime.UtcNow });
        await context.SaveChangesAsync();

        ApplySpecialistViewModel? result = await service.GetApplicationDraftAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal("Latest", result!.Occupation);
        Assert.Equal("Latest bio", result.Biography);
    }

    [Fact]
    public async Task UserHasPendingApplicationAsync_AndUserIsAlreadySpecialistAsync_ShouldReturnExpectedValues()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        SpecialistApplicationService service = CreateService(context, userManager);

        GlowUser pendingUser = await CreatePersistedUserAsync(userManager, "Pending", "Specialist", "pending.specialist@test.bg");
        GlowUser specialistUser = await CreatePersistedUserAsync(userManager, "Real", "Specialist", "real.specialist@test.bg", isSpecialist: true);
        context.Set<SpecialistApplication>().Add(new SpecialistApplication { Id = 1, UserId = pendingUser.Id, User = pendingUser, Occupation = "Therapist", ExperienceYears = 5, Status = RequestStatus.Pending, CreatedOn = DateTime.UtcNow });
        await context.SaveChangesAsync();

        bool hasPending = await service.UserHasPendingApplicationAsync(pendingUser.Id);
        bool isSpecialist = await service.UserIsAlreadySpecialistAsync(specialistUser.Id);
        bool missingUserResult = await service.UserIsAlreadySpecialistAsync(Guid.NewGuid());

        Assert.True(hasPending);
        Assert.True(isSpecialist);
        Assert.False(missingUserResult);
    }

    [Fact]
    public async Task ApplyAsync_ShouldThrow_WhenUserIsMissing()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        SpecialistApplicationService service = CreateService(context, userManager);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ApplyAsync(Guid.NewGuid(), new ApplySpecialistViewModel { Occupation = "Therapist", ExperienceYears = 3 }));
    }

    [Fact]
    public async Task ApplyAsync_ShouldThrow_WhenThereIsPendingApplication()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        SpecialistApplicationService service = CreateService(context, userManager);

        GlowUser user = await CreatePersistedUserAsync(userManager, "Pending", "Applicant", "pending.applicant@test.bg");
        context.Set<SpecialistApplication>().Add(new SpecialistApplication { Id = 1, UserId = user.Id, User = user, Occupation = "Therapist", ExperienceYears = 3, Status = RequestStatus.Pending, CreatedOn = DateTime.UtcNow });
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ApplyAsync(user.Id, new ApplySpecialistViewModel { Occupation = "Therapist", ExperienceYears = 3 }));
    }

    [Fact]
    public async Task ApplyAsync_ShouldCreatePendingApplication()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        SpecialistApplicationService service = CreateService(context, userManager);

        GlowUser user = await CreatePersistedUserAsync(userManager, "Fresh", "Applicant", "fresh.applicant@test.bg");

        await service.ApplyAsync(user.Id, new ApplySpecialistViewModel { Occupation = "Cosmetician", ExperienceYears = 6, Biography = "Bio" });

        SpecialistApplication application = Assert.Single(context.Set<SpecialistApplication>());
        Assert.Equal(RequestStatus.Pending, application.Status);
        Assert.Equal("Cosmetician", application.Occupation);
    }

    [Fact]
    public async Task ApproveAsync_ShouldCreateEmployeeAndAssignRole()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);
        await EnsureRoleExistsAsync(roleManager, "Specialist");
        SpecialistApplicationService service = CreateService(context, userManager);

        GlowUser user = await CreatePersistedUserAsync(userManager, "Approve", "User", "approve@test.bg");
        context.Set<SpecialistApplication>().Add(new SpecialistApplication
        {
            Id = 1,
            UserId = user.Id,
            User = user,
            Occupation = "Therapist",
            ExperienceYears = 10,
            Biography = "Bio",
            Status = RequestStatus.Pending,
            CreatedOn = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        await service.ApproveAsync(1);

        GlowUser? updatedUser = await userManager.FindByIdAsync(user.Id.ToString());
        Employee employee = Assert.Single(context.Employees);
        SpecialistApplication application = context.Set<SpecialistApplication>().Single();

        Assert.NotNull(updatedUser);
        Assert.True(updatedUser!.IsSpecialist);
        Assert.Equal(RequestStatus.Accepted, application.Status);
        Assert.Equal("Therapist", employee.Occupation);
        Assert.True(await userManager.IsInRoleAsync(updatedUser, "Specialist"));
    }

    [Fact]
    public async Task ApproveAsync_ShouldUpdateExistingEmployee_WhenItAlreadyExists()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);
        await EnsureRoleExistsAsync(roleManager, "Specialist");
        SpecialistApplicationService service = CreateService(context, userManager);

        GlowUser user = await CreatePersistedUserAsync(userManager, "Existing", "Employee", "existing@test.bg");
        context.Employees.Add(new Employee
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Occupation = "Old Occupation",
            ExperienceYears = 1,
            Biography = "Old",
            IsDeleted = true
        });
        context.Set<SpecialistApplication>().Add(new SpecialistApplication
        {
            Id = 2,
            UserId = user.Id,
            User = user,
            Occupation = "New Occupation",
            ExperienceYears = 8,
            Biography = "New",
            Status = RequestStatus.Pending,
            CreatedOn = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        await service.ApproveAsync(2);

        Employee updatedEmployee = context.Employees.Single(e => e.UserId == user.Id);
        GlowUser? updatedUser = await userManager.FindByIdAsync(user.Id.ToString());

        Assert.Equal("New Occupation", updatedEmployee.Occupation);
        Assert.Equal(8, updatedEmployee.ExperienceYears);
        Assert.Equal("New", updatedEmployee.Biography);
        Assert.False(updatedEmployee.IsDeleted);
        Assert.True(updatedUser!.IsSpecialist);
        Assert.True(await userManager.IsInRoleAsync(updatedUser, "Specialist"));
    }

    [Fact]
    public async Task ApproveAsync_ShouldThrow_WhenApplicationIsAlreadyReviewed()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        SpecialistApplicationService service = CreateService(context, userManager);

        GlowUser user = await CreatePersistedUserAsync(userManager, "Reviewed", "User", "reviewed@test.bg");
        context.Set<SpecialistApplication>().Add(new SpecialistApplication { Id = 3, UserId = user.Id, User = user, Occupation = "Therapist", ExperienceYears = 5, Status = RequestStatus.Accepted, CreatedOn = DateTime.UtcNow });
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.ApproveAsync(3));
    }

    [Fact]
    public async Task DeclineAsync_ShouldSetDeclinedStatus_AndTrimReason()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        SpecialistApplicationService service = CreateService(context, userManager);

        GlowUser user = await CreatePersistedUserAsync(userManager, "Decline", "User", "decline@test.bg");
        context.Set<SpecialistApplication>().Add(new SpecialistApplication { Id = 4, UserId = user.Id, User = user, Occupation = "Therapist", ExperienceYears = 5, Status = RequestStatus.Pending, CreatedOn = DateTime.UtcNow });
        await context.SaveChangesAsync();

        await service.DeclineAsync(4, "  Missing documents  ");

        SpecialistApplication application = context.Set<SpecialistApplication>().Single();
        Assert.Equal(RequestStatus.Declined, application.Status);
        Assert.Equal("Missing documents", application.RejectionReason);
    }

    private static GlowCareDbContext CreateContext()
    {
        DbContextOptions<GlowCareDbContext> options = new DbContextOptionsBuilder<GlowCareDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        GlowCareDbContext context = new GlowCareDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        ClearSeedData(context);
        context.ChangeTracker.Clear();

        return context;
    }

    private static void ClearSeedData(GlowCareDbContext context)
    {
        context.Reviews.RemoveRange(context.Reviews);
        context.Procedures.RemoveRange(context.Procedures);
        context.EmployeesServices.RemoveRange(context.EmployeesServices);
        context.Schedules.RemoveRange(context.Schedules);
        context.Employees.RemoveRange(context.Employees);
        context.Set<SpecialistApplication>().RemoveRange(context.Set<SpecialistApplication>());
        context.Services.RemoveRange(context.Services);
        context.Categories.RemoveRange(context.Categories);
        context.Memberships.RemoveRange(context.Memberships);
        context.UserRoles.RemoveRange(context.UserRoles);
        context.UserClaims.RemoveRange(context.UserClaims);
        context.UserLogins.RemoveRange(context.UserLogins);
        context.UserTokens.RemoveRange(context.UserTokens);
        context.RoleClaims.RemoveRange(context.RoleClaims);
        context.Roles.RemoveRange(context.Roles);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();
    }

    private static UserManager<GlowUser> CreateUserManager(GlowCareDbContext context)
    {
        var store = new UserStore<GlowUser, IdentityRole<Guid>, GlowCareDbContext, Guid>(context);

        return new UserManager<GlowUser>(
            store,
            Options.Create(new IdentityOptions()),
            new PasswordHasher<GlowUser>(),
            Array.Empty<IUserValidator<GlowUser>>(),
            Array.Empty<IPasswordValidator<GlowUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null,
            NullLogger<UserManager<GlowUser>>.Instance);
    }

    private static RoleManager<IdentityRole<Guid>> CreateRoleManager(GlowCareDbContext context)
    {
        var store = new RoleStore<IdentityRole<Guid>, GlowCareDbContext, Guid>(context);

        return new RoleManager<IdentityRole<Guid>>(
            store,
            Array.Empty<IRoleValidator<IdentityRole<Guid>>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            NullLogger<RoleManager<IdentityRole<Guid>>>.Instance);
    }

    private static SpecialistApplicationService CreateService(GlowCareDbContext context, UserManager<GlowUser> userManager)
    {
        return new SpecialistApplicationService(
            new Repository<SpecialistApplication, int>(context),
            new Repository<Employee, Guid>(context),
            new Repository<GlowUser, Guid>(context),
            userManager);
    }

    private static async Task<GlowUser> CreatePersistedUserAsync(
        UserManager<GlowUser> userManager,
        string firstName,
        string lastName,
        string email,
        bool isSpecialist = false)
    {
        GlowUser user = new()
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),
            NormalizedEmail = email.ToUpperInvariant(),
            IsSpecialist = isSpecialist,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        await AssertIdentitySuccessAsync(userManager.CreateAsync(user));

        if (isSpecialist)
        {
            await AssertIdentitySuccessAsync(userManager.UpdateAsync(user));
        }

        return user;
    }

    private static async Task EnsureRoleExistsAsync(RoleManager<IdentityRole<Guid>> roleManager, string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        IdentityRole<Guid> role = new()
        {
            Id = Guid.NewGuid(),
            Name = roleName,
            NormalizedName = roleName.ToUpperInvariant()
        };

        await AssertIdentitySuccessAsync(roleManager.CreateAsync(role));
    }

    private static async Task AssertIdentitySuccessAsync(Task<IdentityResult> resultTask)
    {
        IdentityResult result = await resultTask;
        Assert.True(result.Succeeded, string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
