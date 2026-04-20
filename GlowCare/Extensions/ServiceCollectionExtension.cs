using GlowCare.Core.Contracts;
using GlowCare.Core.Implementations;
using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Repositories;
using GlowCare.Services.Implementations;

namespace GlowCare.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterRepositories(
        this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

        return services;
    }

    public static IServiceCollection RegisterUserDefinedServices(
            this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEmployeeService, GlowCare.Core.Implementations.EmployeeService>();
        services.AddScoped<IProcedureService, ProcedureService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<ISpecialistApplicationService, SpecialistApplicationService>();

        return services;
    }
}

