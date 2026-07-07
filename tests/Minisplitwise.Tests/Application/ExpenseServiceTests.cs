using Minisplitwise.Application.Expenses;
using Minisplitwise.Application.Services;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Exceptions;
using Minisplitwise.Domain.Interfaces;
using NSubstitute;

namespace MiniSplitwise.Tests.Application;

public class ExpenseServiceTests
{
    private readonly IExpenseRepository _expenseRepository = Substitute.For<IExpenseRepository>();
    private readonly IGroupRepository _groupRepository = Substitute.For<IGroupRepository>();
    private readonly IMemberRepository _memberRepository = Substitute.For<IMemberRepository>();
    private readonly ExpenseService _service;

    public ExpenseServiceTests()
    {
        _service = new ExpenseService(_expenseRepository, _groupRepository, _memberRepository);
    }

    private static Member CreateMember(string name, string email)
        => Member.Create(name, email, new DateTime(1990, 1, 1));

    private static Group CreateGroup(string name, params Member[] members)
        => Group.Create(name, members.ToList());

    [Fact]
    public async Task CreateExpenseAsync_WithValidData_ReturnsMappedDto()
    {
        var payer = CreateMember("John", "john@example.com");
        var sharedMember = CreateMember("Jane", "jane@example.com");
        var group = CreateGroup("Trip", payer, sharedMember);
        var sharedWith = new List<Member> { sharedMember };
        var date = new DateTime(2026, 7, 1);

        var request = new ExpenseRequestDto(
            "Dinner",
            100m,
            date,
            group.Id,
            payer.Id,
            false,
            new List<Guid> { sharedMember.Id }
        );

        _groupRepository.GetGroupByIdAsync(group.Id, Arg.Any<CancellationToken>()).Returns(group);
        _memberRepository.GetMemberByIdAsync(payer.Id, Arg.Any<CancellationToken>()).Returns(payer);
        _memberRepository.GetMembersByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(sharedWith);
        _expenseRepository.CreateExpenseAsync(Arg.Any<Expense>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Expense>());

        var response = await _service.CreateExpenseAsync(request);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Dinner", response.Description);
        Assert.Equal(100m, response.Amount);
        Assert.Equal(date, response.Date);
        Assert.Equal(group.Id, response.Group.Id);
        Assert.Equal(group.Name, response.Group.Name);
        Assert.Equal(payer.Id, response.PaidBy.Id);
        Assert.Equal(payer.Name, response.PaidBy.Name);
        Assert.False(response.ForEveryone);
        Assert.Single(response.SharedWith);
        Assert.Equal(sharedMember.Id, response.SharedWith[0].Id);
        Assert.Equal(sharedMember.Name, response.SharedWith[0].Name);
    }

    [Fact]
    public async Task CreateExpenseAsync_WithForEveryoneTrue_ReturnsDtoWithForEveryoneFlag()
    {
        var payer = CreateMember("John", "john@example.com");
        var other = CreateMember("Jane", "jane@example.com");
        var group = CreateGroup("Trip", payer, other);

        var request = new ExpenseRequestDto(
            "Hotel",
            500m,
            DateTime.Today,
            group.Id,
            payer.Id,
            true,
            new List<Guid>()
        );

        _groupRepository.GetGroupByIdAsync(group.Id, Arg.Any<CancellationToken>()).Returns(group);
        _memberRepository.GetMemberByIdAsync(payer.Id, Arg.Any<CancellationToken>()).Returns(payer);
        _memberRepository.GetMembersByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(new List<Member>());
        _expenseRepository.CreateExpenseAsync(Arg.Any<Expense>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Expense>());

        var response = await _service.CreateExpenseAsync(request);

        Assert.True(response.ForEveryone);
        Assert.Empty(response.SharedWith);
    }

    [Fact]
    public async Task CreateExpenseAsync_PassesCreatedExpenseToRepository()
    {
        var payer = CreateMember("John", "john@example.com");
        var sharedMember = CreateMember("Jane", "jane@example.com");
        var group = CreateGroup("Trip", payer, sharedMember);
        var sharedWith = new List<Member> { sharedMember };

        var request = new ExpenseRequestDto(
            "Lunch",
            50m,
            DateTime.Today,
            group.Id,
            payer.Id,
            false,
            new List<Guid> { sharedMember.Id }
        );

        _groupRepository.GetGroupByIdAsync(group.Id, Arg.Any<CancellationToken>()).Returns(group);
        _memberRepository.GetMemberByIdAsync(payer.Id, Arg.Any<CancellationToken>()).Returns(payer);
        _memberRepository.GetMembersByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(sharedWith);
        _expenseRepository.CreateExpenseAsync(Arg.Any<Expense>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Expense>());

        await _service.CreateExpenseAsync(request);

        await _expenseRepository.Received(1).CreateExpenseAsync(
            Arg.Is<Expense>(e =>
                e.Description == "Lunch" &&
                e.Amount == 50m &&
                e.Group == group &&
                e.PaidBy == payer &&
                e.ForEveryone == false &&
                e.SharedWith.Count == 1
            ),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateExpenseAsync_WhenGroupNotFound_PropagatesNotFoundException()
    {
        var request = new ExpenseRequestDto(
            "Dinner",
            100m,
            DateTime.Today,
            Guid.NewGuid(),
            Guid.NewGuid(),
            false,
            new List<Guid> { Guid.NewGuid() }
        );

        _groupRepository.GetGroupByIdAsync(request.GroupId, Arg.Any<CancellationToken>())
            .Returns<Task<Group>>(_ => throw new NotFoundException("Group not found"));

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateExpenseAsync(request));

        Assert.Equal("Group not found", exception.Message);
        await _expenseRepository.DidNotReceive().CreateExpenseAsync(Arg.Any<Expense>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateExpenseAsync_WhenPaidByMemberNotFound_PropagatesNotFoundException()
    {
        var payer = CreateMember("John", "john@example.com");
        var other = CreateMember("Jane", "jane@example.com");
        var group = CreateGroup("Trip", payer, other);

        var request = new ExpenseRequestDto(
            "Dinner",
            100m,
            DateTime.Today,
            group.Id,
            Guid.NewGuid(),
            false,
            new List<Guid>()
        );

        _groupRepository.GetGroupByIdAsync(group.Id, Arg.Any<CancellationToken>()).Returns(group);
        _memberRepository.GetMemberByIdAsync(request.PaidById, Arg.Any<CancellationToken>())
            .Returns<Task<Member>>(_ => throw new NotFoundException("Member not found"));

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateExpenseAsync(request));

        Assert.Equal("Member not found", exception.Message);
        await _expenseRepository.DidNotReceive().CreateExpenseAsync(Arg.Any<Expense>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateExpenseAsync_WhenSharedWithMembersNotFound_PropagatesNotFoundException()
    {
        var payer = CreateMember("John", "john@example.com");
        var other = CreateMember("Jane", "jane@example.com");
        var group = CreateGroup("Trip", payer, other);

        var request = new ExpenseRequestDto(
            "Dinner",
            100m,
            DateTime.Today,
            group.Id,
            payer.Id,
            false,
            new List<Guid> { Guid.NewGuid() }
        );

        _groupRepository.GetGroupByIdAsync(group.Id, Arg.Any<CancellationToken>()).Returns(group);
        _memberRepository.GetMemberByIdAsync(payer.Id, Arg.Any<CancellationToken>()).Returns(payer);
        _memberRepository.GetMembersByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>())
            .Returns<Task<List<Member>>>(_ => throw new NotFoundException("Members not found"));

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateExpenseAsync(request));

        Assert.Equal("Members not found", exception.Message);
        await _expenseRepository.DidNotReceive().CreateExpenseAsync(Arg.Any<Expense>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetExpensesByGroupIdAsync_ReturnsMappedDtos()
    {
        var payer = CreateMember("John", "john@example.com");
        var sharedMember = CreateMember("Jane", "jane@example.com");
        var group = CreateGroup("Trip", payer, sharedMember);
        var sharedWith = new List<Member> { sharedMember };

        var expense1 = Expense.Create("Dinner", 100m, new DateTime(2026, 7, 1), group, payer, false, sharedWith);
        var expense2 = Expense.Create("Hotel", 500m, new DateTime(2026, 7, 2), group, payer, true, new List<Member>());

        var expenses = new List<Expense> { expense1, expense2 };

        _expenseRepository.GetExpensesByGroupIdAsync(group.Id, Arg.Any<CancellationToken>()).Returns(expenses);

        var response = await _service.GetExpensesByGroupIdAsync(group.Id);

        Assert.Equal(2, response.Count);

        Assert.Equal(expense1.Id, response[0].Id);
        Assert.Equal("Dinner", response[0].Description);
        Assert.Equal(100m, response[0].Amount);
        Assert.False(response[0].ForEveryone);
        Assert.Single(response[0].SharedWith);

        Assert.Equal(expense2.Id, response[1].Id);
        Assert.Equal("Hotel", response[1].Description);
        Assert.Equal(500m, response[1].Amount);
        Assert.True(response[1].ForEveryone);
        Assert.Empty(response[1].SharedWith);
    }

    [Fact]
    public async Task GetExpensesByGroupIdAsync_WhenNoExpenses_ReturnsEmptyList()
    {
        var groupId = Guid.NewGuid();
        _expenseRepository.GetExpensesByGroupIdAsync(groupId, Arg.Any<CancellationToken>()).Returns(new List<Expense>());

        var response = await _service.GetExpensesByGroupIdAsync(groupId);

        Assert.Empty(response);
    }

    [Fact]
    public async Task GetExpensesByGroupIdAsync_MapsAllFieldsCorrectly()
    {
        var payer = CreateMember("John Doe", "john@example.com");
        var sharedMember = CreateMember("Jane Smith", "jane@example.com");
        var group = CreateGroup("Paris Trip", payer, sharedMember);
        var sharedWith = new List<Member> { sharedMember };
        var date = new DateTime(2026, 7, 5, 14, 30, 0);

        var expense = Expense.Create("Restaurant Le Jules", 150.50m, date, group, payer, false, sharedWith);
        var expenses = new List<Expense> { expense };

        _expenseRepository.GetExpensesByGroupIdAsync(group.Id, Arg.Any<CancellationToken>()).Returns(expenses);

        var response = await _service.GetExpensesByGroupIdAsync(group.Id);

        Assert.Single(response);
        var dto = response[0];
        Assert.Equal(expense.Id, dto.Id);
        Assert.Equal("Restaurant Le Jules", dto.Description);
        Assert.Equal(150.50m, dto.Amount);
        Assert.Equal(date, dto.Date);
        Assert.Equal(group.Id, dto.Group.Id);
        Assert.Equal("Paris Trip", dto.Group.Name);
        Assert.Equal(payer.Id, dto.PaidBy.Id);
        Assert.Equal("John Doe", dto.PaidBy.Name);
        Assert.False(dto.ForEveryone);
        Assert.Single(dto.SharedWith);
        Assert.Equal(sharedMember.Id, dto.SharedWith[0].Id);
        Assert.Equal("Jane Smith", dto.SharedWith[0].Name);
    }
}
