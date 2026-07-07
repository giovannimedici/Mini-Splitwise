using Minisplitwise.Application.Expenses;
using Minisplitwise.Application.Groups;
using Minisplitwise.Application.Interfaces;
using Minisplitwise.Application.Members;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Interfaces;

namespace Minisplitwise.Application.Services;

public class ExpenseService(
    IExpenseRepository expenseRepository,
    IGroupRepository groupRepository,
    IMemberRepository memberRepository) : IExpenseService
{
    public async Task<ExpenseResponseDto> CreateExpenseAsync(ExpenseRequestDto expenseRequestDto, CancellationToken cancellationToken = default)
    {
        var group = await groupRepository.GetGroupByIdAsync(expenseRequestDto.GroupId, cancellationToken);

        var paidBy = await memberRepository.GetMemberByIdAsync(expenseRequestDto.PaidById, cancellationToken);

        var sharedWith = await memberRepository.GetMembersByIdsAsync(expenseRequestDto.SharedWithIds, cancellationToken);

        Expense expense = Expense.Create(
            expenseRequestDto.Description,
            expenseRequestDto.Amount,
            expenseRequestDto.Date,
            group,
            paidBy,
            expenseRequestDto.ForEveryone,
            sharedWith
        );

        var createdExpense = await expenseRepository.CreateExpenseAsync(expense, cancellationToken);

        return new ExpenseResponseDto(
            createdExpense.Id,
            createdExpense.Description,
            createdExpense.Amount,
            createdExpense.Date,
            new GroupDto(createdExpense.Group.Id, createdExpense.Group.Name),
            new MemberDto(createdExpense.PaidBy.Id, createdExpense.PaidBy.Name),
            createdExpense.ForEveryone,
            createdExpense.SharedWith.Select(m => new MemberDto(m.Id, m.Name)).ToList()
        );
    }

    public async Task<List<ExpenseResponseDto>> GetExpensesByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var expenses = await expenseRepository.GetExpensesByGroupIdAsync(groupId, cancellationToken);

        return await Task.FromResult(expenses.Select(e => new ExpenseResponseDto(
            e.Id,
            e.Description,
            e.Amount,
            e.Date,
            new GroupDto(e.Group.Id, e.Group.Name),
            new MemberDto(e.PaidBy.Id, e.PaidBy.Name),
            e.ForEveryone,
            e.SharedWith.Select(m => new MemberDto(m.Id, m.Name)).ToList()
        )).ToList());
    }
}