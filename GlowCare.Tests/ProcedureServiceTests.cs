using GlowCare.Core.Contracts;
using GlowCare.Core.Implementations;
using GlowCare.Entities;
using GlowCare.Entities.Models;
using GlowCare.Entities.Models.Enums;
using GlowCare.Entities.Repositories;
using GlowCare.ViewModels.Procedures;
using GlowCare.ViewModels.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using ServiceEntity = GlowCare.Entities.Models.Service;
using Xunit;
using EmployeeService = GlowCare.Entities.Models.EmployeeService;

namespace GlowCare.Tests;

public class ProcedureServiceTests
{
    [Fact]
    public async Task CreateProcedureAsync_ShouldThrow_WhenModelIsNull()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        await Assert.ThrowsAsync<NullReferenceException>(() => service.CreateProcedureAsync(null!, Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateProcedureAsync_ShouldCreateScheduledProcedure()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        Guid userId = Guid.NewGuid();
        Guid employeeId = Guid.NewGuid();

        IndexViewModel model = new()
        {
            EmployeeId = employeeId,
            ServiceId = 4,
            AppointmentDate = DateTime.UtcNow.AddDays(1),
            Notes = "Test notes"
        };

        await service.CreateProcedureAsync(model, userId);

        Procedure procedure = Assert.Single(context.Procedures);
        Assert.Equal(userId, procedure.UserId);
        Assert.Equal(employeeId, procedure.EmployeeId);
        Assert.Equal(Status.Scheduled, procedure.Status);
        Assert.False(procedure.RewardPointsGranted);
    }

    [Fact]
    public async Task GetAllProcedureDetailsByUserIdAsync_ShouldThrow_WhenUserIsMissing()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        await Assert.ThrowsAsync<NullReferenceException>(() => service.GetAllProcedureDetailsByUserIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllProcedureDetailsByUserIdAsync_ShouldSynchronizePastScheduledProcedures()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out TrackingUserService fakeUserService);

        Membership membership = new() { Id = 1, Title = MembershipTitle.GlowEntry, DiscountPercentage = 5, Points = 10 };
        GlowUser client = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Raya",
            LastName = "Client",
            UserName = "raya",
            NormalizedUserName = "RAYA",
            Email = "raya@test.bg",
            LoyaltyPoints = 0,
            Membership = membership,
            MembershipId = membership.Id
        };
        GlowUser specialistUser = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Miro",
            LastName = "Spec",
            UserName = "miro",
            NormalizedUserName = "MIRO",
            Email = "miro@test.bg",
            IsSpecialist = true
        };
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Therapist", ExperienceYears = 7 };
        ServiceEntity serviceEntity = new() { Id = 1, Name = "Massage", DurationInMinutes = 60, Price = 100, Points = 15 };

        context.Memberships.Add(membership);
        context.Users.AddRange(client, specialistUser);
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Procedures.AddRange(
            new Procedure
            {
                Id = 1,
                UserId = client.Id,
                EmployeeId = employee.Id,
                Employee = employee,
                ServiceId = serviceEntity.Id,
                Service = serviceEntity,
                AppointmentDate = DateTime.Now.AddDays(-1),
                Status = Status.Scheduled,
                RewardPointsGranted = false
            },
            new Procedure
            {
                Id = 2,
                UserId = client.Id,
                EmployeeId = employee.Id,
                Employee = employee,
                ServiceId = serviceEntity.Id,
                Service = serviceEntity,
                AppointmentDate = DateTime.Now.AddDays(1),
                Status = Status.Scheduled,
                RewardPointsGranted = false
            });
        await context.SaveChangesAsync();

        var result = (await service.GetAllProcedureDetailsByUserIdAsync(client.Id)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(15, client.LoyaltyPoints);
        Assert.True(fakeUserService.UpdateUserMembershipCalled);
        Assert.Equal(Status.Completed, context.Procedures.Single(p => p.Id == 1).Status);
        Assert.True(context.Procedures.Single(p => p.Id == 1).RewardPointsGranted);
    }

    [Fact]
    public async Task GetDeleteProcedureAsync_ShouldReturnModel_WhenProcedureBelongsToUser()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        GlowUser user = new() { Id = Guid.NewGuid(), FirstName = "Nina", LastName = "User", UserName = "nina", NormalizedUserName = "NINA", Email = "nina@test.bg" };
        ServiceEntity serviceEntity = new() { Id = 1, Name = "Facial", DurationInMinutes = 40, Price = 80, Points = 8 };
        context.Users.Add(user);
        context.Services.Add(serviceEntity);
        context.Procedures.Add(new Procedure { Id = 8, UserId = user.Id, User = user, ServiceId = serviceEntity.Id, Service = serviceEntity, AppointmentDate = DateTime.UtcNow, Status = Status.Scheduled });
        await context.SaveChangesAsync();

        DeleteProcedureViewModel result = await service.GetDeleteProcedureAsync(8, user.Id);

        Assert.Equal(8, result.Id);
        Assert.Equal("Nina User", result.ClientName);
        Assert.Equal("Facial", result.ServiceName);
    }

    [Fact]
    public async Task GetDeleteProcedureAsync_ShouldThrow_WhenUserDoesNotOwnProcedure()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        GlowUser owner = new() { Id = Guid.NewGuid(), FirstName = "Owner", LastName = "One", UserName = "owner", NormalizedUserName = "OWNER", Email = "owner@test.bg" };
        ServiceEntity serviceEntity = new() { Id = 1, Name = "Service", DurationInMinutes = 30, Price = 50, Points = 5 };
        context.Users.Add(owner);
        context.Services.Add(serviceEntity);
        context.Procedures.Add(new Procedure { Id = 9, UserId = owner.Id, User = owner, ServiceId = 1, Service = serviceEntity, AppointmentDate = DateTime.UtcNow, Status = Status.Scheduled });
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<NullReferenceException>(() => service.GetDeleteProcedureAsync(9, Guid.NewGuid()));
    }

    [Fact]
    public async Task IsSlotAvailableAsync_ShouldReturnFalse_WhenServiceDoesNotExist()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        AvailabilityCheckResultViewModel result = await service.IsSlotAvailableAsync(Guid.NewGuid(), 99, DateTime.Now.AddHours(2));

        Assert.False(result.IsAvailable);
        Assert.Equal("Услугата не беше намерена.", result.Message);
    }

    [Fact]
    public async Task IsSlotAvailableAsync_ShouldReturnFalse_WhenEmployeeCannotDoService()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        context.Services.Add(new ServiceEntity { Id = 1, Name = "Therapy", DurationInMinutes = 60, Price = 80, Points = 8 });
        await context.SaveChangesAsync();

        AvailabilityCheckResultViewModel result = await service.IsSlotAvailableAsync(Guid.NewGuid(), 1, DateTime.Now.AddHours(2));

        Assert.False(result.IsAvailable);
        Assert.Equal("Избраният специалист не извършва тази процедура.", result.Message);
    }

    [Fact]
    public async Task IsSlotAvailableAsync_ShouldReturnFalse_WhenScheduleIsMissing()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        Guid employeeId = Guid.NewGuid();
        context.Services.Add(new ServiceEntity { Id = 1, Name = "Therapy", DurationInMinutes = 60, Price = 80, Points = 8 });
        context.EmployeesServices.Add(new EmployeeService { EmployeeId = employeeId, ServiceId = 1 });
        await context.SaveChangesAsync();

        AvailabilityCheckResultViewModel result = await service.IsSlotAvailableAsync(employeeId, 1, DateTime.Now.AddHours(2));

        Assert.False(result.IsAvailable);
        Assert.Equal("Специалистът не работи в този ден.", result.Message);
    }

    [Fact]
    public async Task IsSlotAvailableAsync_ShouldReturnFalse_WhenTimeIsOutsideWorkingHours()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        Guid employeeId = Guid.NewGuid();
        DateTime requestedTime = NextDayOfWeek(DayOfWeek.Monday).Date.AddHours(8);
        context.Services.Add(new ServiceEntity { Id = 1, Name = "Therapy", DurationInMinutes = 60, Price = 80, Points = 8 });
        context.EmployeesServices.Add(new EmployeeService { EmployeeId = employeeId, ServiceId = 1 });
        context.Schedules.Add(new Schedule { Id = 1, EmployeeId = employeeId, DayOfWeek = DayOfWeek.Monday, StartTime = "09:00", EndTime = "17:00" });
        await context.SaveChangesAsync();

        AvailabilityCheckResultViewModel result = await service.IsSlotAvailableAsync(employeeId, 1, requestedTime);

        Assert.False(result.IsAvailable);
        Assert.Contains("извън работното време", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task IsSlotAvailableAsync_ShouldReturnFalse_WhenThereIsConflict()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        Guid employeeId = Guid.NewGuid();
        DateTime requestedTime = NextDayOfWeek(DayOfWeek.Tuesday).Date.AddHours(10);
        ServiceEntity serviceEntity = new() { Id = 1, Name = "Therapy", DurationInMinutes = 60, Price = 80, Points = 8 };

        context.Services.Add(serviceEntity);
        context.EmployeesServices.Add(new EmployeeService { EmployeeId = employeeId, ServiceId = 1, Service = serviceEntity });
        context.Schedules.Add(new Schedule { Id = 1, EmployeeId = employeeId, DayOfWeek = DayOfWeek.Tuesday, StartTime = "09:00", EndTime = "17:00" });
        context.Procedures.Add(new Procedure
        {
            Id = 1,
            EmployeeId = employeeId,
            ServiceId = 1,
            Service = serviceEntity,
            AppointmentDate = requestedTime,
            Status = Status.Scheduled
        });
        await context.SaveChangesAsync();

        AvailabilityCheckResultViewModel result = await service.IsSlotAvailableAsync(employeeId, 1, requestedTime.AddMinutes(30));

        Assert.False(result.IsAvailable);
        Assert.Equal("Избраният час е зает.", result.Message);
    }

    [Fact]
    public async Task IsSlotAvailableAsync_ShouldReturnTrue_WhenSlotIsFree()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        Guid employeeId = Guid.NewGuid();
        DateTime requestedTime = NextDayOfWeek(DayOfWeek.Wednesday).Date.AddHours(11);
        ServiceEntity serviceEntity = new() { Id = 1, Name = "Therapy", DurationInMinutes = 60, Price = 80, Points = 8 };

        context.Services.Add(serviceEntity);
        context.EmployeesServices.Add(new EmployeeService { EmployeeId = employeeId, ServiceId = 1, Service = serviceEntity });
        context.Schedules.Add(new Schedule { Id = 1, EmployeeId = employeeId, DayOfWeek = DayOfWeek.Wednesday, StartTime = "09:00", EndTime = "17:00" });
        await context.SaveChangesAsync();

        AvailabilityCheckResultViewModel result = await service.IsSlotAvailableAsync(employeeId, 1, requestedTime);

        Assert.True(result.IsAvailable);
        Assert.Contains("60", result.Message);
    }

    [Fact]
    public async Task GetEmployeeSelectListAsync_ShouldReturnOnlyActiveEmployees()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        GlowUser activeUser = new() { Id = Guid.NewGuid(), FirstName = "Active", LastName = "User", UserName = "active", NormalizedUserName = "ACTIVE", Email = "active@test.bg" };
        GlowUser deletedUser = new() { Id = Guid.NewGuid(), FirstName = "Deleted", LastName = "User", UserName = "deleted", NormalizedUserName = "DELETED", Email = "deleted@test.bg" };
        context.Users.AddRange(activeUser, deletedUser);
        context.Employees.AddRange(
            new Employee { Id = Guid.NewGuid(), UserId = activeUser.Id, User = activeUser, Occupation = "A", ExperienceYears = 1, IsDeleted = false },
            new Employee { Id = Guid.NewGuid(), UserId = deletedUser.Id, User = deletedUser, Occupation = "B", ExperienceYears = 1, IsDeleted = true });
        await context.SaveChangesAsync();

        List<SelectListItem> result = (await service.GetEmployeeSelectListAsync()).ToList();

        Assert.Single(result);
        Assert.Equal("Active User", result.First().Text);
    }

    [Fact]
    public async Task GetServiceSelectListAsync_AndGetServicesByEmployeeIdAsync_ShouldReturnOnlyActiveServices()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        Guid employeeId = Guid.NewGuid();
        ServiceEntity active = new() { Id = 1, Name = "Active", DurationInMinutes = 30, Price = 40, Points = 4 };
        ServiceEntity deleted = new() { Id = 2, Name = "Deleted", DurationInMinutes = 30, Price = 40, Points = 4, IsDeleted = true };
        context.Services.AddRange(active, deleted);
        context.EmployeesServices.AddRange(
            new EmployeeService { EmployeeId = employeeId, ServiceId = 1, Service = active },
            new EmployeeService { EmployeeId = employeeId, ServiceId = 2, Service = deleted });
        await context.SaveChangesAsync();

        List<SelectListItem> serviceList = (await service.GetServiceSelectListAsync()).ToList();
        List<SelectListItem> employeeServices = (await service.GetServicesByEmployeeIdAsync(employeeId)).ToList();

        Assert.Single(serviceList);
        Assert.Single(employeeServices);
        Assert.Equal("Active", employeeServices.First().Text);
    }

    [Fact]
    public async Task RejectProcedureAsync_ShouldCancelFutureScheduledProcedure_WhenSpecialistOwnsIt()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        Guid specialistUserId = Guid.NewGuid();
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUserId, Occupation = "Therapist", ExperienceYears = 5 };
        context.Employees.Add(employee);
        context.Procedures.Add(new Procedure
        {
            Id = 20,
            EmployeeId = employee.Id,
            Employee = employee,
            AppointmentDate = DateTime.Now.AddDays(1),
            Status = Status.Scheduled
        });
        await context.SaveChangesAsync();

        await service.CancelProcedureAsync(20, specialistUserId);

        Assert.Equal(Status.Cancelled, context.Procedures.Single().Status);
        Assert.True(context.Procedures.Single().IsDeleted);
        Assert.Equal(CancelledBy.Employee, context.Procedures.Single().CancelledBy);
    }

    [Fact]
    public async Task RejectProcedureAsync_ShouldThrow_WhenSpecialistDoesNotOwnProcedure()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        Employee employee = new() { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Occupation = "Therapist", ExperienceYears = 5 };
        context.Employees.Add(employee);
        context.Procedures.Add(new Procedure
        {
            Id = 21,
            EmployeeId = employee.Id,
            Employee = employee,
            AppointmentDate = DateTime.Now.AddDays(1),
            Status = Status.Scheduled
        });
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.CancelProcedureAsync(21, Guid.NewGuid()));
    }

    [Fact]
    public async Task RejectProcedureAsync_ShouldThrow_WhenProcedureIsPast()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        Guid specialistUserId = Guid.NewGuid();
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUserId, Occupation = "Therapist", ExperienceYears = 5 };
        context.Employees.Add(employee);
        context.Procedures.Add(new Procedure
        {
            Id = 22,
            EmployeeId = employee.Id,
            Employee = employee,
            AppointmentDate = DateTime.Now.AddHours(-2),
            Status = Status.Scheduled
        });
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CancelProcedureAsync(22, specialistUserId));
    }

    [Fact]
    public async Task GetAllProcedureDetailsByUserIdAsync_ShouldIncludeEmployeeCancelledProceduresForClient()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        Membership membership = new() { Id = 1, Title = MembershipTitle.GlowEntry, DiscountPercentage = 5, Points = 10 };
        GlowUser client = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Raya",
            LastName = "Client",
            UserName = "raya2",
            NormalizedUserName = "RAYA2",
            Email = "raya2@test.bg",
            Membership = membership,
            MembershipId = membership.Id
        };
        GlowUser specialistUser = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Miro",
            LastName = "Spec",
            UserName = "miro2",
            NormalizedUserName = "MIRO2",
            Email = "miro2@test.bg",
            IsSpecialist = true
        };
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Therapist", ExperienceYears = 7 };
        ServiceEntity serviceEntity = new() { Id = 33, Name = "Massage", DurationInMinutes = 60, Price = 100, Points = 15 };

        context.Memberships.Add(membership);
        context.Users.AddRange(client, specialistUser);
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Procedures.Add(new Procedure
        {
            Id = 30,
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

        List<DetailsProcedureViewModel> result = (await service.GetAllProcedureDetailsByUserIdAsync(client.Id)).ToList();

        Assert.Single(result);
        Assert.Equal("Отказана от специалист", result[0].Status);
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

    private static ProcedureService CreateService(GlowCareDbContext context, out TrackingUserService fakeUserService)
    {
        fakeUserService = new TrackingUserService();

        return new ProcedureService(
            new Repository<Procedure, int>(context),
            new Repository<ServiceEntity, int>(context),
            new Repository<Employee, int>(context),
            new Repository<Schedule, int>(context),
            new Repository<EmployeeService, int>(context),
            CreateUserManager(context),
            fakeUserService);
    }

    private static DateTime NextDayOfWeek(DayOfWeek dayOfWeek)
    {
        DateTime date = DateTime.Now.Date;
        while (date.DayOfWeek != dayOfWeek)
        {
            date = date.AddDays(1);
        }

        return date <= DateTime.Now.Date ? date.AddDays(7) : date;
    }

    private sealed class TrackingUserService : IUserService
    {
        public bool UpdateUserMembershipCalled { get; private set; }

        public Task<bool> AssignUserToRoleAsync(Guid userId, string roleName) => throw new NotImplementedException();
        public Task<IEnumerable<GlowCare.ViewModels.Users.AllUsersViewModel>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 5) => throw new NotImplementedException();
        public Task<int> GetTotalPagesAsync(int pageSize = 5) => throw new NotImplementedException();
        public Task<bool> UserExistsByIdAsync(Guid userId) => throw new NotImplementedException();
        public Task<bool> RemoveUserFromRoleAsync(Guid userId, string roleName) => throw new NotImplementedException();
        public Task<bool> DeleteUserAsync(Guid userId) => throw new NotImplementedException();
        public Task<GlowCare.ViewModels.Users.UserProfileViewModel> GetUserProfileAsync(Guid userId) => throw new NotImplementedException();

        public Task UpdateUserMembershipAsync(GlowUser user)
        {
            UpdateUserMembershipCalled = true;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task CancelProcedureAsync_ShouldCancelFutureScheduledProcedure_WhenUserOwnsIt()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        Guid userId = Guid.NewGuid();

        GlowUser user = new()
        {
            Id = userId,
            FirstName = "Test",
            LastName = "User",
            UserName = "testuser",
            NormalizedUserName = "TESTUSER",
            Email = "testuser@test.bg"
        };

        GlowUser specialistUser = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Spec",
            LastName = "User",
            UserName = "specuser",
            NormalizedUserName = "SPECUSER",
            Email = "specuser@test.bg",
            IsSpecialist = true
        };

        Employee employee = new()
        {
            Id = Guid.NewGuid(),
            UserId = specialistUser.Id,
            User = specialistUser,
            Occupation = "Therapist",
            ExperienceYears = 5
        };

        ServiceEntity serviceEntity = new()
        {
            Id = 1,
            Name = "Massage",
            DurationInMinutes = 60,
            Price = 100,
            Points = 10
        };

        context.Users.AddRange(user, specialistUser);
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Procedures.Add(new Procedure
        {
            Id = 30,
            UserId = userId,
            User = user,
            EmployeeId = employee.Id,
            Employee = employee,
            ServiceId = serviceEntity.Id,
            Service = serviceEntity,
            AppointmentDate = DateTime.Now.AddDays(2),
            Status = Status.Scheduled,
            IsDeleted = false
        });

        await context.SaveChangesAsync();

        await service.CancelProcedureAsync(30, userId);

        Procedure procedure = context.Procedures.Single(p => p.Id == 30);

        Assert.Equal(Status.Cancelled, procedure.Status);
        Assert.True(procedure.IsDeleted);
        Assert.Equal(CancelledBy.User, procedure.CancelledBy);
    }

    [Fact]
    public async Task CancelProcedureAsync_ShouldThrow_WhenUserDoesNotOwnProcedure()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        GlowUser owner = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Owner",
            LastName = "User",
            UserName = "owneruser",
            NormalizedUserName = "OWNERUSER",
            Email = "owneruser@test.bg"
        };

        GlowUser specialistUser = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Spec",
            LastName = "User",
            UserName = "specuser2",
            NormalizedUserName = "SPECUSER2",
            Email = "specuser2@test.bg",
            IsSpecialist = true
        };

        Employee employee = new()
        {
            Id = Guid.NewGuid(),
            UserId = specialistUser.Id,
            User = specialistUser,
            Occupation = "Therapist",
            ExperienceYears = 5
        };

        ServiceEntity serviceEntity = new()
        {
            Id = 1,
            Name = "Massage",
            DurationInMinutes = 60,
            Price = 100,
            Points = 10
        };

        context.Users.AddRange(owner, specialistUser);
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Procedures.Add(new Procedure
        {
            Id = 31,
            UserId = owner.Id,
            User = owner,
            EmployeeId = employee.Id,
            Employee = employee,
            ServiceId = serviceEntity.Id,
            Service = serviceEntity,
            AppointmentDate = DateTime.Now.AddDays(2),
            Status = Status.Scheduled,
            IsDeleted = false
        });

        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.CancelProcedureAsync(31, Guid.NewGuid()));
    }

    [Fact]
    public async Task CancelProcedureAsync_ShouldThrow_WhenProcedureIsPast()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context, out _);

        Guid userId = Guid.NewGuid();

        GlowUser user = new()
        {
            Id = userId,
            FirstName = "Past",
            LastName = "User",
            UserName = "pastuser",
            NormalizedUserName = "PASTUSER",
            Email = "pastuser@test.bg"
        };

        GlowUser specialistUser = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Spec",
            LastName = "User",
            UserName = "specuser3",
            NormalizedUserName = "SPECUSER3",
            Email = "specuser3@test.bg",
            IsSpecialist = true
        };

        Employee employee = new()
        {
            Id = Guid.NewGuid(),
            UserId = specialistUser.Id,
            User = specialistUser,
            Occupation = "Therapist",
            ExperienceYears = 5
        };

        ServiceEntity serviceEntity = new()
        {
            Id = 1,
            Name = "Massage",
            DurationInMinutes = 60,
            Price = 100,
            Points = 10
        };

        context.Users.AddRange(user, specialistUser);
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.Procedures.Add(new Procedure
        {
            Id = 32,
            UserId = userId,
            User = user,
            EmployeeId = employee.Id,
            Employee = employee,
            ServiceId = serviceEntity.Id,
            Service = serviceEntity,
            AppointmentDate = DateTime.Now.AddHours(-3),
            Status = Status.Scheduled,
            IsDeleted = false
        });

        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CancelProcedureAsync(32, userId));
    }

}
