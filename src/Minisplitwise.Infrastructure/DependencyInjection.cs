using Microsoft.Extensions.DependencyInjection;
using Minisplitwise.Domain.Interfaces;
using Minisplitwise.Infrastructure.Data.Repositories;

namespace Minisplitwise.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        //change to scoped when implementing database
        services.AddSingleton<IMemberRepository, MemberRepository>();
        
        return services;
    }
}