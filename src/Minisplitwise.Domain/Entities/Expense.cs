namespace Minisplitwise.Domain.Entities;

public sealed class Expense{
    public Guid Id { get; private set; }
    public string Description { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public Group Group { get; private set; } = default!;
    public Member PaidBy { get; private set; } = default!;
    public bool ForEveryone { get; private set; }
    public List<Member> SharedWith { get; private set; } = new();

    private Expense() {}
    public static Expense Create(string description, decimal amount, DateTime date, Group group, Member paidBy, bool forEveryone, List<Member> sharedBy, List<Member> sharedWith){
        return new Expense{
            Id = Guid.NewGuid(),
            Description = description,
            Amount = amount,
            Date = date,
            Group = group,
            PaidBy = paidBy,
            ForEveryone = forEveryone,
            SharedWith = sharedWith
        };
    }
}