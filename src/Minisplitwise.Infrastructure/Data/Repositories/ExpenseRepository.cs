using System.Collections.Concurrent;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Interfaces;

namespace Minisplitwise.Infrastructure.Data.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ConcurrentDictionary<Guid, Expense> _expenses = new();

    public async Task<Expense> CreateExpenseAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        await Task.FromResult(_expenses.TryAdd(expense.Id, expense));

        return expense;
    }

    public async Task<List<Expense>> GetExpensesByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_expenses.Values.Where(e => e.Group.Id == groupId).ToList());
    }

    public async Task<List<Expense>> GetExpensesByMemberIdAsync(Guid memberId, Guid groupId, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_expenses.Values.Where(e => e.Group.Id == groupId && e.SharedWith.Any(m => m.Id == memberId)).ToList());
    }
}