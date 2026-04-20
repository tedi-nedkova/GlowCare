using GlowCare.Core.Implementations;
using GlowCare.Entities;
using GlowCare.Entities.Models;
using GlowCare.Entities.Repositories;
using GlowCare.ViewModels.Reviews;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ServiceEntity = GlowCare.Entities.Models.Service;
using Xunit;

namespace GlowCare.Tests;

public class ReviewServiceTests
{
    [Fact]
    public async Task GetAddReviewModelAsync_ShouldReturnNull_WhenEmployeeDoesNotExist()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        var service = CreateService(context, userManager);

        var result = await service.GetAddReviewModelAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAddReviewModelAsync_ShouldPreselectRequestedProcedure_WhenItExists()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);

        GlowUser specialistUser = new() { Id = Guid.NewGuid(), FirstName = "Ivan", LastName = "Petrov", UserName = "spec", NormalizedUserName = "SPEC", Email = "spec@test.bg" };
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Therapist", ExperienceYears = 5 };
        GlowUser client = new() { Id = Guid.NewGuid(), FirstName = "Mira", LastName = "Koleva", UserName = "client", NormalizedUserName = "CLIENT", Email = "client@test.bg" };
        ServiceEntity facial = new() { Id = 1, Name = "Facial", DurationInMinutes = 45, Price = 70, Points = 7 };
        ServiceEntity massage = new() { Id = 2, Name = "Massage", DurationInMinutes = 60, Price = 100, Points = 10 };

        context.Users.AddRange(specialistUser, client);
        context.Employees.Add(employee);
        context.Services.AddRange(facial, massage);
        context.Procedures.AddRange(
            new Procedure { Id = 1, UserId = client.Id, EmployeeId = employee.Id, ServiceId = facial.Id, Service = facial, AppointmentDate = DateTime.Now.AddDays(-3) },
            new Procedure { Id = 2, UserId = client.Id, EmployeeId = employee.Id, ServiceId = massage.Id, Service = massage, AppointmentDate = DateTime.Now.AddDays(-1) });
        await context.SaveChangesAsync();

        var service = CreateService(context, userManager);

        AddReviewViewModel? result = await service.GetAddReviewModelAsync(employee.Id, client.Id, 2);

        Assert.NotNull(result);
        Assert.Equal(2, result!.ProcedureId);
        Assert.Equal(massage.Id, result.ServiceId);
        Assert.Equal("Ivan Petrov", result.SpecialistName);
        Assert.Equal(2, result.AvailableProcedures.Count());
    }

    [Fact]
    public async Task GetAddReviewModelAsync_ShouldFallbackToFirstProcedure_WhenRequestedProcedureIsInvalid()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);

        GlowUser specialistUser = new() { Id = Guid.NewGuid(), FirstName = "Eva", LastName = "Petrova", UserName = "spec2", NormalizedUserName = "SPEC2", Email = "spec2@test.bg" };
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Cosmetician", ExperienceYears = 6 };
        GlowUser client = new() { Id = Guid.NewGuid(), FirstName = "Ani", LastName = "Georgieva", UserName = "client2", NormalizedUserName = "CLIENT2", Email = "client2@test.bg" };
        ServiceEntity serviceEntity = new() { Id = 1, Name = "Laser", DurationInMinutes = 30, Price = 80, Points = 8 };

        context.Users.AddRange(specialistUser, client);
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Procedures.Add(new Procedure
        {
            Id = 5,
            UserId = client.Id,
            EmployeeId = employee.Id,
            ServiceId = serviceEntity.Id,
            Service = serviceEntity,
            AppointmentDate = DateTime.Now.AddDays(-2)
        });
        await context.SaveChangesAsync();

        var service = CreateService(context, userManager);

        AddReviewViewModel? result = await service.GetAddReviewModelAsync(employee.Id, client.Id, 999);

        Assert.NotNull(result);
        Assert.Equal(5, result!.ProcedureId);
        Assert.Equal(serviceEntity.Id, result.ServiceId);
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldThrow_WhenUserDoesNotExist()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        var service = CreateService(context, userManager);

        AddReviewViewModel model = new()
        {
            EmployeeId = Guid.NewGuid(),
            ProcedureId = 1,
            Rating = 5,
            Comment = "Comment"
        };

        await Assert.ThrowsAsync<NullReferenceException>(() => service.CreateReviewAsync(model, Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldThrow_WhenProcedureIdIsMissing()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);

        GlowUser user = new() { Id = Guid.NewGuid(), FirstName = "User", LastName = "One", UserName = "user1", NormalizedUserName = "USER1", Email = "user1@test.bg" };
        GlowUser specialistUser = new() { Id = Guid.NewGuid(), FirstName = "Special", LastName = "One", UserName = "special1", NormalizedUserName = "SPECIAL1", Email = "special1@test.bg" };
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Therapist", ExperienceYears = 3 };

        context.Users.AddRange(user, specialistUser);
        context.Employees.Add(employee);
        await context.SaveChangesAsync();

        var service = CreateService(context, userManager);

        AddReviewViewModel model = new() { EmployeeId = employee.Id, Rating = 4, Comment = "Comment", ProcedureId = null };

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateReviewAsync(model, user.Id));
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldCreateReview_WhenDataIsValid()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);

        GlowUser user = new() { Id = Guid.NewGuid(), FirstName = "User", LastName = "Two", UserName = "user2", NormalizedUserName = "USER2", Email = "user2@test.bg" };
        GlowUser specialistUser = new() { Id = Guid.NewGuid(), FirstName = "Special", LastName = "Two", UserName = "special2", NormalizedUserName = "SPECIAL2", Email = "special2@test.bg" };
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Therapist", ExperienceYears = 9 };
        ServiceEntity serviceEntity = new() { Id = 3, Name = "Peeling", DurationInMinutes = 25, Price = 55, Points = 5 };
        Procedure procedure = new()
        {
            Id = 12,
            UserId = user.Id,
            EmployeeId = employee.Id,
            ServiceId = serviceEntity.Id,
            Service = serviceEntity,
            AppointmentDate = DateTime.Now.AddDays(-1)
        };

        context.Users.AddRange(user, specialistUser);
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Procedures.Add(procedure);
        await context.SaveChangesAsync();

        var service = CreateService(context, userManager);
        AddReviewViewModel model = new() { EmployeeId = employee.Id, ProcedureId = procedure.Id, Rating = 5, Comment = "Amazing" };

        await service.CreateReviewAsync(model, user.Id);

        Review review = Assert.Single(context.Reviews);
        Assert.Equal(user.Id, review.UserId);
        Assert.Equal(employee.Id, review.EmployeeId);
        Assert.Equal(procedure.Id, review.ProcedureId);
        Assert.Equal(serviceEntity.Id, review.ServiceId);
        Assert.Equal("Amazing", review.Comment);
    }

    [Fact]
    public async Task GetReviewsByEmployeeIdAsync_ShouldReturnNull_WhenEmployeeIsMissing()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);
        var service = CreateService(context, userManager);

        var result = await service.GetReviewsByEmployeeIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetReviewsByEmployeeIdAsync_ShouldReturnMappedReviews()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);

        GlowUser specialistUser = new() { Id = Guid.NewGuid(), FirstName = "Lili", LastName = "Marinova", UserName = "spec3", NormalizedUserName = "SPEC3", Email = "spec3@test.bg" };
        GlowUser author = new() { Id = Guid.NewGuid(), FirstName = "Kris", LastName = "Danev", UserName = "author", NormalizedUserName = "AUTHOR", Email = "author@test.bg" };
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Expert", ExperienceYears = 10 };
        ServiceEntity serviceEntity = new() { Id = 1, Name = "Hydra", DurationInMinutes = 40, Price = 85, Points = 8 };

        context.Users.AddRange(specialistUser, author);
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Reviews.AddRange(
            new Review { Id = 1, EmployeeId = employee.Id, UserId = author.Id, User = author, ServiceId = 1, Service = serviceEntity, Rating = 5, Comment = "Top", CreatedAt = DateTime.UtcNow.AddHours(-1) },
            new Review { Id = 2, EmployeeId = employee.Id, UserId = author.Id, User = author, ServiceId = 1, Service = serviceEntity, Rating = 3, Comment = "Okay", CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new Review { Id = 3, EmployeeId = employee.Id, UserId = author.Id, User = author, ServiceId = 1, Service = serviceEntity, Rating = 1, Comment = "Deleted", CreatedAt = DateTime.UtcNow.AddHours(-3), IsDeleted = true });
        await context.SaveChangesAsync();

        var service = CreateService(context, userManager);

        ReviewIndexViewModel? result = await service.GetReviewsByEmployeeIdAsync(employee.Id);

        Assert.NotNull(result);
        Assert.Equal("Lili Marinova", result!.SpecialistName);
        Assert.Equal(2, result.ReviewsCount);
        Assert.Equal(4, result.AverageRating);
        Assert.Equal("Top", result.Reviews.First().Comment);
    }

    [Fact]
    public async Task GetAllReviewsForAdminAsync_ShouldReturnOnlyActiveReviews()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);

        GlowUser specialistUser = new() { Id = Guid.NewGuid(), FirstName = "Spec", LastName = "Admin", UserName = "specAdmin", NormalizedUserName = "SPECADMIN", Email = "specadmin@test.bg" };
        GlowUser author = new() { Id = Guid.NewGuid(), FirstName = "Author", LastName = "Admin", UserName = "authorAdmin", NormalizedUserName = "AUTHORADMIN", Email = "authoradmin@test.bg" };
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Expert", ExperienceYears = 8 };
        ServiceEntity serviceEntity = new() { Id = 4, Name = "Spa", DurationInMinutes = 90, Price = 120, Points = 12 };

        context.Users.AddRange(specialistUser, author);
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Reviews.AddRange(
            new Review { Id = 10, EmployeeId = employee.Id, Employee = employee, UserId = author.Id, User = author, ServiceId = serviceEntity.Id, Service = serviceEntity, Rating = 5, Comment = "Newest", CreatedAt = DateTime.UtcNow },
            new Review { Id = 11, EmployeeId = employee.Id, Employee = employee, UserId = author.Id, User = author, ServiceId = serviceEntity.Id, Service = serviceEntity, Rating = 4, Comment = "Deleted", CreatedAt = DateTime.UtcNow.AddMinutes(-1), IsDeleted = true });
        await context.SaveChangesAsync();

        var service = CreateService(context, userManager);

        var result = (await service.GetAllReviewsForAdminAsync()).ToList();

        ReviewIndexViewModel? ignored = await service.GetReviewsByEmployeeIdAsync(employee.Id);
        Assert.Single(result);
        Assert.Equal("Newest", result.First().Comment);
        Assert.Equal("Author Admin", result.First().AuthorName);
        Assert.Equal("Spec Admin", result.First().SpecialistName);
    }

    [Fact]
    public async Task SoftDeleteReviewAsync_ShouldMarkReviewAsDeleted()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);

        context.Reviews.Add(new Review { Id = 100, Comment = "Delete me", CreatedAt = DateTime.UtcNow, Rating = 2 });
        await context.SaveChangesAsync();

        var service = CreateService(context, userManager);

        await service.SoftDeleteReviewAsync(100);

        Assert.True(context.Reviews.Single().IsDeleted);
    }

    [Fact]
    public async Task SoftDeleteReviewAsync_ShouldThrow_WhenAlreadyDeleted()
    {
        using GlowCareDbContext context = CreateContext();
        UserManager<GlowUser> userManager = CreateUserManager(context);

        context.Reviews.Add(new Review { Id = 101, Comment = "Deleted", CreatedAt = DateTime.UtcNow, Rating = 1, IsDeleted = true });
        await context.SaveChangesAsync();

        var service = CreateService(context, userManager);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.SoftDeleteReviewAsync(101));
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

    private static ReviewService CreateService(GlowCareDbContext context, UserManager<GlowUser> userManager)
    {
        return new ReviewService(
            new Repository<Review, int>(context),
            new Repository<Procedure, int>(context),
            new Repository<Employee, Guid>(context),
            userManager);
    }
}
