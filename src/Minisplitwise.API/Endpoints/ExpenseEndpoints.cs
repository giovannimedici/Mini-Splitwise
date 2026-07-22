using Minisplitwise.Application.Expenses;
using Minisplitwise.Application.Interfaces;
using FluentValidation;
using QuickJwt.AspNetCore;

namespace Minisplitwise.API.Endpoints;

public static class ExpenseEndpoints
{
    public static void MapExpenseEndpoints(this IEndpointRouteBuilder app)
    {
        var expenses = app.MapGroup("/expenses")
                        .AddEndpointFilter<JwtEndpointFilter>();

        expenses.MapPost("", CreateExpenseAsync)
            .WithName("CreateExpense")
            .WithTags("Expenses")
            .WithSummary("Create a new expense")
            .WithDescription("Create a new expense with the given description, amount, date, group id, paid by id and shared with ids");
        
        expenses.MapGet("/{groupId}", GetExpensesByGroupIdAsync)
            .WithName("GetExpensesByGroupId")
            .WithTags("Expenses")
            .WithSummary("Get all expenses by group id")
            .WithDescription("Get all expenses by group id");
        
        expenses.MapGet("/{memberId}/{groupId}", GetExpensesByMemberIdAsync)
            .WithName("GetExpensesByMemberId")
            .WithTags("Expenses")
            .WithSummary("Get all expenses by member id and group id")
            .WithDescription("Get all expenses by member id and group id");
            
        expenses.MapGet("/{groupId}/payments", CalculatePaymentsByGroupIdAsync)
            .WithName("CalculatePaymentsByGroupId")
            .WithTags("Expenses")
            .WithSummary("Calculate payments by group id")
            .WithDescription("Calculate payments by group id");
    }

    public static async Task<IResult> CreateExpenseAsync(
        ExpenseRequestDto expenseRequestDto, 
        IExpenseService expenseService, 
        IValidator<ExpenseRequestDto> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(expenseRequestDto, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary(), "Validation error");
        }

        var expense = await expenseService.CreateExpenseAsync(expenseRequestDto, cancellationToken);

        return Results.Created($"/expenses/{expense.Id}", expense);
    }

    public static async Task<IResult> GetExpensesByGroupIdAsync(Guid groupId, IExpenseService expenseService, CancellationToken cancellationToken)
    {
        var expenses = await expenseService.GetExpensesByGroupIdAsync(groupId, cancellationToken);

        return Results.Ok(expenses);
    }

    public static async Task<IResult> GetExpensesByMemberIdAsync(Guid memberId, Guid groupId, IExpenseService expenseService, CancellationToken cancellationToken)
    {
        var expenses = await expenseService.GetExpensesByMemberIdAsync(memberId, groupId, cancellationToken);

        return Results.Ok(expenses);
    }

    public static async Task<IResult> CalculatePaymentsByGroupIdAsync(Guid groupId, IExpenseService expenseService, CancellationToken cancellationToken)
    {
        var payments = await expenseService.CalculatePaymentsByGroupIdAsync(groupId, cancellationToken);

        return Results.Ok(payments);
    }
}