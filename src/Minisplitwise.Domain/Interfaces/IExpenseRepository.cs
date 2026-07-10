using Minisplitwise.Domain.Entities;

namespace Minisplitwise.Domain.Interfaces;

public interface IExpenseRepository
{
    Task<Expense> CreateExpenseAsync(Expense expense, CancellationToken cancellationToken = default);
    Task<List<Expense>> GetExpensesByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<List<Expense>> GetExpensesByMemberIdAsync(Guid memberId, Guid groupId, CancellationToken cancellationToken = default);
}