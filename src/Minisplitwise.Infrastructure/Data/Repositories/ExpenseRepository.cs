using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Exceptions;
using Minisplitwise.Domain.Interfaces;

namespace Minisplitwise.Infrastructure.Data.Repositories;

public class ExpenseRepository (MinisplitwiseDbContext context) : IExpenseRepository
{
    public async Task<Expense> CreateExpenseAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        await context.Expenses.AddAsync(expense, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return expense;
    }

    public async Task<List<Expense>> GetExpensesByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var expenses = await context.Expenses
                                    .Where(e => e.Group.Id == groupId)
                                    .Include(e => e.Group)
                                    .Include(e => e.PaidBy)
                                    .Include(e => e.SharedWith)
                                    .ToListAsync(cancellationToken);

        if(expenses.Count == 0)
        {
            throw new NotFoundException("No expenses found for the given group id");
        }

        return expenses;
    }

    public async Task<List<Expense>> GetExpensesByMemberIdAsync(Guid memberId, Guid groupId, CancellationToken cancellationToken = default)
    {
        return await context.Expenses.Where(e => e.Group.Id == groupId && e.SharedWith.Any(m => m.Id == memberId)).ToListAsync(cancellationToken);
    }
}