using System;
using System.Linq;
using System.Threading.Tasks;
using GlowCare.Entities;
using GlowCare.Entities.Models;
using GlowCare.Entities.Repositories;
using Microsoft.EntityFrameworkCore;
using ServiceEntity = GlowCare.Entities.Models.Service;
using EmployeeServiceEntity = GlowCare.Entities.Models.EmployeeService;
using Xunit;

namespace GlowCare.Tests;

public class EmployeeServiceTests
{
    [Fact]
    public async Task GetEmployeeByIdAsync_ShouldReturnNull_WhenEmployeeDoesNotExist()
    {
        using GlowCareDbContext context = CreateContext();

        var service = CreateService(context);

        var result = await service.GetEmployeeByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_ShouldReturnMappedEmployee_WhenEmployeeExists()
    {
        using GlowCareDbContext context = CreateContext();

        GlowUser user = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Ivan",
            LastName = "Petrov",
            Email = "ivan@test.bg",
            PhoneNumber = "0888123456",
            UserName = "ivan@test.bg",
            NormalizedUserName = "IVAN@TEST.BG",
            IsSpecialist = true
        };

        Employee employee = new()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Occupation = "Massage Therapist",
            ExperienceYears = 7,
            Biography = "Bio"
        };

        ServiceEntity keptService = new()
        {
            Id = 1,
            Name = "Deep Tissue",
            DurationInMinutes = 60,
            Price = 90,
            Points = 9,
            IsDeleted = false
        };

        ServiceEntity deletedService = new()
        {
            Id = 2,
            Name = "Old Service",
            DurationInMinutes = 30,
            Price = 30,
            Points = 3,
            IsDeleted = true
        };

        context.Users.Add(user);
        context.Employees.Add(employee);
        context.Services.AddRange(keptService, deletedService);
        context.EmployeesServices.AddRange(
            new EmployeeServiceEntity
            {
                EmployeeId = employee.Id,
                Employee = employee,
                ServiceId = keptService.Id,
                Service = keptService
            },
            new EmployeeServiceEntity
            {
                EmployeeId = employee.Id,
                Employee = employee,
                ServiceId = deletedService.Id,
                Service = deletedService
            });

        context.Schedules.AddRange(
            new Schedule
            {
                Id = 1,
                EmployeeId = employee.Id,
                DayOfWeek = DayOfWeek.Friday,
                StartTime = "10:00",
                EndTime = "18:00"
            },
            new Schedule
            {
                Id = 2,
                EmployeeId = employee.Id,
                DayOfWeek = DayOfWeek.Monday,
                StartTime = "09:00",
                EndTime = "17:00"
            });

        context.Reviews.AddRange(
            new Review
            {
                Id = 1,
                EmployeeId = employee.Id,
                Rating = 5,
                Comment = "Great",
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                Id = 2,
                EmployeeId = employee.Id,
                Rating = 3,
                Comment = "Ok",
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                Id = 3,
                EmployeeId = employee.Id,
                Rating = 1,
                Comment = "Deleted",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = true
            });

        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetEmployeeByIdAsync(employee.Id);

        Assert.NotNull(result);
        Assert.Equal(employee.Id, result!.Id);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal("Ivan Petrov", result.FullName);
        Assert.Equal(4, result.AverageRating);
        Assert.Equal(2, result.ReviewsCount);

        var services = result.Services.ToList();
        Assert.Single(services);
        Assert.Equal("Deep Tissue", services[0]);

        var workingHours = result.WorkingHours.ToList();
        Assert.Equal(2, workingHours.Count);
        Assert.Equal("Понеделник", workingHours[0].DayOfWeek);
        Assert.Equal("Петък", workingHours[1].DayOfWeek);
    }

    [Fact]
    public async Task GetEmployeesForIndexAsync_ShouldTrimFiltersAndBuildAvailableServices()
    {
        using GlowCareDbContext context = CreateContext();

        Category category = new()
        {
            Id = 1,
            Name = "Body"
        };

        GlowUser user1 = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Maria",
            LastName = "Ivanova",
            Email = "maria@test.bg",
            UserName = "maria@test.bg",
            NormalizedUserName = "MARIA@TEST.BG",
            IsSpecialist = true
        };

        GlowUser user2 = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Petar",
            LastName = "Dimitrov",
            Email = "petar@test.bg",
            UserName = "petar@test.bg",
            NormalizedUserName = "PETAR@TEST.BG",
            IsSpecialist = true
        };

        GlowUser deletedUser = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Deleted",
            LastName = "User",
            Email = "deleted@test.bg",
            UserName = "deleted@test.bg",
            NormalizedUserName = "DELETED@TEST.BG",
            IsSpecialist = true,
            IsDeleted = true
        };

        Employee employee1 = new()
        {
            Id = Guid.NewGuid(),
            UserId = user1.Id,
            User = user1,
            Occupation = "Massage therapist",
            ExperienceYears = 4
        };

        Employee employee2 = new()
        {
            Id = Guid.NewGuid(),
            UserId = user2.Id,
            User = user2,
            Occupation = "Cosmetologist",
            ExperienceYears = 6
        };

        Employee deletedEmployee = new()
        {
            Id = Guid.NewGuid(),
            UserId = deletedUser.Id,
            User = deletedUser,
            Occupation = "Should not appear",
            ExperienceYears = 2
        };

        ServiceEntity massage = new()
        {
            Id = 1,
            CategoryId = category.Id,
            Category = category,
            Name = "Massage",
            DurationInMinutes = 60,
            Price = 100,
            Points = 10
        };

        ServiceEntity facial = new()
        {
            Id = 2,
            CategoryId = category.Id,
            Category = category,
            Name = "Facial",
            DurationInMinutes = 45,
            Price = 70,
            Points = 7
        };

        context.Categories.Add(category);
        context.Users.AddRange(user1, user2, deletedUser);
        context.Employees.AddRange(employee1, employee2, deletedEmployee);
        context.Services.AddRange(massage, facial);
        context.EmployeesServices.AddRange(
            new EmployeeServiceEntity
            {
                EmployeeId = employee1.Id,
                Employee = employee1,
                ServiceId = massage.Id,
                Service = massage
            },
            new EmployeeServiceEntity
            {
                EmployeeId = employee1.Id,
                Employee = employee1,
                ServiceId = facial.Id,
                Service = facial
            },
            new EmployeeServiceEntity
            {
                EmployeeId = employee2.Id,
                Employee = employee2,
                ServiceId = facial.Id,
                Service = facial
            });

        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetEmployeesForIndexAsync(" maria ", " facial ");

        var employees = result.Employees.ToList();
        var availableServices = result.AvailableServices.ToList();

        Assert.Single(employees);
        Assert.Equal("Maria Ivanova", employees[0].FullName);
        Assert.Equal("maria", result.SearchTerm);
        Assert.Equal("facial", result.SelectedService);

        Assert.Equal(2, availableServices.Count);
        Assert.Contains("Facial", availableServices);
        Assert.Contains("Massage", availableServices);
    }

    [Fact]
    public async Task GetEmployeesForIndexAsync_ShouldIgnoreNonSpecialists()
    {
        using GlowCareDbContext context = CreateContext();

        Category category = new()
        {
            Id = 1,
            Name = "Face"
        };

        GlowUser specialistUser = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Ani",
            LastName = "Dimitrova",
            Email = "ani@test.bg",
            UserName = "ani@test.bg",
            NormalizedUserName = "ANI@TEST.BG",
            IsSpecialist = true
        };

        GlowUser regularUser = new()
        {
            Id = Guid.NewGuid(),
            FirstName = "Niki",
            LastName = "Dimitrov",
            Email = "niki@test.bg",
            UserName = "niki@test.bg",
            NormalizedUserName = "NIKI@TEST.BG",
            IsSpecialist = false
        };

        Employee specialist = new()
        {
            Id = Guid.NewGuid(),
            UserId = specialistUser.Id,
            User = specialistUser,
            Occupation = "Cosmetician",
            ExperienceYears = 8
        };

        Employee regular = new()
        {
            Id = Guid.NewGuid(),
            UserId = regularUser.Id,
            User = regularUser,
            Occupation = "Assistant",
            ExperienceYears = 2
        };

        ServiceEntity massage = new()
        {
            Id = 1,
            CategoryId = category.Id,
            Category = category,
            Name = "Massage",
            DurationInMinutes = 60,
            Price = 100,
            Points = 10
        };

        context.Categories.Add(category);
        context.Users.AddRange(specialistUser, regularUser);
        context.Employees.AddRange(specialist, regular);
        context.Services.Add(massage);
        context.EmployeesServices.AddRange(
            new EmployeeServiceEntity
            {
                EmployeeId = specialist.Id,
                Employee = specialist,
                ServiceId = massage.Id,
                Service = massage
            },
            new EmployeeServiceEntity
            {
                EmployeeId = regular.Id,
                Employee = regular,
                ServiceId = massage.Id,
                Service = massage
            });

        await context.SaveChangesAsync();

        var service = CreateService(context);

        var result = await service.GetEmployeesForIndexAsync(null, "Massage");
        var employees = result.Employees.ToList();

        Assert.Single(employees);
        Assert.Equal(specialist.Id, employees[0].Id);
        Assert.Equal("Ani Dimitrova", employees[0].FullName);
    }

    private static GlowCareDbContext CreateContext()
    {
        DbContextOptions<GlowCareDbContext> options = new DbContextOptionsBuilder<GlowCareDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        GlowCareDbContext context = new(options);
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

    private static GlowCare.Core.Implementations.EmployeeService CreateService(GlowCareDbContext context)
    {
        return new GlowCare.Core.Implementations.EmployeeService(
            new Repository<Employee, Guid>(context),
            new Repository<EmployeeServiceEntity, int>(context),
            new Repository<Review, int>(context));
    }
}
