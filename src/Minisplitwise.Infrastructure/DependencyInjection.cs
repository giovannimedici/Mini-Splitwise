using Microsoft.Extensions.DependencyInjection;
using Minisplitwise.Application.Interfaces;
using Minisplitwise.Infrastructure.Data.Repositories;

namespace Minisplitwise.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IMemberRepository, MemberRepository>();
        
        return services;
    }
}