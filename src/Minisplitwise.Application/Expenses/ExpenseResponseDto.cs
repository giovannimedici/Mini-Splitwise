using Minisplitwise.Application.Groups;
using Minisplitwise.Application.Members;

namespace Minisplitwise.Application.Expenses;

public record ExpenseResponseDto(
    Guid Id,
    string Description,
    decimal Amount,
    DateTime Date,
    GroupDto Group,
    MemberDto PaidBy,
    bool ForEveryone,
    List<MemberDto> SharedWith
);