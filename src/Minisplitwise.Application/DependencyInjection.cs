using Microsoft.Extensions.DependencyInjection;
using Minisplitwise.Application.Services;
using Minisplitwise.Application.Services.Interfaces;

namespace Minisplitwise.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMemberService, MemberService>();
        
        return services;
    }
}