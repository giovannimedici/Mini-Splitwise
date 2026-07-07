using Minisplitwise.Application.Expenses;
using Minisplitwise.Application.Interfaces;

namespace Minisplitwise.API.Endpoints;

public static class ExpenseEndpoints
{
    public static void MapExpenseEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/expenses", CreateExpenseAsync)
            .WithName("CreateExpense")
            .WithTags("Expenses")
            .WithSummary("Create a new expense")
            .WithDescription("Create a new expense with the given description, amount, date, group id, paid by id and shared with ids");
        
        app.MapGet("/expenses/{groupId}", GetExpensesByGroupIdAsync)
            .WithName("GetExpensesByGroupId")
            .WithTags("Expenses")
            .WithSummary("Get all expenses by group id")
            .WithDescription("Get all expenses by group id");
    }

    public static async Task<IResult> CreateExpenseAsync(ExpenseRequestDto expenseRequestDto, IExpenseService expenseService, CancellationToken cancellationToken)
    {
        var expense = await expenseService.CreateExpenseAsync(expenseRequestDto, cancellationToken);

        return Results.Created($"/expenses/{expense.Id}", expense);
    }

    public static async Task<IResult> GetExpensesByGroupIdAsync(Guid groupId, IExpenseService expenseService, CancellationToken cancellationToken)
    {
        var expenses = await expenseService.GetExpensesByGroupIdAsync(groupId, cancellationToken);

        return Results.Ok(expenses);
    }
}