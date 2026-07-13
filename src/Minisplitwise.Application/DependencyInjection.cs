using Microsoft.Extensions.DependencyInjection;
using Minisplitwise.Application.Services;
using Minisplitwise.Application.Interfaces;
using Minisplitwise.Application.Validators;
using FluentValidation;

namespace Minisplitwise.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddValidatorsFromAssemblyContaining<MemberValidator>();
        services.AddValidatorsFromAssemblyContaining<GroupValidator>();
        services.AddValidatorsFromAssemblyContaining<ExpenseValidator>();
        
        return services;
    }
}