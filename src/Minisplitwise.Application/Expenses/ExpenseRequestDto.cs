namespace Minisplitwise.Application.Expenses;

public record ExpenseRequestDto(
    string Description,
    decimal Amount,
    DateTime Date,
    Guid GroupId,
    Guid PaidById,
    bool ForEveryone,
    List<Guid> SharedWithIds
);