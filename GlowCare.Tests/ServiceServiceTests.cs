using GlowCare.Core.Implementations;
using GlowCare.Entities;
using GlowCare.Entities.Models;
using GlowCare.Entities.Models.Enums;
using GlowCare.Entities.Repositories;
using GlowCare.ViewModels.Services;
using Microsoft.EntityFrameworkCore;
using ServiceEntity = GlowCare.Entities.Models.Service;
using Xunit;
using EmployeeService = GlowCare.Entities.Models.EmployeeService;

namespace GlowCare.Tests;

public class ServiceServiceTests
{
    [Fact]
    public async Task CreateServiceAsync_ShouldThrow_WhenModelIsNull()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context);

        await Assert.ThrowsAsync<NullReferenceException>(() => service.CreateServiceAsync(null!, Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateServiceAsync_ShouldCreateServiceWithoutAssignments_WhenNoEmployeesAreSelected()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context);

        AddServiceViewModel model = new()
        {
            Name = "Hydra",
            CategoryId = 1,
            Description = "Desc",
            DurationInMinutes = 45,
            Price = 70,
            Points = 7,
            SelectedEmployeeIds = new List<Guid>()
        };

        await service.CreateServiceAsync(model, Guid.NewGuid());

        Assert.Single(context.Services);
        Assert.Empty(context.EmployeesServices);
    }

    [Fact]
    public async Task CreateServiceAsync_ShouldCreateAssignmentsOnlyForValidSpecialists()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context);

        GlowUser validUser = new() { Id = Guid.NewGuid(), FirstName = "Valid", LastName = "Spec", UserName = "valid", NormalizedUserName = "VALID", Email = "valid@test.bg", IsSpecialist = true };
        GlowUser invalidUser = new() { Id = Guid.NewGuid(), FirstName = "Invalid", LastName = "User", UserName = "invalid", NormalizedUserName = "INVALID", Email = "invalid@test.bg", IsSpecialist = false };
        Employee validEmployee = new() { Id = Guid.NewGuid(), UserId = validUser.Id, User = validUser, Occupation = "Therapist", ExperienceYears = 5 };
        Employee invalidEmployee = new() { Id = Guid.NewGuid(), UserId = invalidUser.Id, User = invalidUser, Occupation = "Assistant", ExperienceYears = 2 };

        context.Users.AddRange(validUser, invalidUser);
        context.Employees.AddRange(validEmployee, invalidEmployee);
        await context.SaveChangesAsync();

        AddServiceViewModel model = new()
        {
            Name = "Massage",
            CategoryId = 1,
            Description = "Desc",
            DurationInMinutes = 60,
            Price = 100,
            Points = 10,
            SelectedEmployeeIds = new List<Guid> { validEmployee.Id, invalidEmployee.Id, Guid.Empty, validEmployee.Id }
        };

        await service.CreateServiceAsync(model, Guid.NewGuid());

        Assert.Single(context.Services);
        EmployeeService assignment = Assert.Single(context.EmployeesServices);
        Assert.Equal(validEmployee.Id, assignment.EmployeeId);
    }

    [Fact]
    public async Task DeleteServiceAsync_ShouldSoftDeleteService()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context);

        context.Services.Add(new ServiceEntity { Id = 1, Name = "Delete", CategoryId = 1, DurationInMinutes = 30, Price = 40, Points = 4 });
        await context.SaveChangesAsync();

        await service.DeleteServiceAsync(new DeleteServiceViewModel { Id = 1 });

        Assert.True(context.Services.Single().IsDeleted);
    }

    [Fact]
    public async Task EditServiceAsync_ShouldUpdateServiceAndAssignments()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context);

        GlowUser oldUser = new() { Id = Guid.NewGuid(), FirstName = "Old", LastName = "Spec", UserName = "old", NormalizedUserName = "OLD", Email = "old@test.bg", IsSpecialist = true };
        GlowUser newUser = new() { Id = Guid.NewGuid(), FirstName = "New", LastName = "Spec", UserName = "new", NormalizedUserName = "NEW", Email = "new@test.bg", IsSpecialist = true };
        Employee oldEmployee = new() { Id = Guid.NewGuid(), UserId = oldUser.Id, User = oldUser, Occupation = "Therapist", ExperienceYears = 3 };
        Employee newEmployee = new() { Id = Guid.NewGuid(), UserId = newUser.Id, User = newUser, Occupation = "Therapist", ExperienceYears = 4 };
        ServiceEntity serviceEntity = new() { Id = 1, Name = "OldName", CategoryId = 1, Description = "Old", DurationInMinutes = 30, Price = 40, Points = 4 };

        context.Users.AddRange(oldUser, newUser);
        context.Employees.AddRange(oldEmployee, newEmployee);
        context.Services.Add(serviceEntity);
        context.EmployeesServices.Add(new EmployeeService { EmployeeId = oldEmployee.Id, ServiceId = serviceEntity.Id, Employee = oldEmployee, Service = serviceEntity });
        await context.SaveChangesAsync();

        EditServiceViewModel model = new()
        {
            Id = 1,
            Name = "NewName",
            CategoryId = 2,
            Description = "New",
            DurationInMinutes = 90,
            Price = 150,
            Points = 15,
            SelectedEmployeeIds = new List<Guid> { newEmployee.Id }
        };

        await service.EditServiceAsync(model);

        ServiceEntity updated = context.Services.Include(s => s.EmployeeServices).Single();
        Assert.Equal("NewName", updated.Name);
        Assert.Equal(2, updated.CategoryId);
        Assert.Single(updated.EmployeeServices);
        Assert.Equal(newEmployee.Id, updated.EmployeeServices.Single().EmployeeId);
    }

    [Fact]
    public async Task GetEditServiceAsync_ShouldReturnModelWithSelectedEmployeesAndLookupData()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context);

        Category category = new() { Id = 1, Name = "Face" };
        GlowUser specialistUser = new() { Id = Guid.NewGuid(), FirstName = "Anna", LastName = "Spec", UserName = "anna", NormalizedUserName = "ANNA", Email = "anna@test.bg", IsSpecialist = true };
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Expert", ExperienceYears = 8 };
        ServiceEntity serviceEntity = new() { Id = 1, Name = "Hydra", CategoryId = 1, Description = "Desc", DurationInMinutes = 40, Price = 70, Points = 7 };

        context.Categories.Add(category);
        context.Users.Add(specialistUser);
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.EmployeesServices.Add(new EmployeeService { EmployeeId = employee.Id, ServiceId = serviceEntity.Id, Employee = employee, Service = serviceEntity });
        await context.SaveChangesAsync();

        EditServiceViewModel result = await service.GetEditServiceAsync(1);

        Assert.Equal(1, result.Id);
        Assert.Single(result.SelectedEmployeeIds!);
        Assert.Single(result.Categories);
        Assert.Single(result.Specialists);
        Assert.True(result.Specialists.First().IsSelected);
    }

    [Fact]
    public async Task GetAdminServiceManagementViewModelAsync_ShouldFilterAndPaginate()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context);

        context.Categories.Add(new Category { Id = 1, Name = "Body" });
        context.Services.AddRange(
            new ServiceEntity { Id = 1, Name = "Alpha Massage", CategoryId = 1, DurationInMinutes = 60, Price = 80, Points = 8 },
            new ServiceEntity { Id = 2, Name = "Beta Facial", CategoryId = 1, DurationInMinutes = 45, Price = 60, Points = 6 },
            new ServiceEntity { Id = 3, Name = "Gamma Spa", CategoryId = 1, DurationInMinutes = 90, Price = 150, Points = 15 });
        await context.SaveChangesAsync();

        AdminServiceManagementViewModel result = await service.GetAdminServiceManagementViewModelAsync("a", 2, 1);

        Assert.Equal(2, result.CurrentPage);
        Assert.Equal(3, result.TotalServices);
        Assert.Single(result.Services);
        Assert.NotEmpty(result.Categories);
    }

    [Fact]
    public async Task GetAllServicesAsync_ShouldReturnOnlyActiveServices()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context);

        context.Categories.Add(new Category { Id = 1, Name = "Body" });
        context.Services.AddRange(
            new ServiceEntity { Id = 1, Name = "Active", CategoryId = 1, DurationInMinutes = 60, Price = 80, Points = 8 },
            new ServiceEntity { Id = 2, Name = "Deleted", CategoryId = 1, DurationInMinutes = 60, Price = 80, Points = 8, IsDeleted = true });
        await context.SaveChangesAsync();

        List<ServiceInfoViewModel> result = (await service.GetAllServicesAsync()).ToList();

        Assert.Single(result);
        Assert.Equal("Active", result.First().Name);
        Assert.Equal("Body", result.First().CategoryName);
    }

    [Fact]
    public async Task GetAllServicesForAdminAsync_ShouldMapAssignedSpecialists()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context);

        Category category = new() { Id = 1, Name = "Face" };
        GlowUser specialistUser = new() { Id = Guid.NewGuid(), FirstName = "Moni", LastName = "Spec", UserName = "moni", NormalizedUserName = "MONI", Email = "moni@test.bg", IsSpecialist = true };
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Expert", ExperienceYears = 8 };
        ServiceEntity serviceEntity = new() { Id = 1, Name = "Hydra", CategoryId = 1, DurationInMinutes = 40, Price = 70, Points = 7 };

        context.Categories.Add(category);
        context.Users.Add(specialistUser);
        context.Employees.Add(employee);
        context.Services.Add(serviceEntity);
        context.EmployeesServices.Add(new EmployeeService { EmployeeId = employee.Id, ServiceId = serviceEntity.Id, Employee = employee, Service = serviceEntity });
        await context.SaveChangesAsync();

        List<AdminServiceListItemViewModel> result = (await service.GetAllServicesForAdminAsync()).ToList();

        Assert.Single(result);
        Assert.Single(result.First().AssignedSpecialists);
        Assert.Equal("Moni Spec", result.First().AssignedSpecialists.First());
    }

    [Fact]
    public async Task GetFilteredServicesAsync_ShouldFilterByCategoryPriceAndAvailability()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context);

        DateTime tomorrowAtTen = DateTime.Now.Date.AddDays(1).AddHours(10);
        if (tomorrowAtTen <= DateTime.Now)
        {
            tomorrowAtTen = tomorrowAtTen.AddDays(1);
        }

        GlowUser specialistUser = new() { Id = Guid.NewGuid(), FirstName = "Lora", LastName = "Spec", UserName = "lora", NormalizedUserName = "LORA", Email = "lora@test.bg", IsSpecialist = true };
        Employee employee = new() { Id = Guid.NewGuid(), UserId = specialistUser.Id, User = specialistUser, Occupation = "Therapist", ExperienceYears = 6 };
        Category category = new() { Id = 1, Name = "Body" };
        ServiceEntity matching = new() { Id = 1, Name = "Budget Massage", CategoryId = 1, Category = category, DurationInMinutes = 60, Price = 49, Points = 4 };
        ServiceEntity nonMatching = new() { Id = 2, Name = "Premium Facial", CategoryId = 1, Category = category, DurationInMinutes = 60, Price = 120, Points = 12 };

        context.Users.Add(specialistUser);
        context.Employees.Add(employee);
        context.Categories.Add(category);
        context.Services.AddRange(matching, nonMatching);
        context.EmployeesServices.Add(new EmployeeService { EmployeeId = employee.Id, Employee = employee, ServiceId = matching.Id, Service = matching });
        context.Schedules.Add(new Schedule { Id = 1, EmployeeId = employee.Id, DayOfWeek = tomorrowAtTen.DayOfWeek, StartTime = "10:00", EndTime = "18:00" });
        await context.SaveChangesAsync();

        List<ServiceInfoViewModel> result = (await service.GetFilteredServicesAsync(1, "under50", "thisWeek")).ToList();

        Assert.Single(result);
        Assert.Equal("Budget Massage", result.First().Name);
        Assert.True(result.First().EarliestAvailableSlot.HasValue);
    }

    [Fact]
    public async Task GetCategoryOptionsAsync_ShouldReturnOrderedCategories()
    {
        using GlowCareDbContext context = CreateContext();
        var service = CreateService(context);

        context.Categories.AddRange(
            new Category { Id = 1, Name = "Zeta" },
            new Category { Id = 2, Name = "Alpha" });
        await context.SaveChangesAsync();

        List<ServiceCategoryOptionViewModel> result = (await service.GetCategoryOptionsAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Alpha", result.First().Name);
        Assert.Equal("Zeta", result[1].Name);
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

    private static ServiceService CreateService(GlowCareDbContext context)
    {
        return new ServiceService(
            context,
            new Repository<ServiceEntity, int>(context),
            new Repository<Category, int>(context),
            new Repository<Employee, Guid>(context),
            new Repository<EmployeeService, int>(context),
            new Repository<Schedule, int>(context),
            new Repository<Procedure, int>(context));
    }
}
