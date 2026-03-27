using GlowCare.Core.Contracts;
using GlowCare.Core.Implementations;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Portify.Entities.Repositories;

namespace GlowCare.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IRepository<Procedure, int>, Repository<Procedure, int>>();
        services.AddScoped<IRepository<GlowUser, Guid>, Repository<GlowUser, Guid>>();
        services.AddScoped<IRepository<Review, int>, Repository<Review, int>>();
        services.AddScoped<IRepository<Service, int>, Repository<Service, int>>();
        //services.AddScoped<IRepository<EmployeeSchedule, int>, Repository<EmployeeSchedule, int>>();
        services.AddScoped<IRepository<GlowCare.Entities.Models.EmployeeService, int>, Repository<GlowCare.Entities.Models.EmployeeService, int>>();
        services.AddScoped<IRepository<Employee, int>, Repository<Employee, int>>();
        services.AddScoped<IRepository<Schedule, int>, Repository<Schedule, int>>();

        return services;
    }

    public static IServiceCollection RegisterUserDefinedServices(
            this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProcedureService, ProcedureService>();
        services.AddScoped<IEmployeeService, GlowCare.Core.Implementations.EmployeeService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IServiceService, ServiceService>();

        return services;
    }
}

