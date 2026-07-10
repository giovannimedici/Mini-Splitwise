using Minisplitwise.Application.Members;

namespace Minisplitwise.Application.Expenses;

public record ExpenseMemberResponseDto(
    Guid Id,
    string Description,
    decimal Amount,
    DateTime Date,
    MemberDto Member,
    MemberDto PaidBy
);