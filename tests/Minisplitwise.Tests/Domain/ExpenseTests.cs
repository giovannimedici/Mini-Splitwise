using Minisplitwise.Domain.Entities;

namespace MiniSplitwise.Tests.Domain;

public class ExpenseTests
{
    private static Member CreateMember(string name = "John", string email = "john@example.com")
        => Member.Create(name, email, new DateTime(1990, 1, 1));

    private static Group CreateGroup(params Member[] members)
        => Group.Create("Trip", members.ToList());

    [Fact]
    public void Create_WithValidData_ReturnsExpense()
    {
        var payer = CreateMember("John", "john@example.com");
        var other = CreateMember("Jane", "jane@example.com");
        var group = CreateGroup(payer, other);
        var date = new DateTime(2026, 7, 1);
        var sharedWith = new List<Member> { other };

        var expense = Expense.Create(
            "Dinner", 100m, date, group, payer,
            forEveryone: false,
            sharedWith: sharedWith);

        Assert.NotEqual(Guid.Empty, expense.Id);
        Assert.Equal("Dinner", expense.Description);
        Assert.Equal(100m, expense.Amount);
        Assert.Equal(date, expense.Date);
        Assert.Same(group, expense.Group);
        Assert.Same(payer, expense.PaidBy);
        Assert.False(expense.ForEveryone);
        Assert.Equal(sharedWith, expense.SharedWith);
    }

    [Fact]
    public void Create_ForEveryone_SetsFlag()
    {
        var payer = CreateMember("John", "john@example.com");
        var other = CreateMember("Jane", "jane@example.com");
        var group = CreateGroup(payer, other);

        var expense = Expense.Create(
            "Hotel", 500m, DateTime.Today, group, payer,
            forEveryone: true,
            sharedWith: new List<Member>());

        Assert.True(expense.ForEveryone);
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var payer = CreateMember("John", "john@example.com");
        var other = CreateMember("Jane", "jane@example.com");
        var group = CreateGroup(payer, other);

        var first = Expense.Create("A", 10m, DateTime.Today, group, payer, true, new List<Member>());
        var second = Expense.Create("B", 20m, DateTime.Today, group, payer, true, new List<Member>());

        Assert.NotEqual(first.Id, second.Id);
    }
}
