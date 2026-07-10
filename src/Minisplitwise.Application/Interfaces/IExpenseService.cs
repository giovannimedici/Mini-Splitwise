using Minisplitwise.Application.Expenses;
using Minisplitwise.Application.Payments;

namespace Minisplitwise.Application.Interfaces;

public interface IExpenseService
{
    Task<ExpenseResponseDto> CreateExpenseAsync(ExpenseRequestDto expenseRequestDto, CancellationToken cancellationToken = default);
    Task<List<ExpenseResponseDto>> GetExpensesByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<List<ExpenseMemberResponseDto>> GetExpensesByMemberIdAsync(Guid memberId, Guid groupId, CancellationToken cancellationToken = default);
    Task<List<PaymentResponseDto>> CalculatePaymentsByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
}