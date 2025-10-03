using GlowCare.Entities.Contracts.Interfaces;
using GlowCare.Entities.Models;
using Portify.Entities.Repositories;

namespace GlowCare.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IRepository<Procedure, int>, Repository<Procedure, int>>();

        return services;
    }
}

