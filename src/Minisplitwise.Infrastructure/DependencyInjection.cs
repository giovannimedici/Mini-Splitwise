using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minisplitwise.Domain.Interfaces;
using Minisplitwise.Infrastructure.Data;
using Minisplitwise.Infrastructure.Data.Repositories;

namespace Minisplitwise.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connStr = configuration
            .GetConnectionString("MinisplitwiseDb")
            ?? throw new InvalidOperationException("Connection string 'MinisplitwiseDb' not found.");

        services.AddDbContext<MinisplitwiseDbContext>(options =>
            options.UseSqlite(connStr)
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        
        return services;
    }
}