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

        return services;
    }

    public static IServiceCollection RegisterUserDefinedServices(
            this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IReviewService, ReviewService>();

        return services;
    }
}

