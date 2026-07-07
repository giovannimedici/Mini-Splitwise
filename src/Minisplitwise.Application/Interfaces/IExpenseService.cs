using Minisplitwise.Application.Expenses;

namespace Minisplitwise.Application.Interfaces;

public interface IExpenseService
{
    Task<ExpenseResponseDto> CreateExpenseAsync(ExpenseRequestDto expenseRequestDto, CancellationToken cancellationToken = default);
    Task<List<ExpenseResponseDto>> GetExpensesByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
}