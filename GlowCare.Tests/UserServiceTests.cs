using GlowCare.Core.Implementations;
using GlowCare.Entities;
using GlowCare.Entities.Models;
using GlowCare.Entities.Models.Enums;
using GlowCare.Entities.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ServiceEntity = GlowCare.Entities.Models.Service;
using Xunit;

namespace GlowCare.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task AssignUserToRoleAsync_ShouldReturnFalse_WhenUserIsMissingOrRoleIsInvalid()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);
        UserService service = CreateService(context, roleManager, userManager);

        bool missingUser = await service.AssignUserToRoleAsync(Guid.NewGuid(), "Admin");

        GlowUser deletedUser = await CreatePersistedUserAsync(userManager, "Deleted", "User", "deleted@test.bg");
        deletedUser.IsDeleted = true;
        await AssertIdentitySuccessAsync(userManager.UpdateAsync(deletedUser));

        bool deletedUserResult = await service.AssignUserToRoleAsync(deletedUser.Id, "Admin");

        await EnsureRoleExistsAsync(roleManager, "Specialist");
        GlowUser regularUser = await CreatePersistedUserAsync(userManager, "Regular", "User", "regular@test.bg");
        bool forbiddenRole = await service.AssignUserToRoleAsync(regularUser.Id, "Specialist");

        Assert.False(missingUser);
        Assert.False(deletedUserResult);
        Assert.False(forbiddenRole);
    }

    [Fact]
    public async Task AssignUserToRoleAsync_ShouldAddRole_WhenDataIsValid()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);
        await EnsureRoleExistsAsync(roleManager, "Admin");
        GlowUser user = await CreatePersistedUserAsync(userManager, "Role", "User", "role@test.bg");

        UserService service = CreateService(context, roleManager, userManager);

        bool result = await service.AssignUserToRoleAsync(user.Id, "Admin");

        Assert.True(result);
        Assert.True(await userManager.IsInRoleAsync(user, "Admin"));
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserIsMissing()
    {
        using GlowCareDbContext context = CreateContext();
        UserService service = CreateService(context, CreateRoleManager(context), CreateUserManager(context));

        bool result = await service.DeleteUserAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldSoftDeleteUserAndEmployee()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);
        GlowUser user = await CreatePersistedUserAsync(userManager, "Delete", "Me", "deleteme@test.bg");
        context.Employees.Add(new Employee
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Occupation = "Therapist",
            ExperienceYears = 5
        });
        await context.SaveChangesAsync();

        UserService service = CreateService(context, roleManager, userManager);

        bool result = await service.DeleteUserAsync(user.Id);

        GlowUser? updatedUser = await userManager.FindByIdAsync(user.Id.ToString());
        Employee updatedEmployee = context.Employees.Single(e => e.UserId == user.Id);

        Assert.True(result);
        Assert.NotNull(updatedUser);
        Assert.True(updatedUser!.IsDeleted);
        Assert.True(updatedEmployee.IsDeleted);
        Assert.Equal(DateTimeOffset.MaxValue, updatedUser.LockoutEnd);
    }

    [Fact]
    public async Task GetAllUsersAsync_AndGetTotalPagesAsync_ShouldReturnPagedUsers()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);
        await EnsureRoleExistsAsync(roleManager, "Admin");

        GlowUser first = await CreatePersistedUserAsync(userManager, "First", "User", "first@test.bg");
        await CreatePersistedUserAsync(userManager, "Second", "User", "second@test.bg");
        await CreatePersistedUserAsync(userManager, "Third", "User", "third@test.bg");
        await AssertIdentitySuccessAsync(userManager.AddToRoleAsync(first, "Admin"));

        UserService service = CreateService(context, roleManager, userManager);

        List<GlowCare.ViewModels.Users.AllUsersViewModel> users = (await service.GetAllUsersAsync(2, 2)).ToList();
        int totalPages = await service.GetTotalPagesAsync(2);

        Assert.Single(users);
        Assert.Equal(2, totalPages);
    }

    [Fact]
    public async Task RemoveUserFromRoleAsync_ShouldReturnFalse_WhenUserOrRoleIsInvalid()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);
        UserService service = CreateService(context, roleManager, userManager);

        bool result = await service.RemoveUserFromRoleAsync(Guid.NewGuid(), "Admin");

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveUserFromRoleAsync_ShouldReturnTrue_WhenUserIsNotInRole()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);
        await EnsureRoleExistsAsync(roleManager, "Admin");
        GlowUser user = await CreatePersistedUserAsync(userManager, "No", "Role", "norole@test.bg");

        UserService service = CreateService(context, roleManager, userManager);

        bool result = await service.RemoveUserFromRoleAsync(user.Id, "Admin");

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveUserFromRoleAsync_ShouldUnsetSpecialistFlag_AndRevokeLatestAcceptedApplication()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);
        await EnsureRoleExistsAsync(roleManager, "Specialist");

        GlowUser user = await CreatePersistedUserAsync(userManager, "Spec", "User", "spec.user@test.bg", isSpecialist: true);
        context.Set<SpecialistApplication>().AddRange(
            new SpecialistApplication { Id = 1, UserId = user.Id, User = user, Status = RequestStatus.Accepted, Occupation = "Old", ExperienceYears = 1, CreatedOn = DateTime.UtcNow.AddDays(-2) },
            new SpecialistApplication { Id = 2, UserId = user.Id, User = user, Status = RequestStatus.Accepted, Occupation = "New", ExperienceYears = 2, CreatedOn = DateTime.UtcNow });
        await context.SaveChangesAsync();
        await AssertIdentitySuccessAsync(userManager.AddToRoleAsync(user, "Specialist"));

        UserService service = CreateService(context, roleManager, userManager);

        bool result = await service.RemoveUserFromRoleAsync(user.Id, "Specialist");

        GlowUser? updatedUser = await userManager.FindByIdAsync(user.Id.ToString());
        SpecialistApplication latestApplication = context.Set<SpecialistApplication>().Single(a => a.Id == 2);

        Assert.True(result);
        Assert.NotNull(updatedUser);
        Assert.False(updatedUser!.IsSpecialist);
        Assert.Equal(RequestStatus.Revoked, latestApplication.Status);
    }

    [Fact]
    public async Task UpdateUserMembershipAsync_ShouldAssignHighestEligibleMembership()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        GlowUser user = await CreatePersistedUserAsync(userManager, "Member", "User", "member@test.bg");
        user.LoyaltyPoints = 120;
        await AssertIdentitySuccessAsync(userManager.UpdateAsync(user));
        context.Memberships.AddRange(
            new Membership { Id = 1, Title = MembershipTitle.GlowEntry, DiscountPercentage = 5, Points = 50 },
            new Membership { Id = 2, Title = MembershipTitle.GlowPlus, DiscountPercentage = 10, Points = 100 },
            new Membership { Id = 3, Title = MembershipTitle.GlowElite, DiscountPercentage = 15, Points = 200 });
        await context.SaveChangesAsync();

        UserService service = CreateService(context, CreateRoleManager(context), userManager);

        await service.UpdateUserMembershipAsync(user);

        GlowUser? updatedUser = await userManager.FindByIdAsync(user.Id.ToString());
        Assert.Equal(2, updatedUser!.MembershipId);
    }

    [Fact]
    public async Task UserExistsByIdAsync_ShouldIgnoreDeletedUsers()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        GlowUser user = await CreatePersistedUserAsync(userManager, "Exists", "User", "exists@test.bg");
        GlowUser deletedUser = await CreatePersistedUserAsync(userManager, "Deleted", "Exists", "deleted.exists@test.bg");
        deletedUser.IsDeleted = true;
        await AssertIdentitySuccessAsync(userManager.UpdateAsync(deletedUser));

        UserService service = CreateService(context, CreateRoleManager(context), userManager);

        Assert.True(await service.UserExistsByIdAsync(user.Id));
        Assert.False(await service.UserExistsByIdAsync(deletedUser.Id));
    }

    [Fact]
    public async Task GetUserProfileAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        using GlowCareDbContext context = CreateContext();
        UserService service = CreateService(context, CreateRoleManager(context), CreateUserManager(context));

        await Assert.ThrowsAsync<NullReferenceException>(() => service.GetUserProfileAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetUserProfileAsync_ShouldSynchronizeRewards_ForRegularUser()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        GlowUser user = await CreatePersistedUserAsync(userManager, "Regular", "User", "regular@test.bg");
        context.Memberships.AddRange(
            new Membership { Id = 1, Title = MembershipTitle.GlowEntry, DiscountPercentage = 5, Points = 10 },
            new Membership { Id = 2, Title = MembershipTitle.GlowPlus, DiscountPercentage = 10, Points = 30 });
        GlowUser specialistUser = await CreatePersistedUserAsync(userManager, "Spec", "Owner", "specowner@test.bg", isSpecialist: true);
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Therapist", ExperienceYears = 7 };
        ServiceEntity serviceEntity = new() { Id = 1, Name = "Massage", DurationInMinutes = 60, Price = 100, Points = 15 };
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Procedures.Add(new Procedure
        {
            Id = 1,
            UserId = user.Id,
            User = user,
            EmployeeId = employee.Id,
            Employee = employee,
            ServiceId = serviceEntity.Id,
            Service = serviceEntity,
            AppointmentDate = DateTime.Now.AddDays(-1),
            Status = Status.Scheduled,
            RewardPointsGranted = false
        });
        await context.SaveChangesAsync();

        UserService service = CreateService(context, CreateRoleManager(context), userManager);

        var result = await service.GetUserProfileAsync(user.Id);
        Procedure updatedProcedure = context.Procedures.Single();
        GlowUser? updatedUser = await userManager.FindByIdAsync(user.Id.ToString());

        Assert.False(result.IsAdmin);
        Assert.False(result.IsSpecialist);
        Assert.Equal(15, result.TotalPoints);
        Assert.Single(result.Procedures);
        Assert.True(updatedProcedure.RewardPointsGranted);
        Assert.Equal(Status.Completed, updatedProcedure.Status);
        Assert.NotEmpty(result.Memberships);
        Assert.Equal(15, updatedUser!.LoyaltyPoints);
    }

    [Fact]
    public async Task GetUserProfileAsync_ShouldReturnSpecialistProfile()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);
        await EnsureRoleExistsAsync(roleManager, "Specialist");

        GlowUser specialistUser = await CreatePersistedUserAsync(userManager, "Specialist", "User", "specialist@test.bg", isSpecialist: true);
        GlowUser client = await CreatePersistedUserAsync(userManager, "Client", "User", "client.profile@test.bg");
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Therapist", ExperienceYears = 9 };
        ServiceEntity serviceEntity = new() { Id = 1, Name = "Therapy", DurationInMinutes = 60, Price = 90, Points = 9 };
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Procedures.Add(new Procedure
        {
            Id = 2,
            UserId = client.Id,
            User = client,
            EmployeeId = employee.Id,
            Employee = employee,
            ServiceId = serviceEntity.Id,
            Service = serviceEntity,
            AppointmentDate = DateTime.Now.AddDays(1),
            Status = Status.Scheduled
        });
        await context.SaveChangesAsync();
        await AssertIdentitySuccessAsync(userManager.AddToRoleAsync(specialistUser, "Specialist"));

        UserService service = CreateService(context, roleManager, userManager);

        var result = await service.GetUserProfileAsync(specialistUser.Id);

        Assert.True(result.IsSpecialist);
        Assert.False(result.IsAdmin);
        Assert.Single(result.Procedures);
        Assert.Empty(result.Memberships);
        Assert.Equal("Client User", result.Procedures[0].ClientName);
    }

    [Fact]
    public async Task GetUserProfileAsync_ShouldKeepEmployeeCancelledProceduresVisibleForClient()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);

        GlowUser client = await CreatePersistedUserAsync(userManager, "Raya", "Client", "raya.visibility@test.bg");
        GlowUser specialistUser = await CreatePersistedUserAsync(userManager, "Miro", "Spec", "miro.visibility@test.bg", isSpecialist: true);
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Therapist", ExperienceYears = 9 };
        ServiceEntity serviceEntity = new() { Id = 7, Name = "Therapy", DurationInMinutes = 60, Price = 90, Points = 9 };

        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Procedures.Add(new Procedure
        {
            Id = 22,
            UserId = client.Id,
            User = client,
            EmployeeId = employee.Id,
            Employee = employee,
            ServiceId = serviceEntity.Id,
            Service = serviceEntity,
            AppointmentDate = DateTime.Now.AddDays(1),
            Status = Status.Cancelled,
            IsDeleted = true,
            CancelledBy = CancelledBy.Employee
        });
        await context.SaveChangesAsync();

        UserService service = CreateService(context, roleManager, userManager);

        var result = await service.GetUserProfileAsync(client.Id);

        Assert.Single(result.Procedures);
        Assert.Equal("Отказана от специалист", result.Procedures[0].Status);
        Assert.False(result.Procedures[0].CanBeCancelledByUser);
    }

    [Fact]
    public async Task GetUserProfileAsync_ShouldKeepUserCancelledProceduresVisibleForSpecialist()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);
        await EnsureRoleExistsAsync(roleManager, "Specialist");

        GlowUser specialistUser = await CreatePersistedUserAsync(userManager, "Specialist", "User", "specialist.visibility@test.bg", isSpecialist: true);
        GlowUser client = await CreatePersistedUserAsync(userManager, "Client", "User", "client.visibility@test.bg");
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Therapist", ExperienceYears = 9 };
        ServiceEntity serviceEntity = new() { Id = 8, Name = "Therapy", DurationInMinutes = 60, Price = 90, Points = 9 };

        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Procedures.Add(new Procedure
        {
            Id = 23,
            UserId = client.Id,
            User = client,
            EmployeeId = employee.Id,
            Employee = employee,
            ServiceId = serviceEntity.Id,
            Service = serviceEntity,
            AppointmentDate = DateTime.Now.AddDays(1),
            Status = Status.Cancelled,
            IsDeleted = true,
            CancelledBy = CancelledBy.User
        });
        await context.SaveChangesAsync();
        await AssertIdentitySuccessAsync(userManager.AddToRoleAsync(specialistUser, "Specialist"));

        UserService service = CreateService(context, roleManager, userManager);

        var result = await service.GetUserProfileAsync(specialistUser.Id);

        Assert.Single(result.Procedures);
        Assert.Equal("Отказана от клиент", result.Procedures[0].Status);
        Assert.False(result.Procedures[0].CanBeRejectedBySpecialist);
    }

    [Fact]
    public async Task GetUserProfileAsync_ShouldReturnAdminProfile()
    {
        using GlowCareDbContext context = CreateContext();
        RoleManager<IdentityRole<Guid>> roleManager = CreateRoleManager(context);
        UserManager<GlowUser> userManager = CreateUserManager(context);
        await EnsureRoleExistsAsync(roleManager, "Admin");
        GlowUser admin = await CreatePersistedUserAsync(userManager, "Admin", "User", "admin@test.bg");
        await AssertIdentitySuccessAsync(userManager.AddToRoleAsync(admin, "Admin"));

        UserService service = CreateService(context, roleManager, userManager);

        var result = await service.GetUserProfileAsync(admin.Id);

        Assert.True(result.IsAdmin);
        Assert.False(result.IsSpecialist);
        Assert.Empty(result.Procedures);
        Assert.Empty(result.Memberships);
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

    private static UserService CreateService(GlowCareDbContext context, RoleManager<IdentityRole<Guid>> roleManager, UserManager<GlowUser> userManager)
    {
        return new UserService(
            roleManager,
            userManager,
            new Repository<Employee, Guid>(context),
            new Repository<SpecialistApplication, int>(context),
            new Repository<Procedure, int>(context),
            new Repository<Membership, int>(context));
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
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = email.ToUpperInvariant(),
            Age = 25,
            Gender = Gender.Female,
            IsSpecialist = isSpecialist,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        await AssertIdentitySuccessAsync(userManager.CreateAsync(user));
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
